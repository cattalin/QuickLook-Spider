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

            var crawledWebsites = new ESWriteWebsitesManager();
            var pendingWebsites = new ESWritePendingWebsitesManager();
            var suggestions = new ESWriteSuggestionsManager();

            CrawlManager crawlManager = new CrawlManager(crawledWebsites, pendingWebsites, suggestions);
            
            crawlManager.StartCrawlingAsync();

            Console.ReadLine();
        }
    }
}
