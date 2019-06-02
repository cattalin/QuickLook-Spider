using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Shared.Models;

namespace Spider.Managers
{
    static class UtilsAsync
    {
        static public async Task<HtmlDocument> LoadWebsiteAsync(string url)
        {
            try
            {
                var sanitizedUrl = WebUtility.UrlDecode(url);

                HttpClient httpClient = new HttpClient();
                var responseResult = await httpClient.GetAsync(sanitizedUrl);

                string res = responseResult.Content.ReadAsStringAsync().Result;
                var website = WebUtility.HtmlDecode(res);

                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(website);

                return htmlDoc;

            }
            catch (Exception ex)
            {
                Console.WriteLine("The given Url might be malformed ---> " + url);
                throw ex;
            }
        }

        static public HtmlDocument LoadWebsiteAsResponseMessage(string url)
        {
            try
            {
                var sanitizedUrl = WebUtility.UrlDecode(url);

                using (HttpClient client = new HttpClient())
                {
                    using (HttpResponseMessage response = client.GetAsync(url).Result)
                    {
                        var byteArray = response.Content.ReadAsByteArrayAsync().Result;
                        var res = Encoding.ASCII.GetString(byteArray, 0, byteArray.Length);

                        var website = WebUtility.HtmlDecode(res);

                        var htmlDoc = new HtmlDocument();
                        htmlDoc.LoadHtml(website);

                        return htmlDoc;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("The given Url might be malformed ---> " + url);
                throw ex;
            }
        }

        static public async Task<WebsiteInfo> RetrieveWebsiteInfoAsync(string url, HtmlDocument htmlDoc)
        {
            var _title = htmlDoc.DocumentNode.SelectSingleNode("//head//title");
            var title = _title?.InnerHtml;

            var _html = htmlDoc.DocumentNode.SelectSingleNode("//html");

            var _language = _html.HasAttributes
                            && _html.Attributes.Where(a => a.Name
                                .ToLower()
                                .Equals("lang")
                            ).Any()
                ? _html.Attributes["lang"].Value
                : "en";

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

            var _page = htmlDoc.DocumentNode.SelectSingleNode("//body");
            var _paragraphs = _page.SelectNodes("//p");

            var paragraphs = _paragraphs
                ?.Select(p => WebUtility.HtmlDecode(p.InnerText));

            var fullPage = _page.InnerText;

            return new WebsiteInfo
            {
                Id = url,
                Url = url,
                Title = title,
                DescriptionMeta = description,
                Paragraphs = paragraphs.ToList(),
                FullPageContent = fullPage,
                Language = _language,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now
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
                if (hrefAttributeIndex != -1)
                {
                    return r.Attributes[hrefAttributeIndex].Value;
                }

                return null;

            });


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
