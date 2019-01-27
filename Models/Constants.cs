
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public static class Constants
    {
        public const string ELASTICSEARCH_URL = "http://localhost:9200";
        public const string VISITED_WEBSITES_INDEX = "websites";
        public const string PENDING_WEBSITES_INDEX = "pending_websites";

        public const int BATCH_SIZE = 20;
    }
}
