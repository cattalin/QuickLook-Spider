using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shared.DTOs;
using Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace QuickLook.Web.Controllers
{
    public class BaseController : Controller
    {
        [AllowAnonymous]
        public IActionResult Error()
        {
            return View();
        }

        protected SearchPagination CreatePagination(int take, int page)
        {
            SearchPagination pagination = new SearchPagination()
            {
                Take = take >= 0 ? take : 10,
                Page = page > 0 ? page : 1,
                From = take != 0 && page != 0 ? take * (page - 1) : 0
            };

            return pagination;
        }

        protected SearchPagination CreatePagination(SearchContentDTO searchedContent)
        {
            var take = searchedContent.Take ?? 10;
            var page = searchedContent.Page ?? 1;

            return CreatePagination(take, page);
        }
    }
}
