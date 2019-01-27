using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Spider.Managers;
using ElasticsearchService.OutputManagers;
using Shared.Interfaces;
using Shared.Models;
using System.Web;

namespace Spider
{
    class Program
    {
        static string filePath = @"D:\Programare\ANUL 3\Licenta\Results\results" +
                                 DateTimeOffset.UtcNow.ToUnixTimeSeconds() + ".json";


        static void Main(string[] args)
        {
            List<string> urlList = new List<string> { "https://www.youtube.com/watch?v=e8CLsYzE5wk" };
            CrawlStatusManager.Init();
            CrawlStatusManager.AddPendingWebsites(urlList);

            ESWebsitesInputManager crawledWebsites = new ESWebsitesInputManager();
            ESPendingWebsitesInputManager pendingWebsites = new ESPendingWebsitesInputManager();
//            IOutputManager outputManager = new FileOutputManager(filePath);

            List<PendingWebsite> test = new List<PendingWebsite>()
            {
                new PendingWebsite()
                {
                    Id = "https://en.wikipedia.org/wiki/Bogdan_II_of_Moldavia123",
                    CreateDate = DateTime.Now.AddYears(-2),
                    Url = "https://en.wikipedia.org/wiki/Bogdan_II_of_Moldavia"
                },
                new PendingWebsite()
                {
                    Id = "https://en.wikipedia.org/wiki/Bogdan_II_of_Moldavia321",
                    CreateDate = DateTime.Now.AddYears(-2),
                    Url = "https://en.wikipedia.org/wiki/Bogdan_II_of_Moldavia666"
                }
            };

//            pendingWebsites.BulkOutputAsync(test);
//            pendingWebsites.GetNextPendingBatchRandom(12);
//            pendingWebsites.GetNextPendingBatchRandomNest(12);

            CrawlManager crawlManager = new CrawlManager(crawledWebsites, pendingWebsites);
            
            crawlManager.StartCrawlingAsync(urlList);

            Console.ReadLine();
        }
    }
}
