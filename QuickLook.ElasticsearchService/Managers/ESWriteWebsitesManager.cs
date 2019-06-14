using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Models;

namespace ElasticsearchService.OutputManagers
{
    public class ESWriteWebsitesManager : ESWriteManager<WebsiteInfo>
    {
        public ESWriteWebsitesManager()
        {
            index = "websites";
        }
    }
}
