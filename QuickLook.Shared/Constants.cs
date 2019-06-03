
using System;
using System.Collections.Generic;
using System.Text;
using Shared.Models;

namespace Shared
{
    public static class Constants
    {
        public const string ELASTICSEARCH_URL = "http://localhost:9200";
        public const string VISITED_WEBSITES_INDEX = "websites";
        public const string PENDING_WEBSITES_INDEX = "pending_websites";
        public const string SEARCH_SUGGESTIONS_INDEX = "search_suggestions";
        public const string DEFAULT_MAPPING_TYPE = "_doc";

        public const int BATCH_SIZE = 20;

        public static Language[] Languages =
        {
            new Language()
            {
                Name = "English",
                Key = "en"
            },
            new Language()
            {
                Name = "Romana",
                Key = "ro"
            },
            new Language()
            {
                Name = "French",
                Key = "fr"
            },
            new Language()
            {
                Name = "German",
                Key = "de"
            },
        };
    }
}
