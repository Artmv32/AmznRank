using AmznStudio.Core.Web;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace AmznStudio.Core.Logic
{
    public class CheckResult
    {
        public DateTime Date { get; set; }

        public string Keyword { get; set; }

        public string ASIN { get; set; }

        public int Page { get; set; }
    }

    public class Checker
    {
        public void ProcessKeywords(IEnumerable<string> keywords, string ASIN, Action<CheckResult> onProcessed)
        {
            var po = new ParallelOptions
            {
                MaxDegreeOfParallelism = 2,
            };

            var failed = new ConcurrentBag<string>();
            var list = keywords;
            do
            {
                Parallel.ForEach(list, po, async kwd =>
                {
                    try
                    {
                        using (var amz = new AmazonWeb())
                        {
                            var isRanked = await amz.GetIsRankedAsync(kwd, ASIN);
                            int pos = -1;

                            if (isRanked)
                            {
                                for (int i = 1; i < 15; i++)
                                {
                                    var position = await amz.SearchProductAsync(kwd, ASIN, (uint)i);
                                    if (position != -1)
                                    {
                                        pos = i;
                                        break;
                                    }
                                }
                            }

                            var result = new CheckResult
                            {
                                ASIN = ASIN,
                                Date = DateTime.UtcNow,
                                Page = pos,
                                Keyword = kwd,
                            };
                            onProcessed?.Invoke(result);
                        }
                    }
                    catch (WebException)
                    {
                        failed.Add(kwd);
                    }
                });

                list = failed.ToArray();
                if (list.Any())
                {
                    Thread.Sleep(3000);
                }

            } while (list.Any());            
        }
    }
}
