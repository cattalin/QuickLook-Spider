using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickLook.RelationalDbService.Entities;
using QuickLook.Shared.Mappers;
using QuickLook.WebApi.Mappers;
using Shared.DTOs;
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
        public IActionResult GetBookmarkList(int take = 10, int page = 1)
        {
            var userId = Request.HttpContext.User.Claims.First(c => c.Type == "UserId").Value;

            var context = new ApplicationDbContext();
            var results = context.Bookmarks
                .Skip((page-1)*take)
                .Take(take)
                .Where(b => b.UserId == userId)
                .ToList()
                .ToDtos();

            return Ok(results);
        }

        [HttpPost]
        [Authorize]
        public IActionResult CreateBookmark(WebsiteInfoDto payload)
        {
            var userId = Request.HttpContext.User.Claims.First(c => c.Type == "UserId").Value;

            var newBookmark = payload.ToEntity(userId);

            var context = new ApplicationDbContext();
            context.Bookmarks.Add(newBookmark);
            context.SaveChanges();

            return Ok(new { Status = "Success" });
        }
    }
}
