using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
