using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Nest;
using Shared;
using Shared.Models;

namespace ElasticsearchService.OutputManagers
{
    public class ESPendingWebsitesInputManager : ESInputManager<PendingWebsite>
    {
        private ConnectionSettings settings;
        private ElasticClient client;

        public ESPendingWebsitesInputManager()
        : base()
        {
            this.index = Constants.PENDING_WEBSITES_INDEX;
            settings = new ConnectionSettings(new Uri(Constants.ELASTICSEARCH_URL))
                .DefaultIndex(index)
                .DefaultMappingFor<PendingWebsite>(i => i
                    .IndexName(index)
                    .TypeName("_doc")
                );

            client = new ElasticClient(settings);
        }

        public List<PendingWebsite> GetNextPendingBatchRandom(int BATCH_SIZE)
        {

            var searchResponse = lowlevelClient.Search<StringResponse>(index, "_doc", PostData.Serializable(new
            {
                size = BATCH_SIZE,
                query = new
                {
                    function_score = new
                    {
                        functions = new[]
                        {
                            new
                            {
                                random_score = new
                                {
                                    seed = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                                }
                            }
                        }
                    }
                }
            }));

            var successful = searchResponse.Success;
            var responseJson = searchResponse.Body;

            return new List<PendingWebsite>();
        }

        public List<PendingWebsite> GetNextPendingBatchRandomNest(int BATCH_SIZE)
        {
            var searchResponse = client.Search<PendingWebsite>(s => s
                .Size(BATCH_SIZE)
                .Query(q => q
                    .FunctionScore(f => f
                        .Functions(ff => ff
                            .RandomScore(rs => rs
                                .Seed(DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                            )
                        )
                    )
                )
            );

            return searchResponse.Hits.Select(h => h.Source).ToList();
        }
    }
}
