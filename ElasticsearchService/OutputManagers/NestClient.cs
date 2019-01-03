using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Models;

namespace ElasticsearchService.OutputManagers
{
    public class NestClient
    {
        private ConnectionSettings settings;
        private ElasticClient client;

        private string index = "websites";

        public NestClient()
        {
            settings = new ConnectionSettings(new Uri("http://localhost:9200"))
                .DefaultIndex(index);

            client = new ElasticClient(settings);
        }

        public List<WebsiteInfo> WilcardSearch(string searchedContent)
        {
            var searchResult = client.Search<WebsiteInfo>(s => s
                .AllIndices()
                .AllTypes()
                .From(0)
                .Size(30)
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

        public List<WebsiteInfo> FullTextSearch(string searchedContent)
        {
            var searchResult = client.Search<WebsiteInfo>(s => s
                .AllIndices()
                .AllTypes()
                .From(0)
                .Size(30)
                .Query(q => q
                    .MultiMatch(w => w
                        .Fields(f => f.Field("Url").Field("Title").Field("DescriptionMeta"))
                        .Query(searchedContent)
                        .Fuzziness(Fuzziness.EditDistance(2))
                    )
                )

            );


            var results = searchResult.Documents;

            return results.ToList();
        }

        public List<WebsiteInfo> GetWebsitesByUrl(string url)
        {
            var searchResult = client.Search<WebsiteInfo>(s => s
                .AllIndices()
                .AllTypes()
                .From(0)
                .Size(30)
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

    }
}
