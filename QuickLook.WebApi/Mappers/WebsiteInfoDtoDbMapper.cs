using QuickLook.RelationalDbService.Entities;
using Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickLook.WebApi.Mappers
{
    public static class WebsiteInfoDtoDbMapper
    {
        public static Bookmark ToEntity(this WebsiteInfoDto dto, string userId)
        {
            var entity = new Bookmark();

            entity.Id = Guid.NewGuid().ToString();
            entity.Language = dto.Language;
            entity.Title = dto.Title;
            entity.Url = dto.Url;
            entity.UserId = userId;
            entity.ElasticsearchId = dto.Id;
            entity.CreateDate = DateTime.Now;
            entity.Content = string.Join("\n", dto.Highlights);

            return entity;
        }
    }
}
