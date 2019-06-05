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
        [HttpGet("edge-ngram")]
        public IActionResult EdgeNGram(string searchedContent)
        {
            ESReadSuggestionsManager client = new ESReadSuggestionsManager();
            var searchResult = client
                .EdgeNGramSearch(searchedContent);

            return Ok(searchResult.Documents);
        }

        [HttpGet("completion")]
        public IActionResult CompletionSuggester(string searchedContent)
        {
            ESReadSuggestionsManager client = new ESReadSuggestionsManager();
            var searchResult = client
                .CompletionSuggesterSearch(searchedContent);

            var results = searchResult.Suggest.FirstOrDefault().Value.FirstOrDefault().Options?.Select(s => new { Suggestion = s.Text });
            return Ok(results);
        }

        [HttpPost]
        public IActionResult AddSuggestion(string content)
        {
            ESReadWebsitesManager client = new ESReadWebsitesManager();
            
            return Ok();
        }
    }
}