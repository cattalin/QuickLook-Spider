using System;
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
    public class SearchController : Controller
    {
        public IActionResult Search()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Search(SearchContentDTO searchedContent)
        {
            return RedirectToAction("Results", new { searchedContent = searchedContent.Input });
        }

        public IActionResult Results(string searchedContent, int take, int page)
        {
            ViewData["SearchedContent"] = new SearchContentDTO
            {
                Input = searchedContent
            };

            Pagination pagination = CreatePagination(take, page);

            NestClient client = new NestClient();
            var searchResult = client
                .FullTextSearch(searchedContent, pagination)
                .ToDto(pagination, searchedContent);

            if (searchResult.Hits.Count == 0)
            {
                return View("NotFound");
            }

            return View(searchResult);
        }

        [AllowAnonymous]
        public IActionResult Error()
        {
            return View();
        }

        private Pagination CreatePagination(int take, int page)
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
    }
}