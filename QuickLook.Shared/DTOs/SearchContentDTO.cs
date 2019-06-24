using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shared.DTOs
{
    public class SearchContentDTO
    {
        [JsonProperty(PropertyName = "searchedContent")]
        public string Input { get; set; }
        public string Language { get; set; }

        public bool IsAdvancedSearch { get; set; }
        public bool MatchExactWords { get; set; }
        public bool MatchExactSentence { get; set; }
        public bool MatchUncrawledWebsites { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int Fuzziness { get; set; }
        public int? Take { get; set; }
        public int? Page { get; set; }

        public SearchContentDTO()
        {
            Fuzziness = 1;
            StartDate = DateTime.Now.AddDays(-100);
            EndDate = DateTime.Now;
            Language = "en";
        }

        public SearchContentDTO(SearchContentDTO other, int? newPage)
        {
            this.Input = other.Input;
            this.Language = other.Language;

            this.IsAdvancedSearch = other.IsAdvancedSearch;
            this.MatchExactWords = other.MatchExactWords;
            this.MatchExactSentence = other.MatchExactSentence;
            this.MatchUncrawledWebsites = other.MatchUncrawledWebsites;

            this.Fuzziness = other.Fuzziness;
            this.StartDate = other.StartDate;
            this.EndDate = other.EndDate;

            this.Take = other.Take;
            this.Page = newPage;
        }

        public override string ToString()
        {
            return Input;
        }
    }
}
