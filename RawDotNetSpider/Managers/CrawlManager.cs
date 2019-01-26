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

namespace Spider.Managers
{
    public class CrawlManager
    {
        private HttpsSanitizerWebClient httpsSanitizerWebClient;

        private IOutputManager outputManager;

        private List<Task> crawlTasks = new List<Task>();

        public CrawlManager(IOutputManager outputManager)
        {
            this.outputManager = outputManager;
            this.httpsSanitizerWebClient = new HttpsSanitizerWebClient();
        }

        public async Task StartCrawlingAsync(List<string> urlSeeds)
        {
            //ParseWebsiteRecursivelyAsync(urlSeeds.First());
            //using (esOutputManager = new ElasticsearchOutputManager())
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
                Parallel.ForEach(CrawlStatusManager.TakeNextBatch(), pendingUrl =>
                {
                    if (!CrawlStatusManager.IsUrlVisitedThisSession(pendingUrl))
                    {
                        ParseWebsiteAsync(pendingUrl);
                    }
                    else CrawlStatusManager.IncreaseVisitedCount();
                });
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
                    await outputManager.OutputEntryAsync(retrievedInfo);
                }
                else
                {
                    await outputManager.UpdateEntryAsync(retrievedInfo, existingDocId);
                }

                var relatedWebsiteUrls = await UtilsAsync.RetrieveRelatedWebsitesUrlsAsync(currentUrl, htmlDoc);

                stopwatch.Stop();

                Console.WriteLine(
                    $@"Time Elapsed: {stopwatch.ElapsedMilliseconds} for crawling {currentUrl} with another {relatedWebsiteUrls.Count} referenced websites.");

                await Task.Run(() =>
                {
                    foreach (var relatedWebsiteUrl in relatedWebsiteUrls)
                    {
                        if (!CrawlStatusManager.IsUrlVisitedThisSession(relatedWebsiteUrl))
                        {
                            CrawlStatusManager.AddPendingWebsite(relatedWebsiteUrl);
                        }
                    }
                });

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

                    await outputManager.OutputEntryAsync(retrievedInfo);

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

        public void ParseQueue(List<string> urlList, IOutputManager outputManager)
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

                        outputManager.OutputEntry(retrievedInfo);

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