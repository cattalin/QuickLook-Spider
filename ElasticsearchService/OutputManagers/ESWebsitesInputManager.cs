using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Models;

namespace ElasticsearchService.OutputManagers
{
    public class ESWebsitesInputManager : ESInputManager<WebsiteInfo>
    {
        public ESWebsitesInputManager()
        {
            this.index = "websites";
        }
    }
}
