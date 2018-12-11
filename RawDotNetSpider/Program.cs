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
            List<string> urlList = new List<string> { "https://www.youtube.com/watch?v=VcyFfcJbyeM" };
            CrawlStatusManager.Init();
            CrawlStatusManager.PendingWebsites.AddRange(urlList);

            IOutputManager outputManager = new ElasticsearchOutputManager();
//            IOutputManager outputManager = new FileOutputManager(filePath);

            CrawlManager crawlManager = new CrawlManager(outputManager);

//            ((ElasticsearchOutputManager) outputManager).Search();

            NestClient client = new NestClient();
            client.Search();

//            crawlManager.StartCrawlingAsync(urlList);

            Console.ReadLine();
        }
    }
}
