using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace Interface.Controllers
{
    public class BaseController : Controller
    {
        [AllowAnonymous]
        public IActionResult Error()
        {
            return View();
        }

        protected Pagination CreatePagination(int take, int page)
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
