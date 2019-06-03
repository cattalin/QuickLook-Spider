using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Shared;
using Shared.Models;
using Shared.DTOs;

namespace ElasticsearchService.OutputManagers
{
    public class ESSuggestionsInputManager : ESInputManager<WebsiteInfo>
    {
        public ESSuggestionsInputManager()
        {
            index = "search-suggestions";
        }
    }
}
