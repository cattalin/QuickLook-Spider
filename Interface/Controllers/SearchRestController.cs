﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ElasticsearchService.OutputManagers;
using Interface.Models;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using Shared.Models;

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

        [HttpGet("")]
        public IActionResult Results(string searchedContent, int take, int page)
        {
            var searchedContentDto = new SearchContentDTO()
            {
                Input = searchedContent
            };

            //ViewData["SearchedContent"] = searchedContentDto;

            Pagination pagination = CreatePagination(take, page);

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

        protected Pagination CreatePagination(int take, int page)
        {
            Pagination pagination = new Pagination()
            {
                Take = 10,
                Page = 1,
                From = 0
            };

            if (take != 0 && page != 0)
            {
                pagination = new Pagination()
                {
                    Take = take,
                    Page = page,
                    From = take * (page - 1)
                };
            }

            return pagination;
        }

        protected Pagination CreatePagination(SearchContentDTO searchedContent)
        {
            Pagination pagination = new Pagination()
            {
                Take = 10,
                Page = 1,
                From = 0
            };

            var take = searchedContent.Take ?? 10;
            var page = searchedContent.Page ?? 1;

            if (take != 0 && page != 0)
            {
                pagination = new Pagination()
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