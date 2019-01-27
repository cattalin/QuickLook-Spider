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
        private HttpsSanitizerWebClient httpsSanitizerWebClient;

        private ESWebsitesInputManager crawledWebsites;
        private ESPendingWebsitesInputManager pendingWebsites;

        private List<Task> crawlTasks = new List<Task>();

        public CrawlManager(ESWebsitesInputManager crawledWebsites, ESPendingWebsitesInputManager pendingWebsites)
        {
            this.crawledWebsites = crawledWebsites;
            this.pendingWebsites = pendingWebsites;
            this.httpsSanitizerWebClient = new HttpsSanitizerWebClient();
        }

        public async Task StartCrawlingAsync(List<string> urlSeeds)
        {
            //ParseWebsiteRecursivelyAsync(urlSeeds.First());
            //using (esOutputManager = new ESWebsitesInputManager())
            //{
            //ParseWebsiteRecursivelyAsync(urlSeeds.First(), esOutputManager);
            //}

            await PeriodicCrawlAsync(new TimeSpan(0, 0, 1), new CancellationToken(false));
        }

        public async Task PeriodicCrawlAsync(TimeSpan interval, CancellationToken cancellationToken)
        {
            while (true)
            {
                await CrawlBatch();
                await Task.Delay(interval, cancellationToken);
            }
        }

        public async Task CrawlBatch()
        {
            await Task.Run(() =>
            {
                Parallel.ForEach(pendingWebsites.GetNextPendingBatchRandomNest(Constants.BATCH_SIZE), pendingWeb =>
                    {
                        ParseWebsiteAsync(pendingWeb.Url);
                    });
//                Parallel.ForEach(CrawlStatusManager.TakeNextBatch(), pendingUrl =>
//                {
//                    if (!CrawlStatusManager.IsUrlVisitedThisSession(pendingUrl))
//                    {
//                        ParseWebsiteAsync(pendingUrl);
//                    }
//                    else CrawlStatusManager.IncreaseVisitedCount();
//                });
            });

        }

        public async Task ParseWebsiteAsync(string currentUrl)
        {
            try
            {
                Stopwatch stopwatch;

                stopwatch = Stopwatch.StartNew();

                CrawlStatusManager.MarkAsVisited(currentUrl);

                var htmlDoc = await UtilsAsync.LoadWebsiteAsync(currentUrl);

                var retrievedInfo = await UtilsAsync.RetrieveWebsiteInfoAsync(currentUrl, htmlDoc);

                var existingDocId = CrawlStatusManager.GetWebsiteIdIfAlreadyCrawled(currentUrl);
                if (existingDocId == null)
                {
                    await crawledWebsites.OutputEntryAsync(retrievedInfo);
                }
                else
                {
                    await crawledWebsites.UpdateEntryAsync(retrievedInfo, existingDocId);
                }

                var relatedWebsiteUrls = await UtilsAsync.RetrieveRelatedWebsitesUrlsAsync(currentUrl, htmlDoc);

                stopwatch.Stop();

                Console.WriteLine(
                    $@"Time Elapsed: {stopwatch.ElapsedMilliseconds} for crawling {currentUrl} with another {relatedWebsiteUrls.Count} referenced websites.");


                var pendingWebsites = new List<PendingWebsite>();
                relatedWebsiteUrls.ForEach(w =>
                {
                    pendingWebsites.Add(new PendingWebsite()
                    {
                        CreateDate = DateTime.Now,
                        Id = w,
                        Url = w
                    });
                });

                await Task.Run(() =>
                {
                    return this.pendingWebsites.BulkOutputAsync(pendingWebsites);
                });

//                await Task.Run(() =>
//                {
//                    foreach (var relatedWebsiteUrl in relatedWebsiteUrls)
//                    {
//                        if (!CrawlStatusManager.IsUrlVisitedThisSession(relatedWebsiteUrl))
//                        {
//                            CrawlStatusManager.AddPendingWebsite(relatedWebsiteUrl);
//                        }
//                    }
//                });

            }
            catch (Exception ex)
            {
                Console.WriteLine("Untreated error appeared. Skipping ---> " + currentUrl);
            }

        }

        public async Task ParseWebsiteRecursivelyAsync(string currentUrl)
        {
            if (!CrawlStatusManager.IsWebsiteRecentlyIndexed(currentUrl))//remove all these contains. elasticsearch should handle those
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