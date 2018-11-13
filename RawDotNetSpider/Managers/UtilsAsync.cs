using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace RawDotNetSpider.Managers
{
    static class UtilsAsync
    {
        static public async Task<HtmlDocument> LoadWebsiteAsync(string url)
        {
            try
            {
//                Task.Run(async () =>
                {
                    var sanitizedUrl = WebUtility.UrlDecode(url);

                    HttpClient httpClient = new HttpClient();
                    var responseResult = await httpClient.GetAsync(sanitizedUrl);
                    string website = responseResult.Content.ReadAsStringAsync().Result;

//                    string website = await httpClient.GetStringAsync(sanitizedUrl);

                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(website);

                    return htmlDoc;
                }
//                );

            }
            catch (Exception ex)
            {
                Console.WriteLine("The given Url might be malformated ---> " + url);
                throw ex;
            }
        }

        static public async Task<HtmlDocument> LoadWebsite(string url, HttpsSanitizerWebClient webclient)
        {
            try
            {
                var sanitizedUrl = WebUtility.UrlDecode(url);

                var website = webclient.DownloadString(sanitizedUrl);

                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(website);

                return htmlDoc;
            }
            catch (Exception ex)
            {
                Console.WriteLine("The given Url might be malformated ---> " + url);
                throw ex;
            }
        }

        static public async Task<WebsiteInfo> RetrieveWebsiteInfoAsync(string url, HtmlDocument htmlDoc)
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

        static public async Task<List<string>> RetrieveRelatedWebsitesUrlsAsync(string url, HtmlDocument htmlDoc)
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
