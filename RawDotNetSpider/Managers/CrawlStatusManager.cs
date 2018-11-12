using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RawDotNetSpider.Managers
{
    public static class CrawlStatusManager
    {
        public static List<string> PendingWebsites { get; set; }
        public static Dictionary<string, bool> VisitedWebsites { get; set; }

        public static void Init()
        {
            PendingWebsites = new List<string>();
            VisitedWebsites = new Dictionary<string, bool>();
        }
    }
}
