using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ElasticsearchService.OutputManagers;
using Shared;
using QuickLook.Shared.Mappers;

namespace Spider.Managers
{
    public class CrawlManager
    {
        private ESWriteWebsitesManager crawledWebsites;
        private ESWritePendingWebsitesManager pendingWebsites;
        private ESWriteSuggestionsManager suggestions;

        private List<Task> crawlTasks = new List<Task>();

        public CrawlManager(ESWriteWebsitesManager crawledWebsites, ESWritePendingWebsitesManager pendingWebsites, ESWriteSuggestionsManager suggestions)
        {
            this.crawledWebsites = crawledWebsites;
            this.pendingWebsites = pendingWebsites;
            this.suggestions = suggestions;
        }

        public void StartCrawlingAsync()
        {
            StartCrawlThreadsAsync();
        }

        public async Task StartCrawlingAsyncOld(List<string> urlSeeds)
        {
            await PeriodicCrawlAsyncOld(new TimeSpan(0, 0, 1), new CancellationToken(false));
        }

        public async Task PeriodicCrawlAsyncOld(TimeSpan interval, CancellationToken cancellationToken)
        {
            while (true)
            {
                await CrawlBatchOld();
                //                await Task.Delay(interval, cancellationToken);
            }
        }

        public async Task CrawlBatchOld()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            Console.WriteLine("STARTED_BATCH");

            //            await Task.Run(() =>
            //            {
            //                Parallel.ForEach(pendingWebsites.GetNextPendingBatchRandomNest(Constants.BATCH_SIZE),
            //                    pendingWeb => { ParseWebsiteAsync(pendingWeb.Url); });
            //            });

            List<Task> tasks = new List<Task>();
            pendingWebsites.GetNextPendingBatchRandomNest(Constants.MAX_THREADS).ForEach(async pendingWeb =>
            {
                //tasks.Add(ParseWebsiteAsync(pendingWeb.Url));
            });


            await Task.WhenAll(tasks);

            Console.WriteLine($"ENDED_BATCH({stopwatch.ElapsedMilliseconds})");
        }

        public void StartCrawlThreadsAsync()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            Console.WriteLine("STARTED_BATCH");

            for (var i = 0; i < Constants.MAX_THREADS; i++)
            {
                Task.Run(async () =>
                {
                    while (true)
                    {
                        try
                        {
                            await ParseWebsiteAsync();
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

        public async Task ParseWebsiteAsync()
        {
            var currentUrl = "";

            try
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                var pendingWebsite = this.pendingWebsites.GetNextPendingBatchRandomNest(1).First();
                currentUrl = pendingWebsite.Url;

                var htmlDoc = await UtilsAsync.LoadWebsiteAsync(currentUrl);
                var retrievedInfo = await UtilsAsync.RetrieveWebsiteInfoAsync(currentUrl, htmlDoc);
                await crawledWebsites.IndexEntryAsync(retrievedInfo, retrievedInfo.Id);

                var relatedWebsiteUrls = await UtilsAsync.RetrieveRelatedWebsitesUrlsAsync(currentUrl, htmlDoc);
                var relatedUrls = relatedWebsiteUrls.ToPendingWebsites();
                await this.pendingWebsites.BulkIndexAsync(relatedUrls);

                var retrievedSuggestions = retrievedInfo.ExtractFromCrawledDataAsStrings();
                await suggestions.BulkIndexAsync(retrievedSuggestions);

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

                    await crawledWebsites.IndexEntryAsync(retrievedInfo);

                    var relatedWebsiteUrls = await UtilsAsync.RetrieveRelatedWebsitesUrlsAsync(currentUrl, htmlDoc);


                    stopwatch.Stop();

                    Console.WriteLine($@"Time Elapsed: {stopwatch.ElapsedMilliseconds} for crawling {currentUrl} with another {relatedWebsiteUrls.Count} referenced websites.");

                    foreach (var relatedWebsiteUrl in relatedWebsiteUrls)
                    {
                        //Task.Run(() => ParseWebsiteRecursivelyAsync(relatedWebsiteUrl));
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Untreated error appeared. Skipping ---> " + currentUrl);
                }
            }
        }

        public void ParseQueue(List<string> urlList, ESWriteWebsitesManager outputManager)
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

                        //if (relatedWebsiteUrls != null && relatedWebsiteUrls.Count() > 0)
                        //    ParseRecursively(relatedWebsiteUrls);
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