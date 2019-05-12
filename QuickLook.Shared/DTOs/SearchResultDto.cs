using Nest;
using System;
using System.Collections.Generic;
using System.Text;
using Shared.Models;
using System.Linq;

namespace Shared.DTOs
{
    public class SearchResultDto
    {
        public List<WebsiteInfoDto> SearchHits { get; set; }

        public SearchMetadataDto SearchMetadata { get; set; }

        public SearchPagination SearchPagination { get; set; }
    }
}
