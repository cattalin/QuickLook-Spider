using System;
using System.Collections.Generic;
using System.Text;
using Shared.DTOs;

namespace Shared.DTOs
{
    public class SearchMetadataDto
    {
        public long Took { get; set; }
        public long Total { get; set; }
        public SearchContentDTO SearchedContent { get; set; }
    }
}
