using Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuickLook.Shared.Mappers
{
    public static class PendingWebsiteMapper
    {
        public static List<PendingWebsite> ToPendingWebsites(this List<string> relatedWebsiteUrls)
        {
            var pendingWebsites = new List<PendingWebsite>();
            relatedWebsiteUrls.ForEach(w =>
            {
                pendingWebsites.Add(new PendingWebsite()
                {
                    CreateDate = DateTime.Now,
                    Id = w,
                    Url = w
                });
            });

            return pendingWebsites;
        }
    }
}
