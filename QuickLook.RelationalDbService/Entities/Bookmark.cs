using System;
using System.Collections.Generic;
using System.Text;

namespace QuickLook.RelationalDbService.Entities
{
    public class Bookmark
    {
        public string Id { get; set; }

        public string UserId { get; set; }

        public string ElasticsearchId { get; set; }

        public string Url { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public string Language { get; set; }

        public DateTime CreateDate { get; set; }

        public virtual User User { get; set; }
    }
}
