using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using RawDotNetSpider.Managers;

namespace RawDotNetSpider
{
    public class CrawlManager
    {
        private HttpsSanitizerWebClient httpsSanitizerWebClient;
        private int resultsCount = 0;
        private const int MAX_RESULTS = 10;
        string filePath = @"D:\Programare\ANUL 3\Licenta\Results\results" +
                          DateTimeOffset.UtcNow.ToUnixTimeSeconds() + ".json";

        private FileOutputManager fileOutputManager;
        private ElasticsearchOutputManager esOutputManager;

        private List<Task> crawlTasks = new List<Task>();

        public CrawlManager()
        {
            UtilsAsync.visitedWebsites = new Dictionary<string, bool>();
            Utils.visitedWebsites = new Dictionary<string, bool>();
            this.httpsSanitizerWebClient = new HttpsSanitizerWebClient();
        }

        public void StartCrawling(List<string> urlSeeds)
        {
                resultsCount = 0;

            fileOutputManager = new FileOutputManager(filePath);

            //ParseRecursively(urlSeeds);

            //ParseQueue(urlSeeds);

            ParseWebsiteAsync(urlSeeds.First(), fileOutputManager);
            //ParseQueue(urlSeeds, fileOutputManager);

            //fileOutputManager.Dispose();


            //using (esOutputManager = new ElasticsearchOutputManager())
            //{
            //    //ParseRecursively(urlSeeds);

            //    //ParseQueue(urlSeeds, esOutputManager);

            //ParseWebsiteAsync(urlSeeds.First(), esOutputManager);
            //}

            Console.ReadLine();
        }

        public async Task ParseWebsiteAsync(string url, IOutputManager outputManager)
        {
            if (!UtilsAsync.visitedWebsites.Keys.Contains(url))//remove all these contains. elasticsearch should handle those
            {
                try
                {
                    Console.WriteLine("New website" + url);

                    var htmlDoc = await UtilsAsync.LoadWebsiteAsync(url);

                    var retrievedInfo = await UtilsAsync.RetrieveWebsiteInfoAsync(url, htmlDoc);

                    outputManager.AddEntry(retrievedInfo);

                    var relatedWebsiteUrls = await UtilsAsync.RetrieveRelatedWebsitesUrlsAsync(url, htmlDoc);

                    Console.WriteLine(resultsCount);

                    resultsCount++;


                    var stopwatch = Stopwatch.StartNew();


                    //Parallel.ForEach(relatedWebsiteUrls, relatedWebsiteUrl =>
                    //{
                    //    //if (resultsCount < MAX_RESULTS && relatedWebsiteUrls != null && relatedWebsiteUrls.Count() > 0)
                    //    {
                    //        //Task.Run(() => ParseWebsiteAsync(relatedWebsiteUrl, outputManager));
                    //        ParseWebsiteAsync(relatedWebsiteUrl, outputManager); 
                    //    }
                    //});

                    foreach (var relatedWebsiteUrl in relatedWebsiteUrls)
                    {
                        //Task.Run(() => ParseWebsiteAsync(relatedWebsiteUrl, outputManager));
                        crawlTasks.Add(Task.Run(() => ParseWebsiteAsync(relatedWebsiteUrl, outputManager)));
                    }

                    stopwatch.Stop();

                    Console.WriteLine("Time Elapsed: " + stopwatch.ElapsedMilliseconds + " for another " +
                                      relatedWebsiteUrls.Count + " websites.");


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
            while (i < urlList.Count && i <= MAX_RESULTS)
            {
                string url = urlList[i++];
                try
                {
                    if (!Utils.visitedWebsites.Keys.Contains(url))
                    {
                        var htmlDoc = Utils.LoadWebsite(url);

                        var retrievedInfo = Utils.RetrieveWebsiteInfo(url, htmlDoc);

                        outputManager.AddEntry(retrievedInfo);

                        var relatedWebsiteUrls = Utils.RetrieveRelatedWebsitesUrls(url, htmlDoc);

                        Console.WriteLine(i);

                        if (resultsCount < MAX_RESULTS && relatedWebsiteUrls != null && relatedWebsiteUrls.Count() > 0)
                        {
                            resultsCount += relatedWebsiteUrls.Count;
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

        public void ParseRecursively(List<string> urlList, IOutputManager outputManager)
        {
            urlList.ForEach(url =>
            {
                try
                {
                    if (!Utils.visitedWebsites.Keys.Contains(url))
                    {
                        var htmlDoc = Utils.LoadWebsite(url);

                        var retrievedInfo = Utils.RetrieveWebsiteInfo(url, htmlDoc);

                        fileOutputManager.AddEntry(retrievedInfo);

                        var relatedWebsiteUrls = Utils.RetrieveRelatedWebsitesUrls(url, htmlDoc);

                        if (resultsCount < MAX_RESULTS && relatedWebsiteUrls != null && relatedWebsiteUrls.Count() > 0)
                            ParseRecursively(relatedWebsiteUrls, outputManager);
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