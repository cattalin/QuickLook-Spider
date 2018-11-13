using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace RawDotNetSpider.Managers
{
    static class Utils
    {
        static public HtmlDocument LoadWebsite(string url)
        {
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
            catch (Exception ex)
            {
                Console.WriteLine("The given Url might be malformated ---> " + url);
                throw ex;
            }
        }

        static public WebsiteInfo RetrieveWebsiteInfo(string url, HtmlDocument htmlDoc)
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

        static public List<string> RetrieveRelatedWebsitesUrls(string url, HtmlDocument htmlDoc)
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
