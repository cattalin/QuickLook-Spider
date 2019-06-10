using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ElasticsearchService.OutputManagers;
using Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using Shared.Mappers;

namespace QuickLook.Web.Controllers
{
    [Route("api/search-rest")]
    [ApiController]
    public class SearchRestController : ControllerBase
    {
        public IActionResult Search()
        {
            return Ok(new SearchContentDTO());
        }

        [HttpPost("advanced")]
        public IActionResult AdvancedResults(SearchContentDTO searchedContent)
        {
            //ViewData["SearchedContent"] = searchedContent;

            SearchPagination pagination = CreatePagination(searchedContent);

            ESReadWebsitesManager client = new ESReadWebsitesManager();
            var searchResult = client
                .FullTextSearchAdvanced(searchedContent, pagination)
                .ToDto(pagination, searchedContent);

            if (searchResult.SearchHits.Count == 0)
            {
                return NotFound();
            }

            return Ok(searchResult);
        }

        [HttpGet("")]
        public IActionResult Results(string searchedContent, int take, int page)
        {
            var searchedContentDto = new SearchContentDTO()
            {
                Input = searchedContent
            };

            //ViewData["SearchedContent"] = searchedContentDto;

            SearchPagination pagination = CreatePagination(take, page);

            ESReadWebsitesManager client = new ESReadWebsitesManager();
            var searchResult = client
                .FullTextSearch(searchedContent, pagination)
                .ToDto(pagination, searchedContentDto);

            if (searchResult.SearchHits.Count == 0)
            {
                return NotFound();
            }

            return Ok(searchResult);
        }

        protected SearchPagination CreatePagination(int take, int page)
        {
            SearchPagination pagination = new SearchPagination()
            {
                Take = 10,
                Page = 1,
                From = 0
            };

            if (take != 0 && page != 0)
            {
                pagination = new SearchPagination()
                {
                    Take = take,
                    Page = page,
                    From = take * (page - 1)
                };
            }

            return pagination;
        }

        protected SearchPagination CreatePagination(SearchContentDTO searchedContent)
        {
            SearchPagination pagination = new SearchPagination()
            {
                Take = 10,
                Page = 1,
                From = 0
            };

            var take = searchedContent.Take ?? 10;
            var page = searchedContent.Page ?? 1;

            if (take != 0 && page != 0)
            {
                pagination = new SearchPagination()
                {
                    Take = take,
                    Page = page,
                    From = take * (page - 1)
                };
            }

            return pagination;
        }
    }
}
