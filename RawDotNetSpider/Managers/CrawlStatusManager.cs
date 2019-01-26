using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElasticsearchService.OutputManagers;

namespace Spider.Managers
{
    public static class CrawlStatusManager
    {
        public static Dictionary<string, bool> PendingWebsites { get; set; }
        public static Dictionary<string, bool> VisitedWebsites { get; set; }
        public static NestClient EsClient { get; private set; }

        public static int visitedCount { get; private set; }

        private const int BATCH_SIZE = 10;

        public static void Init()
        {
            PendingWebsites = new Dictionary<string, bool>();
            VisitedWebsites = new Dictionary<string, bool>();
            EsClient = new NestClient();
        }

        public static string GetWebsiteIdIfAlreadyCrawled(string url)
        {
            return EsClient.GetWebsitesByUrl(url).First().Id;
        }

        public static bool IsWebsiteRecentlyIndexed(string url)
        {
            return EsClient.GetWebsitesByUrl(url).Count > 0;
        }

        public static bool IsUrlVisitedThisSession(string url)
        {
            return VisitedWebsites.Keys.Contains(url);
        }

        public static void MarkAsVisited(string url)
        {
            visitedCount++;

            if (!VisitedWebsites.Keys.Contains(url))
            {
                VisitedWebsites.Add(url, true);
            }

            if (PendingWebsites.Keys.Contains(url))
            {
                PendingWebsites.Remove(url);
            }
        }

        public static void IncreaseVisitedCount()
        {
            visitedCount++;
        }

        public static IEnumerable<string> TakeNextBatch()
        {
            return new List<string>(PendingWebsites.Keys.Take(BATCH_SIZE));
        }

        public static void AddPendingWebsite(string url)
        {
            if (!PendingWebsites.Keys.Contains(url))
                PendingWebsites.Add(url, true);
        }

        public static void AddPendingWebsites(List<string> urls)
        {
            foreach (string url in urls)
            {
                PendingWebsites.Add(url, true);
            }
        }
    }
}
