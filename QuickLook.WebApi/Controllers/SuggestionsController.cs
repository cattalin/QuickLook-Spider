using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElasticsearchService.OutputManagers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;

namespace QuickLook.WebApi.Controllers
{
    [Route("api/suggestions")]
    [ApiController]
    public class SuggestionsController : BaseController
    {
        [HttpGet]
        public IActionResult Suggestions(string searchedContent)
        {
            ESOutputManager client = new ESOutputManager();
            var searchResult = client
                .FullTextSearchAdvanced(searchedContent, pagination)
                .ToDto(pagination, searchedContent);

            if (searchResult.SearchHits.Count == 0)
            {
                return NotFound();
            }

            return Ok(searchResult);
        }
    }
}