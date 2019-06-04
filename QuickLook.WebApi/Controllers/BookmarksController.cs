using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickLook.WebApi.Controllers
{

    [Route("api/bookmarks")]
    [ApiController]
    public class BookmarksController: BaseController
    {
        [HttpGet]
        [Authorize]
        public IActionResult GetBookmarkList()
        {
            var userId = Request.HttpContext.User.Claims.First(c => c.Type == "UserId").Value;


            return Ok(userId);
        }

        [HttpPost]
        [Authorize]
        public IActionResult CreateBookmark(WebsiteInfo payload)
        {
            var userId = Request.HttpContext.User.Claims.First(c => c.Type == "UserId").Value;


            return Ok(new { Status = "Success" });
        }
    }
}
