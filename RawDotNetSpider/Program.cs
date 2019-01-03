using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Spider.Managers;
using ElasticsearchService.OutputManagers;

namespace Spider
{
    class Program
    {
        static string filePath = @"D:\Programare\ANUL 3\Licenta\Results\results" +
                                 DateTimeOffset.UtcNow.ToUnixTimeSeconds() + ".json";


        static void Main(string[] args)
        {
            List<string> urlList = new List<string> { "https://en.wikipedia.org/wiki/Stephen_III_of_Moldavia" };
            CrawlStatusManager.Init();
            CrawlStatusManager.AddPendingWebsites(urlList);

            IOutputManager outputManager = new ElasticsearchOutputManager();
//            IOutputManager outputManager = new FileOutputManager(filePath);

            CrawlManager crawlManager = new CrawlManager(outputManager);
            
            crawlManager.StartCrawlingAsync(urlList);

            Console.ReadLine();
        }
    }
}
