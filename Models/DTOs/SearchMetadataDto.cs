using System;
using System.Collections.Generic;
using System.Text;
using Interface.Models;

namespace Shared.DTOs
{
    public class SearchMetadataDto
    {
        public long Took { get; set; }
        public long Total { get; set; }
        public SearchContentDTO SearchedContent { get; set; }
    }
}
