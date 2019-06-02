using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using ElasticsearchService.OutputManagers;
using Shared.Interfaces;
using Shared.Models;
using Shared;

namespace Spider.Managers
{
    public class CrawlManager
    {
        private ESWebsitesInputManager crawledWebsites;
        private ESPendingWebsitesInputManager pendingWebsites;
        private ESSuggestionsInputManager suggestions;

        private List<Task> crawlTasks = new List<Task>();

        public CrawlManager(ESWebsitesInputManager crawledWebsites, ESPendingWebsitesInputManager pendingWebsites)
        {
            this.crawledWebsites = crawledWebsites;
            this.pendingWebsites = pendingWebsites;
        }

        public async Task StartCrawlingAsync(List<string> urlSeeds)
        {
            await StartCrawlThreadsAsync();
            //            await PeriodicCrawlAsync(new TimeSpan(0, 0, 1), new CancellationToken(false));
        }

        public async Task PeriodicCrawlAsync(TimeSpan interval, CancellationToken cancellationToken)
        {
            while (true)
            {
                await CrawlBatch();
                //                await Task.Delay(interval, cancellationToken);
            }
        }

        public async Task CrawlBatch()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            Console.WriteLine("STARTED_BATCH");

            //            await Task.Run(() =>
            //            {
            //                Parallel.ForEach(pendingWebsites.GetNextPendingBatchRandomNest(Constants.BATCH_SIZE),
            //                    pendingWeb => { ParseWebsiteAsync(pendingWeb.Url); });
            //            });

            List<Task> tasks = new List<Task>();
            pendingWebsites.GetNextPendingBatchRandomNest(Constants.BATCH_SIZE).ForEach(async pendingWeb =>
            {
                tasks.Add(ParseWebsiteAsync(pendingWeb.Url));
            });


            await Task.WhenAll(tasks);

            Console.WriteLine($"ENDED_BATCH({stopwatch.ElapsedMilliseconds})");
        }

        public async Task StartCrawlThreadsAsync()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            Console.WriteLine("STARTED_BATCH");

            for (var i = 0; i < Constants.BATCH_SIZE; i++)
            {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                Task.Run(async () =>
                {
                    while (true)
                    {
                        try
                        {
                            var pendingWebsite = pendingWebsites.GetNextPendingBatchRandomNest(1).First();
                            await ParseWebsiteAsync(pendingWebsite.Url);
                        }
                        catch (Exception Ex)
                        {
                            Console.WriteLine(Ex.Message);
                        }
                    }
                });
            }

            Console.WriteLine($"ENDED_BATCH({stopwatch.ElapsedMilliseconds})");
        }

        public async Task ParseWebsiteAsync(string currentUrl)
        {
            try
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                var htmlDoc = await UtilsAsync.LoadWebsiteAsync(currentUrl);

                var retrievedInfo = await UtilsAsync.RetrieveWebsiteInfoAsync(currentUrl, htmlDoc);

                await crawledWebsites.OutputEntryAsync(retrievedInfo, retrievedInfo.Id);

                var relatedWebsiteUrls = await UtilsAsync.RetrieveRelatedWebsitesUrlsAsync(currentUrl, htmlDoc);
                //var pendingWebsites = Utils.ConvertUrlsToModelList(relatedWebsiteUrls);
                //await this.pendingWebsites.BulkOutputAsync(pendingWebsites);

                await suggestions.BulkOutputAsync(retrievedInfo);

                stopwatch.Stop();

                Console.WriteLine($@"Time Elapsed: {stopwatch.ElapsedMilliseconds} for crawling {currentUrl} with another {relatedWebsiteUrls.Count} referenced websites.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Untreated error appeared. Skipping ---> {currentUrl} --- {ex.Message}");
            }
        }

        public async Task ParseWebsiteRecursivelyAsync(string currentUrl)
        {
            if (!CrawlStatusManager.IsWebsiteRecentlyIndexed(currentUrl))
            {
                try
                {
                    Console.WriteLine("New website" + currentUrl);

                    Stopwatch stopwatch;

                    stopwatch = Stopwatch.StartNew();

                    CrawlStatusManager.MarkAsVisited(currentUrl);

                    var htmlDoc = await UtilsAsync.LoadWebsiteAsync(currentUrl);

                    var retrievedInfo = await UtilsAsync.RetrieveWebsiteInfoAsync(currentUrl, htmlDoc);

                    await crawledWebsites.OutputEntryAsync(retrievedInfo);

                    var relatedWebsiteUrls = await UtilsAsync.RetrieveRelatedWebsitesUrlsAsync(currentUrl, htmlDoc);


                    stopwatch.Stop();

                    Console.WriteLine($@"Time Elapsed: {stopwatch.ElapsedMilliseconds} for crawling {currentUrl} with another {relatedWebsiteUrls.Count} referenced websites.");

                    foreach (var relatedWebsiteUrl in relatedWebsiteUrls)
                    {
                        Task.Run(() => ParseWebsiteRecursivelyAsync(relatedWebsiteUrl));
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Untreated error appeared. Skipping ---> " + currentUrl);
                }
            }
        }

        public void ParseQueue(List<string> urlList, ESWebsitesInputManager outputManager)
        {
            int i = 0;
            while (i < urlList.Count)
            {
                string url = urlList[i++];
                try
                {
                    if (!CrawlStatusManager.IsWebsiteRecentlyIndexed(url))
                    {

                        CrawlStatusManager.MarkAsVisited(url);

                        var htmlDoc = Utils.LoadWebsite(url);

                        var retrievedInfo = Utils.RetrieveWebsiteInfo(url, htmlDoc);

                        outputManager.OutputEntry(retrievedInfo);

                        var relatedWebsiteUrls = Utils.RetrieveRelatedWebsitesUrls(url, htmlDoc);

                        Console.WriteLine(i);

                        if (relatedWebsiteUrls != null && relatedWebsiteUrls.Count() > 0)
                        {
                            urlList.AddRange(relatedWebsiteUrls);
                        }

                    }
                    else Console.WriteLine("Website --> ALREADY VISITED -->" + url + i);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Untreated error appeared. Skipping ---> " + url);
                }
            }
        }

        public void ParseRecursively(List<string> urlList)
        {
            urlList.ForEach(url =>
            {
                try
                {
                    if (!CrawlStatusManager.IsWebsiteRecentlyIndexed(url))
                    {

                        CrawlStatusManager.MarkAsVisited(url);

                        var htmlDoc = Utils.LoadWebsite(url);

                        var retrievedInfo = Utils.RetrieveWebsiteInfo(url, htmlDoc);

                        crawledWebsites.OutputEntry(retrievedInfo);

                        var relatedWebsiteUrls = Utils.RetrieveRelatedWebsitesUrls(url, htmlDoc);

                        if (relatedWebsiteUrls != null && relatedWebsiteUrls.Count() > 0)
                            ParseRecursively(relatedWebsiteUrls);
                    }
                    else Console.WriteLine("Website --> ALREADY VISITED -->" + url);
                }
                catch (Exception)
                {
                    Console.WriteLine("Untreated error appeared. Skipping ---> " + url);
                }
            });
        }
    }
}