using System;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Shared.Models;
using Shared;
using Shared.Interfaces;

namespace ElasticsearchService.OutputManagers
{
    public class ESWebsitesInputManager : IDisposable, IOutputManager
    {
        private ConnectionConfiguration settings;
        private ElasticLowLevelClient lowlevelClient;

        protected string index = Constants.VISITED_WEBSITES_INDEX;

        public ESWebsitesInputManager()
        {
            settings = new ConnectionConfiguration(new Uri(Constants.ELASTICSEARCH_URL))
                .RequestTimeout(TimeSpan.FromMinutes(2));

            lowlevelClient = new ElasticLowLevelClient(settings);
        }

        public void OutputEntry(WebsiteInfo retrievedInfo)
        {
            var indexResponse = lowlevelClient.Index<BytesResponse>(index, "_doc", PostData.Serializable(retrievedInfo));
            byte[] responseBytes = indexResponse.Body;
        }

        public async Task OutputEntryAsync(WebsiteInfo retrievedInfo)
        {
            var asyncIndexResponse = await lowlevelClient.IndexAsync<StringResponse>(index, "_doc", PostData.Serializable(retrievedInfo));
            string responseString = asyncIndexResponse.Body;
        }

        public async Task UpdateEntry(WebsiteInfo retrievedInfo, string Id)
        {
            var asyncIndexResponse = lowlevelClient.Update<StringResponse>(index, "_doc", Id, PostData.Serializable(new { doc = retrievedInfo }));
            string responseString = asyncIndexResponse.Body;
        }

        public async Task UpdateEntryAsync(WebsiteInfo retrievedInfo, string Id)
        {
            var asyncIndexResponse = await lowlevelClient.UpdateAsync<StringResponse>(index, "_doc", Id, PostData.Serializable(new { doc = retrievedInfo }));
            string responseString = asyncIndexResponse.Body;
        }

        public void Search()
        {
            var searchResponse = lowlevelClient.Search<StringResponse>(index, "_doc", PostData.Serializable(new
            {
                from = 0,
                size = 10,
                query = new
                {
                    wildcard = new
                    {
                        Url = "*.com"
                    }
                }
            }));

            var successful = searchResponse.Success;
            var responseJson = searchResponse.Body;
        }

        public bool isDisposed = false;

        public void Dispose()
        {
            if (!isDisposed)
            {
                isDisposed = true;
                this.Dispose();
            }
        }
    }
}
