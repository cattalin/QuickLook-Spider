using System;
using System.Collections.Generic;
using System.IO;

namespace RawDotNetSpider
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> urlList = new List<string> { "https://www.piticigratis.com/" };
            CrawlManager crawler = new CrawlManager();

            crawler.StartCrawling(urlList);
        }
    }
}
