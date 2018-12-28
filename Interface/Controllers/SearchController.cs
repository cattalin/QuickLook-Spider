using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElasticsearchService.OutputManagers;
using Interface.Models;
using Microsoft.AspNetCore.Mvc;

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
            return RedirectToAction("Results", new { searchedContent = searchedContent.Input});
        }

        public IActionResult Results(string searchedContent)
        {
            ViewData["SearchedContent"] = new SearchContentDTO
            {
                Input = searchedContent
            };

            NestClient client = new NestClient();
            var results = client.FullTextSearch(searchedContent);

            return View(results);
        }
    }
}