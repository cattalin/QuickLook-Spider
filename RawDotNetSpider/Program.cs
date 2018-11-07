using System;
using System.Collections.Generic;
using System.IO;

namespace RawDotNetSpider
{
    class Program
    {
        static string filePath = @"D:\Programare\ANUL 3\Licenta\Results\results" +
                          DateTimeOffset.UtcNow.ToUnixTimeSeconds() + ".json";

        static void Main(string[] args)
        {
            List<string> urlList = new List<string> { "https://www.youtube.com/watch?v=VcyFfcJbyeM" };
            CrawlManager crawler = new CrawlManager(new FileOutputManager(filePath));
//            CrawlManager crawler = new CrawlManager(new ElasticsearchOutputManager());

            crawler.StartCrawlingAsync(urlList);
        }
    }
}
