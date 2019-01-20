using Nest;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shared.DTOs
{
    public static class NestToDtoMapper
    {
        public static SearchResultDto ToDto(this ISearchResponse<WebsiteInfo> searchResponse, Pagination pagination, string searchedContent)
        {

            SearchResultDto result = new SearchResultDto
            {
                Hits = searchResponse.Hits.ToDtos(),
                SearchMetadata = new SearchMetadataDto
                {
                    Took = searchResponse.Took,
                    Total = searchResponse.Total,
                    SearchedContent = searchedContent
                },
                Pagination = pagination
            };

            return result;
        }

        public static List<WebsiteInfoDto> ToDtos(this IReadOnlyCollection<Nest.IHit<WebsiteInfo>> Hits)
        {
            var results = new List<WebsiteInfoDto>();

            foreach (var hit in Hits)
            {
                var newResult = new WebsiteInfoDto
                {
                    Title = hit.Source.Title,
                    Url = hit.Source.Url,
                    DescriptionMeta = hit.Source.DescriptionMeta,
                    CreateDate = hit.Source.CreateDate,
                    Paragraphs = hit.Source.Paragraphs,
                    Highlights = new List<string>()
                };

                if (hit.Highlights.Count > 0)
                {
                    foreach (var highlight in hit.Highlights?.First().Value.Highlights?.ToList())
                    {
                        newResult.Highlights.Add(highlight);
                    }
                }

                results.Add(newResult);
            }

            return results;
        }
    }
}
