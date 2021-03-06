﻿using System;
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
    public class SearchController : BaseController
    {
        public IActionResult Search()
        {
            return View(new SearchContentDTO());
        }

        [HttpPost]
        public IActionResult Search(SearchContentDTO searchedContent)
        {
            if (!searchedContent.IsAdvancedSearch)
            {
                return RedirectToAction("Results", new { searchedContent = searchedContent.Input });
            }
            else
            {
                return RedirectToAction("AdvancedResults", searchedContent);
            }
        }

        [Route("AdvancedResults")]
        public IActionResult AdvancedResults(SearchContentDTO searchedContent)
        {
            ViewData["SearchedContent"] = searchedContent;

            SearchPagination pagination = CreatePagination(searchedContent);

            ESReadWebsitesManager client = new ESReadWebsitesManager();
            var searchResult = client
                .FullTextSearchAdvanced(searchedContent, pagination)
                .ToDto(pagination, searchedContent);

            if (searchResult.SearchHits.Count == 0)
            {
                return View("NotFound");
            }

            return View("Results", searchResult);
        }

        [Route("Results")]
        public IActionResult Results(string searchedContent, int take, int page)
        {
            var searchedContentDto = new SearchContentDTO()
            {
                Input = searchedContent
            };

            ViewData["SearchedContent"] = searchedContentDto;

            SearchPagination pagination = CreatePagination(take, page);

            ESReadWebsitesManager client = new ESReadWebsitesManager();
            var searchResult = client
                .FullTextSearch(searchedContent, pagination)
                .ToDto(pagination, searchedContentDto);

            if (searchResult.SearchHits.Count == 0)
            {
                return View("NotFound");
            }

            return View(searchResult);
        }
    }
}