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
using QuickLook.Shared.Models;

namespace ElasticsearchService.OutputManagers
{
    public class ESWriteSuggestionsManager : ESWriteManager<string>
    {
        public ESWriteSuggestionsManager()
        {
            index = Constants.SEARCH_SUGGESTIONS_INDEX;
            mapping = Constants.DEFAULT_MAPPING_TYPE;
        }
    }
}
