using AmznStudio.Core.Logic;
using System;
using System.IO;

namespace AmznStudio.Core
{
    class Program
    {
        static string thridPage = "B079SN6CFQ";
        static string firstPage = "B076FRX9F8";

        static void Main(string[] args)
        {
            var keywords = File.ReadAllLines("Keywords.txt");
            
            var checker = new Checker();
            checker.ProcessKeywords(keywords, thridPage, res =>
            {
                System.Console.WriteLine("Processed " + res.ASIN + " " + res.Keyword + " Page: " + res.Page);
            });
            Console.ReadLine();
        }
    }
}
