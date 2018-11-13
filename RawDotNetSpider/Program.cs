using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using RawDotNetSpider.Managers;

namespace RawDotNetSpider
{
    class Program
    {
        static string filePath = @"D:\Programare\ANUL 3\Licenta\Results\results" +
                          DateTimeOffset.UtcNow.ToUnixTimeSeconds() + ".json";

        static void Main(string[] args)
        {
            List<string> urlList = new List<string> { "https://www.youtube.com/watch?v=VcyFfcJbyeM" };
            CrawlStatusManager.Init();
            CrawlStatusManager.PendingWebsites.AddRange(urlList);
//            CrawlManager crawlManager = new CrawlManager(new FileOutputManager(filePath));
            CrawlManager crawlManager = new CrawlManager(new ElasticsearchOutputManager());

            crawlManager.StartCrawlingAsync(urlList);
            
            Console.ReadLine();
        }
    }
}
