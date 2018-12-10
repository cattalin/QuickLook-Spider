﻿using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spider.OutputManagers
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
                .From(0)
                .Size(30)
                .Query(q => q
                    .Wildcard(w => w
                        .Field("Url")
                        .Value("*.com*")
                    )
                )
            );

            var responses = searchResponse.Hits;
        }
    }
}
