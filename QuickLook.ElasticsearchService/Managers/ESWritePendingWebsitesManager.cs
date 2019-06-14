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
    public class ESWritePendingWebsitesManager : ESWriteManager<PendingWebsite>
    {
        private ConnectionSettings settings;
        private ElasticClient client;
        private Random rr;

        public ESWritePendingWebsitesManager()
        : base()
        {
            rr = new Random();
            index = Constants.PENDING_WEBSITES_INDEX;
            settings = new ConnectionSettings(new Uri(Constants.ELASTICSEARCH_URL))
                .DefaultIndex(index)
                .DefaultMappingFor<PendingWebsite>(i => i
                    .IndexName(index)
                    .TypeName(mapping)
                );

            client = new ElasticClient(settings);
        }

        public List<PendingWebsite> GetNextPendingBatchRandom(int BATCH_SIZE)
        {
            var searchResponse = lowlevelClient.Search<StringResponse>(index, mapping, PostData.Serializable(new
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
                                .Seed(rr.Next())
                            )
                        )
                    )
                )
            );

            if (searchResponse.Hits.Count == 0)
                throw new Exception("Elasticsearch Failure");

            return searchResponse.Hits.Select(h => h.Source).ToList();
        }

        public async Task BulkIndexAsync(List<PendingWebsite> items)
        {
            var data = new List<object>();

            items.ForEach(i =>
            {
                data.Add(new
                {
                    index = new
                    {
                        _index = index,
                        _id = i.Id,
                        _type = "_doc"
                    }
                });
                data.Add(i);
            });

            var indexResponse = await lowlevelClient.BulkAsync<StringResponse>(PostData.MultiJson(data));
            var response = indexResponse.Body;
        }
    }
}
