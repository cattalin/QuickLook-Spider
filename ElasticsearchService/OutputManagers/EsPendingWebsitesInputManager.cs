using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticsearchService.OutputManagers
{
    public class ESPendingWebsitesInputManager : ESWebsitesInputManager
    {
        public ESPendingWebsitesInputManager()
        {
            this.index = "pending_websites";
        }
    }
}
