﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
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
            htmlDoc.DocumentNode.Descendants()
                   .Where(n => n.Name == "script" || n.Name == "style")
                   ?.ToList()
                   ?.ForEach(n => n.Remove());

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

            string _headers = "//*[starts-with(name(),'h') and string-length(name()) = 2 and number(substring(name(), 2)) <= 6]";
            var headers = htmlDoc.DocumentNode.SelectNodes(_headers)?.Select(p => SanitizeIrrelevantContent(WebUtility.HtmlDecode(p.InnerText)));

            var _page = htmlDoc.DocumentNode.SelectSingleNode("//body");

            var _paragraphs = _page.SelectNodes("//p");
            var paragraphs = _paragraphs?.Select(p => SanitizeIrrelevantContent(WebUtility.HtmlDecode(p.InnerText)))
                .Where(p => p != "").ToList();

            var fullPage = _page.InnerText;

            return new WebsiteInfo
            {
                Id = url,
                Url = url,
                Title = title,
                DescriptionMeta = description,
                Paragraphs = paragraphs?.ToList(),
                Headers = headers?.ToList(),
                FullPageContent = SanitizeIrrelevantContent(fullPage),
                Language = _language,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now
            };
        }

        static public string SanitizeIrrelevantContent(string htmlContent)
        {
            var result = htmlContent;

            if (result == null || result == "")
            {
                return "";
            }

            result = Regex.Replace(result, @"\b\w{1,2}\b", "");//remove 1 letter words
            result = Regex.Replace(result, @"\s+", " ");       //remove newlines tabs and whitespaces

            return result;
        }

        static public async Task<List<string>> RetrieveRelatedWebsitesUrlsAsync(string url, HtmlDocument htmlDoc)
        {
            var sanitizedUrl = url.Split('#').First();

            var _refs = htmlDoc.DocumentNode.SelectNodes("//a[@href]");
            var _hrefs = _refs?.Select(r =>
            {
                int hrefAttributeIndex = r.Attributes
                    .Select(attr => attr.Name)
                    ?.ToList()
                    .IndexOf("href") ?? -1;
                if (hrefAttributeIndex != -1)
                    return r.Attributes[hrefAttributeIndex].Value;
                return null;
            });

            Uri baseUrl = new Uri(url);
            List<string> relatedWebsitesUrls = new List<string>();

            _hrefs?.ToList().ForEach(_href =>
            {
                #region Sanitizing
                var sanitizedReference = _href.Split('#').First();

                if (sanitizedReference.Equals(sanitizedUrl))
                {
                    return;
                }

                if (sanitizedReference.IndexOfAny(";".ToCharArray()) != -1)
                {
                    return;
                }

                if (sanitizedReference.Contains(".png") || sanitizedReference.Contains(".jpg") || sanitizedReference.Contains(".pdf") || sanitizedReference.Contains(".jpeg"))
                {
                    return;
                }
                #endregion

                if (sanitizedReference.StartsWith("http"))
                {
                    relatedWebsitesUrls.Add(sanitizedReference);
                }
                else if (sanitizedReference.StartsWith("//"))
                {
                    relatedWebsitesUrls.Add(baseUrl.Scheme + ":" + sanitizedReference);
                }
                else if (sanitizedReference.StartsWith("/"))
                {
                    relatedWebsitesUrls.Add(baseUrl.Scheme + "://" + baseUrl.Host + sanitizedReference);
                }
                else if (sanitizedReference.StartsWith("#"))
                {

                }
                else if (sanitizedReference.StartsWith("./"))
                {

                }
                else
                {
                    relatedWebsitesUrls.Add(baseUrl.Scheme + "://" + baseUrl.Host + "/" + sanitizedReference);
                }

            });

            return relatedWebsitesUrls?.ToList();
        }
    }
}
