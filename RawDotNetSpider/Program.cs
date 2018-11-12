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
            CrawlManager crawlManager = new CrawlManager(new FileOutputManager(filePath));
            //CrawlManager crawlManager = new CrawlManager(new ElasticsearchOutputManager());

            crawlManager.StartCrawlingAsync(urlList);

//            var periodicCrawler = new PeriodicCrawl();
//
//            Task.Run(() => periodicCrawler.PeriodicFooAsync(new TimeSpan(0, 0, 1), new CancellationToken(false)));

            Console.ReadLine();
        }
    }

    public class PeriodicCrawl
    {


        public async Task PeriodicFooAsync(TimeSpan interval, CancellationToken cancellationToken)
        {
            while (true)
            {
                //                await FooAsync();
                Console.WriteLine("periodic");
                await Task.Delay(interval, cancellationToken);
            }
        }
    }
}
