using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RawDotNetSpider.OutputManagers
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

        public void Search()
        {
            var searchResponse = client.Search<WebsiteInfo>(s => s
                .AllIndices()
                .AllTypes()
                .Query(q => q
                    .Wildcard(w => w
                        .Field("Url")
                        .Value("*.com")
                    )
                )
            );

            var responses = searchResponse.Hits;
        }
    }
}
