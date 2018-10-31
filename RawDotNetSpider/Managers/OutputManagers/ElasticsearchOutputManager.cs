using System;
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

        public async void AddEntry(WebsiteInfo retrievedInfo)
        {
            //var indexResponse = lowlevelClient.Index<BytesResponse>("website", "website", PostData.Serializable(retrievedInfo));
            //byte[] responseBytes = indexResponse.Body;

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
