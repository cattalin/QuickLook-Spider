using QuickLook.Shared.Models;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuickLook.Shared.Mappers
{
    public static class SearchSuggestionMapper
    {
        public static List<SearchSuggestion> ExtractFromCrawledData(this WebsiteInfo websiteInfo)
        {
            var suggestions = new List<SearchSuggestion>();

            suggestions.Add(new SearchSuggestion
            {
                Name = websiteInfo.Title
            });

            websiteInfo.Headers.ForEach(header =>
            {
                suggestions.Add(new SearchSuggestion
                {
                    Name = String.Concat(header.Take(60))
                });
            });

            return suggestions;
        }
        public static List<string> ExtractFromCrawledDataAsStrings(this WebsiteInfo websiteInfo)
        {
            var suggestions = new List<string>();

            suggestions.Add(websiteInfo.Title);

            websiteInfo.Headers.ForEach(header =>
            {
                suggestions.Add(String.Concat(header.Take(60)));
            });

            return suggestions;
        }
    }
}
