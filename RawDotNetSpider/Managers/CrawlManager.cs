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
using RawDotNetSpider.Managers;

namespace RawDotNetSpider
{
    public class CrawlManager
    {
        private HttpsSanitizerWebClient httpsSanitizerWebClient;

        private IOutputManager outputManager;

        private List<Task> crawlTasks = new List<Task>();

        public CrawlManager(IOutputManager outputManager)
        {
            this.outputManager = outputManager;
            UtilsAsync.visitedWebsites = new Dictionary<string, bool>();
            Utils.visitedWebsites = new Dictionary<string, bool>();
            this.httpsSanitizerWebClient = new HttpsSanitizerWebClient();
        }

        public async Task StartCrawlingAsync(List<string> urlSeeds)
        {
            ParseWebsiteRecursivelyAsync(urlSeeds.First());

            Console.ReadLine();
        }

        public void StartCrawling(List<string> urlSeeds)
        {
            //ParseRecursively(urlSeeds);

            //ParseQueue(urlSeeds);

            ParseWebsiteRecursivelyAsync(urlSeeds.First());


            //using (esOutputManager = new ElasticsearchOutputManager())
            //{
            //ParseWebsiteRecursivelyAsync(urlSeeds.First(), esOutputManager);
            //}

            Console.ReadLine();
        }

        public async Task PeriodicFooAsync(TimeSpan interval, CancellationToken cancellationToken)
        {
            while (true)
            {
//                await FooAsync();
                await Task.Delay(interval, cancellationToken);
            }
        }

        public async Task ParseWebsiteRecursivelyAsync(string url)
        {
            if (!UtilsAsync.visitedWebsites.Keys.Contains(url))//remove all these contains. elasticsearch should handle those
            {
                try
                {
                    Console.WriteLine("New website" + url);

                    Stopwatch stopwatch;

                    stopwatch = Stopwatch.StartNew();

                    var htmlDoc = await UtilsAsync.LoadWebsiteAsync(url);

                    var retrievedInfo = await UtilsAsync.RetrieveWebsiteInfoAsync(url, htmlDoc);

                    await outputManager.OutputEntryAsync(retrievedInfo);

                    var relatedWebsiteUrls = await UtilsAsync.RetrieveRelatedWebsitesUrlsAsync(url, htmlDoc);


                    
                    stopwatch.Stop();

                    Console.WriteLine("Time Elapsed: " + stopwatch.ElapsedMilliseconds + " for another " +
                                      relatedWebsiteUrls.Count + " websites.");

                    //Parallel.ForEach(relatedWebsiteUrls, relatedWebsiteUrl =>
                    //{
                    //    //if (resultsCount < MAX_RESULTS && relatedWebsiteUrls != null && relatedWebsiteUrls.Count() > 0)
                    //    {
                    //        //Task.Run(() => ParseWebsiteRecursivelyAsync(relatedWebsiteUrl, outputManager));
                    //        ParseWebsiteRecursivelyAsync(relatedWebsiteUrl, outputManager); 
                    //    }
                    //});

                    foreach (var relatedWebsiteUrl in relatedWebsiteUrls)
                    {
                        Task.Run(() => ParseWebsiteRecursivelyAsync(relatedWebsiteUrl));
//                        crawlTasks.Add(Task.Run(() => ParseWebsiteRecursivelyAsync(relatedWebsiteUrl)));
                    }



                }
                catch (Exception ex)
                {
                    Console.WriteLine("Untreated error appeared. Skipping ---> " + url);
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
                    if (!Utils.visitedWebsites.Keys.Contains(url))
                    {
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
                    if (!Utils.visitedWebsites.Keys.Contains(url))
                    {
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