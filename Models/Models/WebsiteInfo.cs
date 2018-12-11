using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Models
{
    public class WebsiteInfo
    {
        public string Url { get; set; }

        public string Title { get; set; }

        public string DescriptionMeta { get; set; }

        public DateTime CreateDate { get; set; }
    }
}
