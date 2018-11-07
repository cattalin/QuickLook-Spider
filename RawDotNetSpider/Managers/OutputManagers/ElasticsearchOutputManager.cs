using System;
using System.Threading.Tasks;
using Elasticsearch.Net;

namespace RawDotNetSpider
{ 
    public class ElasticsearchOutputManager : IDisposable, IOutputManager
    {
        private ConnectionConfiguration settings;
        private ElasticLowLevelClient lowlevelClient;

        public ElasticsearchOutputManager()
        {
            settings = new ConnectionConfiguration(new Uri("http://localhost:9200"))
                .RequestTimeout(TimeSpan.FromMinutes(2));

            lowlevelClient = new ElasticLowLevelClient(settings);
        }

        public void OutputEntry(WebsiteInfo retrievedInfo)
        {
            var indexResponse = lowlevelClient.Index<BytesResponse>("website", "_doc", PostData.Serializable(retrievedInfo));
            byte[] responseBytes = indexResponse.Body;
        }

        public async Task OutputEntryAsync(WebsiteInfo retrievedInfo)
        {
            var asyncIndexResponse = await lowlevelClient.IndexAsync<StringResponse>("website", "_doc", PostData.Serializable(retrievedInfo));
            string responseString = asyncIndexResponse.Body;
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
