using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Shared.Models;
using Shared;
using Shared.Interfaces;

namespace ElasticsearchService.OutputManagers
{
    public class ESWriteManager<T> : IOutputManager<T>, IDisposable 
    {
        private ConnectionConfiguration settings;
        protected ElasticLowLevelClient lowlevelClient;

        protected string index = Constants.VISITED_WEBSITES_INDEX;
        protected string mapping = Constants.DEFAULT_MAPPING_TYPE;

        public ESWriteManager()
        {
            settings = new ConnectionConfiguration(new Uri(Constants.ELASTICSEARCH_URL))
                .RequestTimeout(TimeSpan.FromMinutes(2));

            lowlevelClient = new ElasticLowLevelClient(settings);
        }

        public void OutputEntry(T retrievedInfo)
        {
            var indexResponse = lowlevelClient.Index<BytesResponse>(index, mapping, PostData.Serializable(retrievedInfo));
            var response = indexResponse.Body;
        }

        public async Task IndexEntryAsync(T retrievedInfo)
        {
            var asyncIndexResponse = await lowlevelClient.IndexAsync<StringResponse>(index, mapping, PostData.Serializable(retrievedInfo));
            var response = asyncIndexResponse.Body;
        }

        public async Task IndexEntryAsync(T retrievedInfo, string Id)
        {
            var asyncIndexResponse = await lowlevelClient.IndexAsync<StringResponse>(index, mapping, Id, PostData.Serializable(retrievedInfo));
            var response = asyncIndexResponse.Body;
        }

        public async Task UpdateEntryAsync(T retrievedInfo, string Id)
        {
            var asyncIndexResponse = await lowlevelClient.UpdateAsync<StringResponse>(index, mapping, Id, PostData.Serializable(new { doc = retrievedInfo }));
            var response = asyncIndexResponse.Body;
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

        public void Search()
        {
            var searchResponse = lowlevelClient.Search<StringResponse>(index, mapping, PostData.Serializable(new
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
