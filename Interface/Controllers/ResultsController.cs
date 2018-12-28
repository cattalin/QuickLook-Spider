using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElasticsearchService.OutputManagers;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace Interface.Controllers
{
    public class ResultsController : Controller
    {
//        [Route("results/list/{searchedContent}")]
        public IActionResult List(string searchedContent)
        {
            NestClient client = new NestClient();
            var results = client.FullTextSearch(searchedContent);

            ViewData["SearchedContent"] = searchedContent;

            return View(results);
        }
    }
}