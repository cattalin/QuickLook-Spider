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

        public NestClient()
        {
            settings = new ConnectionSettings(new Uri("http://localhost:9200"))
                .DefaultIndex("website");

            client = new ElasticClient(settings);
        }

        public List<WebsiteInfo> Search(string searchedContent)
        {
            var searchResponse = client.Search<WebsiteInfo>(s => s
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

            var responses = searchResponse.Documents;

            return responses.ToList();
        }
    }
}
