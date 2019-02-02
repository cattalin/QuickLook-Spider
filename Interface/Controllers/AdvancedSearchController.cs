using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElasticsearchService.OutputManagers;
using Interface.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using Shared.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Interface.Controllers
{
    [Route("advancedSearch")]
    public class AdvancedSearchController : BaseController
    {
        public IActionResult Search()
        {
            return View();
        }

//        [HttpPost]
        [Route("results")]
        public IActionResult Results(SearchContentDTO searchedContent)
        {
            ViewData["SearchedContent"] = new SearchContentDTO
            {
                Input = searchedContent.Input
            };

            Pagination pagination = CreatePagination(searchedContent);

            ESOutputManager client = new ESOutputManager();
            var searchResult = client
                .FullTextSearch(searchedContent.Input, pagination)
                .ToDto(pagination, searchedContent.Input);

            if (searchResult.Hits.Count == 0)
            {
                return View("NotFound");
            }

            return View(searchResult);
        }
    }
}
