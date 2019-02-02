﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ElasticsearchService.OutputManagers;
using Interface.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using Shared.Models;

namespace Interface.Controllers
{
    public class SearchController : BaseController
    {
        public IActionResult Search()
        {
            return View();
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
            ViewData["SearchedContent"] = new SearchContentDTO
            {
                Input = searchedContent.Input
            };

            Pagination pagination = CreatePagination(searchedContent);

            ESOutputManager client = new ESOutputManager();
            var searchResult = client
                .FullTextSearchAdvanced(searchedContent, pagination)
                .ToDto(pagination, searchedContent.Input);

            if (searchResult.Hits.Count == 0)
            {
                return View("NotFound");
            }

            return View("Results", searchResult);
        }

        [Route("Results")]
        public IActionResult Results(string searchedContent, int take, int page)
        {
            ViewData["SearchedContent"] = new SearchContentDTO
            {
                Input = searchedContent
            };

            Pagination pagination = CreatePagination(take, page);

            ESOutputManager client = new ESOutputManager();
            var searchResult = client
                .FullTextSearch(searchedContent, pagination)
                .ToDto(pagination, searchedContent);

            if (searchResult.Hits.Count == 0)
            {
                return View("NotFound");
            }

            return View(searchResult);
        }
    }
}