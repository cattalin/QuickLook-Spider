using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickLook.WebApi.Controllers
{
    public class BaseController : ControllerBase
    {
        protected Pagination CreatePagination(int take, int page)
        {
            Pagination pagination = new Pagination()
            {
                Take = take >= 0 ? take : 10,
                Page = page > 0 ? page : 1,
                From = take != 0 && page != 0 ? take * (page - 1) : 0
            };

            return pagination;
        }

        protected Pagination CreatePagination(SearchContentDTO searchedContent)
        {
            var take = searchedContent.Take ?? 10;
            var page = searchedContent.Page ?? 1;

            return CreatePagination(take, page);
        }
    }
}
