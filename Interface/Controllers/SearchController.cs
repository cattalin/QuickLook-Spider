using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            return RedirectToAction("List", "Results", searchedContent.Input);
        }
    }
}