using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Interface.Models
{
    public class SearchContentDTO
    {
        public string Input { get; set; }
        public string Language { get; set; }

        public bool IsAdvancedSearch { get; set; }
        public bool MatchExactWords { get; set; }
        public bool MatchExactContent { get; set; }
        public bool MatchUncrawledWebsites { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int Fuzziness { get; set; }

        public override string ToString()
        {
            return Input;
        }
    }
}
