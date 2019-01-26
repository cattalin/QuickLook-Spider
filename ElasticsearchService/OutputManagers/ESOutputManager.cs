using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared;
using Shared.Models;

namespace ElasticsearchService.OutputManagers
{
    public class ESOutputManager
    {
        private ConnectionSettings settings;
        private ElasticClient client;

        private string index = Constants.VISITED_WEBSITES_INDEX;

        public ESOutputManager()
        {
            settings = new ConnectionSettings(new Uri("http://localhost:9200"))
                .DefaultIndex(index);

            client = new ElasticClient(settings);
        }

        public List<WebsiteInfo> WilcardSearch(string searchedContent, Pagination pagination)
        {
            var searchResult = client.Search<WebsiteInfo>(s => s
                .AllIndices()
                .AllTypes()
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

        public ISearchResponse<WebsiteInfo> FullTextSearch(string searchedContent, Pagination pagination)
        {
            var searchResult = client.Search<WebsiteInfo>(s => s
                .AllIndices()
                .AllTypes()
                .From(pagination.From)
                .Size(pagination.Take)
                .Query(q => q
                    .MultiMatch(w => w
                        .Fields(f => f.Field("Url").Field("Title").Field("DescriptionMeta").Field("Paragraphs"))
                        .Query(searchedContent)
                        .Fuzziness(Fuzziness.EditDistance(2))
                    )
                )
                .Highlight(h => h.PreTags("<b>").PostTags("</b>")
                    .Fields(f => f.Field("Url").Field("Title").Field("DescriptionMeta").Field("Paragraphs"))
                )

            );

            return searchResult;
        }

        public ISearchResponse<WebsiteInfo> FullTextSearch(string searchedContent)
        {
            var searchResult = client.Search<WebsiteInfo>(s => s
                .AllIndices()
                .AllTypes()
                .From(0)
                .Size(15)
                .Query(q => q
                    .MultiMatch(w => w
                        .Fields(f => f.Field("Url").Field("Title").Field("DescriptionMeta").Field("Paragraphs"))
                        .Query(searchedContent)
                        .Fuzziness(Fuzziness.EditDistance(1))
                    )
                )
                .Highlight(h => h.PreTags("<b>").PostTags("</b>")
                    .Fields(f => f.Field("Url").Field("Title").Field("DescriptionMeta").Field("Paragraphs"))
                )

            );

            return searchResult;
        }

        public List<WebsiteInfo> GetWebsitesByUrl(string url, Pagination pagination)
        {
            var searchResult = client.Search<WebsiteInfo>(s => s
                .AllIndices()
                .AllTypes()
                .From(pagination.From)
                .Size(pagination.Take)
                .Query(q => q
                    .ConstantScore(w => w
                        .Filter(f => f
                            .Term("Url", url))
                    )
                )
            );

            var results = searchResult.Documents;

            return results.ToList();
        }

        public IReadOnlyCollection<IHit<WebsiteInfo>> GetWebsitesByUrl(string url)
        {
            var searchResult = client.Search<WebsiteInfo>(s => s
                .AllIndices()
                .AllTypes()
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

    }
}
