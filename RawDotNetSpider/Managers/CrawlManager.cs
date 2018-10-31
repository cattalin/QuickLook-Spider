using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace RawDotNetSpider
{
    public class CrawlManager
    {
        private Dictionary<string, bool> visitedWebsites;
        private HttpsSanitizerWebClient httpsSanitizerWebClient;
        private int resultsCount = 0;
        private const int MAX_RESULTS = 10;
        string filePath = @"D:\Programare\ANUL 3\Licenta\Results\results" +
                          DateTimeOffset.UtcNow.ToUnixTimeSeconds() + ".json";

        private FileOutputManager fileOutputManager;
        private ElasticsearchOutputManager esOutputManager;

        public CrawlManager()
        {
            this.visitedWebsites = new Dictionary<string, bool>();
            this.httpsSanitizerWebClient = new HttpsSanitizerWebClient();
        }

        public void StartCrawling(List<string> urlSeeds)
        {
                resultsCount = 0;

            using (fileOutputManager = new FileOutputManager(filePath))
            {
                //ParseRecursively(urlSeeds);

                //ParseQueue(urlSeeds);

                ParseWebsiteAsync(urlSeeds.First(), fileOutputManager);
            }

            //using (esOutputManager = new ElasticsearchOutputManager())
            //{
            //    //ParseRecursively(urlSeeds);

            //    //ParseQueue(urlSeeds, esOutputManager);

            //    ParseWebsiteAsync(urlSeeds.First(), esOutputManager);
            //}

            Console.ReadLine();
        }

        public async void ParseWebsiteAsync(string url, IOutputManager outputManager)
        {
            if (!visitedWebsites.Keys.Contains(url))
            {
                try
                {
                    Console.WriteLine("New website" + url);

                    var htmlDoc = LoadWebsite(url);

                    var retrievedInfo = RetrieveWebsiteInfo(url, htmlDoc);

                    outputManager.AddEntry(retrievedInfo);

                    var relatedWebsiteUrls = RetrieveRelatedWebsitesUrls(url, htmlDoc);

                    Console.WriteLine(resultsCount);

                    resultsCount++;
                    foreach (string relatedWebsiteUrl in relatedWebsiteUrls)
                    {
                        if (resultsCount < MAX_RESULTS && relatedWebsiteUrls != null && relatedWebsiteUrls.Count() > 0)
                        {
                            Task.Run(() => ParseWebsiteAsync(relatedWebsiteUrl, outputManager));
                        }
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
            while (i < urlList.Count && i <= MAX_RESULTS)
            {
                string url = urlList[i++];
                try
                {
                    if (!visitedWebsites.Keys.Contains(url))
                    {
                        var htmlDoc = LoadWebsite(url);

                        var retrievedInfo = RetrieveWebsiteInfo(url, htmlDoc);

                        outputManager.AddEntry(retrievedInfo);

                        var relatedWebsiteUrls = RetrieveRelatedWebsitesUrls(url, htmlDoc);

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
                    if (!visitedWebsites.Keys.Contains(url))
                    {
                        var htmlDoc = LoadWebsite(url);

                        var retrievedInfo = RetrieveWebsiteInfo(url, htmlDoc);

                        fileOutputManager.AddEntry(retrievedInfo);

                        var relatedWebsiteUrls = RetrieveRelatedWebsitesUrls(url, htmlDoc);

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

        private HtmlDocument LoadWebsite(string url)
        {
            visitedWebsites.Add(url, true);

            try
            {
                var sanitizedUrl = WebUtility.UrlDecode(url);


                //var website = httpsSanitizerWebClient.DownloadString(sanitizedUrl);

                HttpClient httpClient = new HttpClient();
                var responseResult = httpClient.GetAsync(sanitizedUrl);
                string website = responseResult.Result.Content.ReadAsStringAsync().Result;
                

                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(website);

                return htmlDoc;
            }
            catch(Exception ex)
            {
                Console.WriteLine("The given Url might be malformated ---> " + url);
                throw ex;
            }
        }

        private WebsiteInfo RetrieveWebsiteInfo(string url, HtmlDocument htmlDoc)
        {
            var _title = htmlDoc.DocumentNode.SelectSingleNode("//head//title");
            var title = _title?.InnerHtml;


            var _metas = htmlDoc.DocumentNode.SelectNodes("//meta");
            var metaDesc = _metas
                                ?.Where(m => m.HasAttributes && m.Attributes
                                              .Where(a => a.Name
                                                            .ToLower()
                                                            .Equals("name")
                                                          && a.Value
                                                            .ToLower()
                                                            .Equals("description"))
                                              .Any());
            var description = metaDesc?.FirstOrDefault()
                                      ?.Attributes
                                      ?.Where(a => a.Name
                                                    .ToLower()
                                                    .Equals("content"))
                                      ?.FirstOrDefault()
                                      ?.Value;

            return new WebsiteInfo
            {
                Url = url,
                Title = title,
                DescriptionMeta = description,
                CreateDate = DateTime.Now
            };
        }

        private List<string> RetrieveRelatedWebsitesUrls(string url, HtmlDocument htmlDoc)
        {

            var _refs = htmlDoc.DocumentNode.SelectNodes("//a[@href]");
            var _hrefs = _refs?.Select(r =>
            {
                int hrefAttributeIndex = r.Attributes
                    .Select(attr => attr.Name)
                    .ToList()
                    .IndexOf("href");
                if (hrefAttributeIndex != null && hrefAttributeIndex != -1)
                {
                    return r.Attributes[hrefAttributeIndex].Value;
                }

                return null;

            });

            //var relatedWebsitesUrls = _hrefs
            //                 ?.Where(_href => _href.StartsWith("http"))
            //                 ?.Select(_href => _href);
            //return relatedWebsitesUrls?.ToList();


            Uri baseUrl = new Uri(url);

            List<string> relatedWebsitesUrls = new List<string>();

            _hrefs.ToList().ForEach(_href =>
            {
                if (_href.StartsWith("http"))
                {
                    relatedWebsitesUrls.Add(_href);
                }
                else if (_href.StartsWith("//"))
                {
                    relatedWebsitesUrls.Add(baseUrl.Scheme + ":" + _href);
                }
                else if (_href.StartsWith("/"))
                {
                    relatedWebsitesUrls.Add(baseUrl.Scheme + "://" + baseUrl.Host + _href);
                }
                else if (_href.StartsWith("./"))
                {

                }
                else if (_href.StartsWith("#"))
                {

                }
                else
                {
                    
                }

            });

            return relatedWebsitesUrls?.ToList();
        }
    }
}