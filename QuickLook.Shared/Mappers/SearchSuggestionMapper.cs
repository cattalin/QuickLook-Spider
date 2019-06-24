using QuickLook.Shared.Models;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace QuickLook.Shared.Mappers
{
    public static class SearchSuggestionMapper
    {
        public static List<SearchSuggestion> ExtractFromCrawledData(this WebsiteInfo websiteInfo)
        {
            var suggestions = new List<SearchSuggestion>();

            suggestions.Add(new SearchSuggestion
            {
                Text = websiteInfo.Title
            });

            websiteInfo.Headers.ForEach(header =>
            {
                suggestions.Add(new SearchSuggestion
                {
                    Text = header.Take(30).ToArray().ToString()
                });
            });

            return suggestions;
        }

        public static List<string> ExtractFromCrawledDataAsStrings(this WebsiteInfo websiteInfo)
        {
            var suggestions = new List<string>();

            suggestions.Add(websiteInfo.Title);

            websiteInfo.Headers?.ForEach(header =>
            {
                suggestions.Add(String.Concat(header.Take(60)));
            });

            var sanitizedSugestions = suggestions
                .Select(s => SanitizeSuggestion(s))
                .Where(s => s != "").ToList();

            return sanitizedSugestions;
        }

        public static string SanitizeSuggestion(string text)
        {
            var step1SanitizationText = text.Replace("[Edit]", "");
            Regex reg = new Regex("[^a-zA-Z0-9' -,.&$]");                           //remove strange characters
            var sanitizedText = reg.Replace(step1SanitizationText, string.Empty);

            return Regex.Replace(sanitizedText, @"\s+", " ");                    //remove newlines tabs and whitespaces
        }
    }
}
