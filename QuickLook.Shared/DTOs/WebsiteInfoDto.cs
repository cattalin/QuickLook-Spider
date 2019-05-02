using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs
{
    public class WebsiteInfoDto
    {
        public string Url { get; set; }

        public string Title { get; set; }

        public string DescriptionMeta { get; set; }

        public List<string> Paragraphs { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public List<string> Highlights { get; set; }
    }
}
