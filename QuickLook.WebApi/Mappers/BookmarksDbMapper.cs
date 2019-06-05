using QuickLook.RelationalDbService.Entities;
using Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuickLook.Shared.Mappers
{
    public static class BookmarksDbMapper
    {
        public static List<WebsiteInfoDto> ToDtos(this List<Bookmark> entity)
        {
            var results = entity.Select(e => e.ToDto()).ToList();

            return results;
        }

        public static WebsiteInfoDto ToDto(this Bookmark entity)
        {
            var result = new WebsiteInfoDto();

            result.Id = entity.ElasticsearchId;
            result.Language = entity.Language;
            result.Title = entity.Title;
            result.Url = entity.Url;
            result.CreateDate = DateTime.Now;
            result.Highlights = new List<string>() { entity.Content };

            return result;
        }
    }
}
