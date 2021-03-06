﻿using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared;
using Shared.Models;
using Shared.DTOs;

namespace ElasticsearchService.OutputManagers
{
    public class ESReadWebsitesManager
    {
        private ConnectionSettings settings;
        private ElasticClient client;

        private string index = Constants.VISITED_WEBSITES_INDEX;
        protected string mapping = Constants.DEFAULT_MAPPING_TYPE;

        public ESReadWebsitesManager()
        {
            settings = new ConnectionSettings(new Uri(Constants.ELASTICSEARCH_URL))
                .DefaultFieldNameInferrer(d => { return d.First().ToString().ToUpper() + d.Substring(1); })
                .EnableDebugMode(response =>
                {
                    if (response.RequestBodyInBytes != null)
                    {
                        Console.WriteLine($"Requested " +
                                          $"{Encoding.UTF8.GetString(response.RequestBodyInBytes)}\n" +
                                          $"{new string('-', 30)}\n");
                    }
                    else
                    {
                        Console.WriteLine($"Requested without body");
                    }
                    if (response.ResponseBodyInBytes != null)
                    {
                        Console.WriteLine($"Status: {response.HttpStatusCode}\n" +
                                          $"{Encoding.UTF8.GetString(response.ResponseBodyInBytes)}\n" +
                                          $"{new string('-', 30)}\n");
                    }
                    else
                    {
                        Console.WriteLine($"Status: {response.HttpStatusCode}\n" +
                                          $"{new string('-', 30)}\n");
                    }
                })
                .DefaultIndex(index);

            client = new ElasticClient(settings);
        }

        public ISearchResponse<WebsiteInfo> FullTextSearchAdvanced(SearchContentDTO searchContent, SearchPagination pagination)
        {
            if (searchContent.MatchUncrawledWebsites)
            {
                return FullTextSearchPendingWebsites(searchContent, pagination);
            }

            if (searchContent.MatchExactSentence)
            {
                return FullTextSearchMatchSentence(searchContent, pagination);
            }

            var searchResult = client.Search<WebsiteInfo>(s => s
                    .Index(Constants.VISITED_WEBSITES_INDEX)
                    .Type(mapping)
                    .From(pagination.From)
                    .Size(pagination.Take)
                    .Query(q => q
                        .Bool(b => b
                            .Must(
                                m =>
                                m.MultiMatch(w => w
                                    .Fields(f => f.Field("Url").Field("Title").Field("DescriptionMeta")
                                        .Field("Paragraphs").Field("FullPageContent"))
                                    .Query(searchContent.Input)
                                    .Fuzziness(Fuzziness.EditDistance(searchContent.Fuzziness))
                                ),
                                m => m.Term(t => t.Language, searchContent.Language),
                                m => m.DateRange(t => t
                                    .Field(f => f.CreateDate)
                                    .GreaterThanOrEquals(searchContent.StartDate)
                                    .LessThanOrEquals(searchContent.EndDate))
                                )
                            )
                        )
                    .Highlight(h => h.PreTags("<b>").PostTags("</b>")
                        .Fields(f => f.Field("Url").Field("Title").Field("DescriptionMeta")
                            .Field("Paragraphs").Field("FullPageContent"))
                    )
                );

            return searchResult;
        }

        public ISearchResponse<WebsiteInfo> FullTextSearchMatchSentence(SearchContentDTO searchContent, SearchPagination pagination)
        {
            var searchResult = client.Search<WebsiteInfo>(s => s
                    .Index(Constants.VISITED_WEBSITES_INDEX)
                    .Type(mapping)
                    .From(pagination.From)
                    .Size(pagination.Take)
                    .Query(q => q
                        .Bool(b => b
                            .Must(
                                m => m.MatchPhrase(w => w
                                    .Field(p => p.FullPageContent)
                                    .Query(searchContent.Input)
                                ),
                                m => m.Term(t => t.Language, searchContent.Language)
                            )
                        )
                    )
                    .Highlight(h => h.PreTags("<b>").PostTags("</b>")
                        .Fields(f => f.Field("Url").Field("Title").Field("DescriptionMeta")
                            .Field("Paragraphs").Field("FullPageContent"))
                    )
                );

            return searchResult;
        }

        public ISearchResponse<WebsiteInfo> FullTextSearchPendingWebsites(SearchContentDTO searchContent, SearchPagination pagination)
        {
            var searchResult = client.Search<WebsiteInfo>(s => s
                .Index(Constants.PENDING_WEBSITES_INDEX)
                .Type(mapping)
                .From(pagination.From)
                .Size(pagination.Take)
                .Query(q => q
                    .Bool(b => b
                        .Must(m => m
                            .MultiMatch(w => w
                            .Fields(f => f.Field("Url"))
                            .Query(searchContent.Input)
                            .Fuzziness(Fuzziness.EditDistance(searchContent.Fuzziness)))
                        )
                    )
                )
                .Highlight(h => h.PreTags("<b>").PostTags("</b>")
                .Fields(f => f.Field("Url"))
                )
            );

            return searchResult;
        }

        public ISearchResponse<WebsiteInfo> FullTextSearch(string searchedContent, SearchPagination pagination)
        {
            var searchResult = client.Search<WebsiteInfo>(s => s
                .Index(Constants.VISITED_WEBSITES_INDEX)
                .Type(mapping)
                .From(pagination.From)
                .Size(pagination.Take)
                .Query(q => q
                    .Bool(b => b
                        .Must(
                            m => m.MultiMatch(w => w
                                .Fields(f => f.Field("Url").Field("Title").Field("DescriptionMeta").Field("Paragraphs"))
                                .Query(searchedContent)
                                .Fuzziness(Fuzziness.EditDistance(2)))
                            , m => m.Term(t => t.Language, "en")
                            )
                    //.Should(m => m.Term(t => t.Language, "en"))
                    )
                )
                .Highlight(h => h
                    .PreTags("<b>").PostTags("</b>")
                    .Fields(f => f.Field("Url").Field("Title").Field("DescriptionMeta").Field("Paragraphs"))
                )
            );

            return searchResult;
        }

        public ISearchResponse<WebsiteInfo> FullTextSearch(string searchedContent)
        {
            var searchResult = client.Search<WebsiteInfo>(s => s
                .AllIndices()
                .Type(mapping)
                .From(0)
                .Size(15)
                .Query(q => q
                    .MultiMatch(w => w
                        .Fields(f => f.Field("Url").Field("Title").Field("DescriptionMeta").Field("Paragraphs"))
                        .Query(searchedContent)
                        .Fuzziness(Fuzziness.EditDistance(1))
                    )
                )
                .Highlight(h => h
                    .PreTags("<b>").PostTags("</b>")
                    .Fields(f => f.Field("Url").Field("Title").Field("DescriptionMeta").Field("Paragraphs"))
                )
            );

            return searchResult;
        }

        public IReadOnlyCollection<IHit<WebsiteInfo>> GetWebsitesByUrl(string url)
        {
            var searchResult = client.Search<WebsiteInfo>(s => s
                .AllIndices()
                .Type(Constants.VISITED_WEBSITES_INDEX)
                .From(0)
                .Size(15)
                .Query(q => q
                    .ConstantScore(w => w
                        .Filter(f => f
                            .Term("Url", url))
                    )
                )
            );

            var results = searchResult.Hits;

            return results.ToList();
        }

        public List<WebsiteInfo> WilcardSearch(string searchedContent, SearchPagination pagination)
        {
            var searchResult = client.Search<WebsiteInfo>(s => s
                .AllIndices()
                .Type(mapping)
                .From(pagination.From)
                .Size(pagination.Take)
                .Query(q => q
                    .Wildcard(w => w
                        .Field("Url")
                        .Value("*" + searchedContent + "*")
                    )
                )
            );

            var results = searchResult.Documents;

            return results.ToList();
        }

        public List<WebsiteInfo> WilcardSearch(string searchedContent)
        {
            var searchResult = client.Search<WebsiteInfo>(s => s
                .AllIndices()
                .AllTypes()
                .From(0)
                .Size(15)
                .Query(q => q
                    .Wildcard(w => w
                        .Field("Url")
                        .Value("*" + searchedContent + "*")
                    )
                )
            );

            var results = searchResult.Documents;

            return results.ToList();
        }
    }
}
