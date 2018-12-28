using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElasticsearchService.OutputManagers;
using Interface.Models;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace Interface.Controllers
{
    public class ResultsController : Controller
    {
        public IActionResult List(string searchedContent)
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