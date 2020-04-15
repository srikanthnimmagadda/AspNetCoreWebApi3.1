using CourseLibrary.Api.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.Api.Controllers
{
    [Route("api")]
    [ApiController]
    public class RootController : ControllerBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet(Name = "GetRoot")]
        public IActionResult GetRoot()
        {
            // create links for root
            var links = new List<Link>
            {
                new Link(Url.Link("GetRoot", new { }), "self", "GET"),
                new Link(Url.Link("GetAuthors", new { }), "authors", "GET"),
                new Link(Url.Link("CreateAuthor", new { }), "create_author", "POST")
            };

            return Ok(links);
        }
    }
}
