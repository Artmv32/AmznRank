using System;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace AmznStudio.Core.Web
{
    public class AmazonWeb : IDisposable
    {
        private WebClient _client = new WebClient();
        private const string BaseUrl = "https://www.amazon.com/";

        public void Dispose()
        {
            if (_client != null)
            {
                _client.Dispose();
                _client = null;
            }
        }

        public string Download(string page)
        {
            return _client.DownloadString(page);
        }

        public Task<string> DownloadAsync(string page)
        {
            return _client.DownloadStringTaskAsync(page);
        }

        public async Task<int> SearchProductAsync(string keyword, string ASIN, uint page)
        {
            var query = BaseUrl + "s/field-keywords=" + UrlEncode(keyword) + "&page=" + page.ToString();
            var html = await DownloadAsync(query);
            if (html != null && html.Contains(ASIN))
            {
                return 1;
            }
            return -1;
        }

        public async Task<bool> GetIsRankedAsync(string keyword, string ASIN)
        {
            var query = BaseUrl + "s/field-keywords=" + ASIN + "+" + UrlEncode(keyword);
            var html = await DownloadAsync(query);
            return html != null && html.Contains(ASIN);
        }

        private string UrlEncode(string keyword)
        {
            return keyword.Replace(" ", "%20");
        }
    }
}
