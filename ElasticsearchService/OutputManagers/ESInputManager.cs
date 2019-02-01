using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Shared.Models;
using Shared;
using Shared.Interfaces;

namespace ElasticsearchService.OutputManagers
{
    public class ESInputManager<T> : IDisposable, IOutputManager<T>
    {
        private ConnectionConfiguration settings;
        protected ElasticLowLevelClient lowlevelClient;

        protected string index = Constants.VISITED_WEBSITES_INDEX;

        public ESInputManager()
        {
            settings = new ConnectionConfiguration(new Uri(Constants.ELASTICSEARCH_URL))
                .RequestTimeout(TimeSpan.FromMinutes(2));

            lowlevelClient = new ElasticLowLevelClient(settings);
        }

        public void OutputEntry(T retrievedInfo)
        {
            var indexResponse = lowlevelClient.Index<BytesResponse>(index, "_doc", PostData.Serializable(retrievedInfo));
            byte[] responseBytes = indexResponse.Body;
        }

        public async Task OutputEntryAsync(T retrievedInfo)
        {
            var asyncIndexResponse = await lowlevelClient.IndexAsync<StringResponse>(index, "_doc", PostData.Serializable(retrievedInfo));
            string responseString = asyncIndexResponse.Body;
        }

        public async Task OutputEntryAsync(T retrievedInfo, string Id)
        {
            var asyncIndexResponse = await lowlevelClient.IndexAsync<StringResponse>(index, "_doc", Id, PostData.Serializable(retrievedInfo));
            string responseString = asyncIndexResponse.Body;
        }

        public async Task BulkOutputAsync(List<PendingWebsite> items)
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
            string responseString = indexResponse.Body;
        }

        public void UpdateEntry(T retrievedInfo, string Id)
        {
            var asyncIndexResponse = lowlevelClient.Update<StringResponse>(index, "_doc", Id, PostData.Serializable(new { doc = retrievedInfo }));
            string responseString = asyncIndexResponse.Body;
        }

        public async Task UpdateEntryAsync(T retrievedInfo, string Id)
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
