using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Shared;
using Shared.Models;
using Shared.DTOs;
using QuickLook.Shared.Models;

namespace ElasticsearchService.OutputManagers
{
    public class ESReadSuggestionsManager
    {
        private ConnectionSettings settings;
        private ElasticClient client;

        private string index = Constants.SEARCH_SUGGESTIONS_INDEX;
        protected string mapping = Constants.DEFAULT_MAPPING_TYPE;

        public ESReadSuggestionsManager()
        {
            settings = new ConnectionSettings(new Uri(Constants.ELASTICSEARCH_URL))
                .DefaultFieldNameInferrer(d => { return d.ToLower(); })
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

        public ISearchResponse<SearchSuggestion> PrefixSearch(string searchContent)
        {
            var searchResult = client.Search<SearchSuggestion>(s => s
                .Index(Constants.SEARCH_SUGGESTIONS_INDEX)
                .Type(mapping)
                .Size(10)
                .Query(q => q
                    .Prefix(p => p
                        .Field("name.keywordstring")
                        .Value(searchContent)
                    )
                )
            );

            return searchResult;
        }

        public ISearchResponse<SearchSuggestion> EdgeNGramSearch(string searchContent)
        {
            var searchResult = client.Search<SearchSuggestion>(s => s
                .Index(Constants.SEARCH_SUGGESTIONS_INDEX)
                .Type(mapping)
                .Size(10)
                .Query(q => q
                    .Match(p => p
                        .Field("name.edgengram")
                        .Query(searchContent)
                    )
                )
            );

            return searchResult;
        }

        public ISearchResponse<SearchSuggestion> CompletionSuggesterSearch(string searchContent)
        {
            var searchResult = client.Search<SearchSuggestion>(s => s
                .Index(Constants.SEARCH_SUGGESTIONS_INDEX)
                .Type(mapping)
                .Size(10)
                .Suggest(q => q
                    .Completion("search-suggest-fuzzy", c => c
                        .Fuzzy(f => f
                            .Fuzziness(Fuzziness.EditDistance(1))
                        )
                        .Prefix(searchContent)
                        .Field("name.completion")
                        .SkipDuplicates(true)
                    )
                )
            );

            return searchResult;
        }
    }
}
