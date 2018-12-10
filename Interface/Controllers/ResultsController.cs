using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Spider;

namespace Interface.Controllers
{
    public class ResultsController : Controller
    {
        public IActionResult List(string searchedContent)
        {
            IEnumerable<WebsiteInfo> results = new List<WebsiteInfo>()
            {
                new WebsiteInfo()
                {
                    CreateDate = DateTime.Now.AddDays(-3),
                    DescriptionMeta = "hahaha",
                    Title = "results 1",
                    Url = "xxx.com"
                }
            };

            ViewData["SearchedContent"] = searchedContent;

            return View(results);
        }
    }
}