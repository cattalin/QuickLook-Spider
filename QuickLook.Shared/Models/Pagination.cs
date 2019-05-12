using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Models
{
    public class SearchPagination
    {
        public int Take { get; set; }
        public int Page { get; set; }
        public int From { get; set; }
    }
}
