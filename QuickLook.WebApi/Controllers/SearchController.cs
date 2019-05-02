using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ElasticsearchService.OutputManagers;
using Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace QuickLook.WebApi.Controllers
{
    [Route("api/search")]
    [ApiController]
    public class SearchController : BaseController
    {
        [HttpPost("advanced")]
        public IActionResult AdvancedResults(SearchContentDTO searchedContent)
        {
            Pagination pagination = CreatePagination(searchedContent);

            ESOutputManager client = new ESOutputManager();
            var searchResult = client
                .FullTextSearchAdvanced(searchedContent, pagination)
                .ToDto(pagination, searchedContent);

            if (searchResult.Hits.Count == 0)
            {
                return NotFound();
            }

            return Ok(searchResult);
        }

        [HttpGet("simple")]
        [ProducesResponseType(200)]
        public IActionResult Results([FromQuery] string searchedContent, [FromQuery] int take, [FromQuery] int page)
        {
            Pagination pagination = CreatePagination(take, page);

            var searchedContentDto = new SearchContentDTO()
            {
                Input = searchedContent,
                Take = pagination.Take,
                Page = pagination.Page,
            };

            ESOutputManager client = new ESOutputManager();
            var searchResult = client
                .FullTextSearch(searchedContent, pagination)
                .ToDto(pagination, searchedContentDto);

            if (searchResult.Hits.Count == 0)
            {
                return NotFound();
            }

            return Ok(searchResult);
        }
    }
}
