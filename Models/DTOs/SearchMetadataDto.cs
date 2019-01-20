using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs
{
    public class SearchMetadataDto
    {
        public long Took { get; set; }
        public long Total { get; set; }
        public string SearchedContent { get; set; }
    }
}
