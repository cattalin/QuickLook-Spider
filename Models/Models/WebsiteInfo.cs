using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Models
{
    public class WebsiteInfo
    {
        public string Id { get; set; }

        public string Url { get; set; }

        public string Title { get; set; }

        public string DescriptionMeta { get; set; }

        public List<string> Paragraphs { get; set; }

        public string FullPageContent { get; set; }

        public string Language { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }
    }
}
