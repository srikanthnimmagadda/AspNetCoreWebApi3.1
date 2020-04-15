using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using AutoMapper;
using CourseLibrary.Api.Attributes;
using CourseLibrary.Api.Dto;
using CourseLibrary.Api.Extensions;
using CourseLibrary.Api.Models.Author;
using CourseLibrary.Api.Services.Interfaces;
using CourseLibrary.Api.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace CourseLibrary.Api.Controllers
{
    [Route("api/authors")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly ICourseLibraryRepository _courseRepository;

        /// <summary>
        /// 
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// 
        /// </summary>
        private readonly IPropertyMappingService _propertyMappingService;

        /// <summary>
        /// 
        /// </summary>
        private readonly IPropertyCheckerService _propertyCheckerService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="courseRepository"></param>
        public AuthorsController(ICourseLibraryRepository courseRepository, IMapper mapper,
            IPropertyMappingService propertyMappingService, IPropertyCheckerService propertyCheckerService)
        {
            _courseRepository = courseRepository ?? throw new ArgumentNullException(nameof(courseRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(courseRepository));
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
            _propertyCheckerService = propertyCheckerService ?? throw new ArgumentNullException(nameof(propertyCheckerService));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        //[HttpGet]
        //[HttpHead]
        //public ActionResult<IEnumerable<Dto.Author>> GetAuthors()
        //{
        //    var authors = _courseRepository.GetAuthors();
        //    return Ok(_mapper.Map<IEnumerable<Dto.Author>>(authors));
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        //[HttpGet]
        //public ActionResult<IEnumerable<Dto.Author>> GetAuthors(string mainCategory, string searchQuery)
        //{
        //    var authors = _courseRepository.GetAuthors(mainCategory, searchQuery);
        //    return Ok(_mapper.Map<IEnumerable<Dto.Author>>(authors));
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet(Name = "GetAuthors")]
        public IActionResult GetAuthors([FromQuery] AuthorsResourceParameters authorsResourceParameters)
        {
            if (!_propertyMappingService.HasValidMappingExistsFor<Author, Entities.Author>
                (authorsResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            if (!_propertyCheckerService.IsTypeHasProperties<Author>
                (authorsResourceParameters.Fields))
            {
                return BadRequest();
            }

            var authors = _courseRepository.GetAuthors(authorsResourceParameters);

            var previousPageLink = authors.HasPrevious ?
                CreateAuthorsResourceUri(authorsResourceParameters,
                ResourceUriType.PreviousPage) : null;

            var nextPageLink = authors.HasNext ?
                CreateAuthorsResourceUri(authorsResourceParameters,
                ResourceUriType.NextPage) : null;

            var paginationMetadata = new
            {
                totalCount = authors.TotalCount,
                pageSize = authors.PageSize,
                currentPage = authors.CurrentPage,
                totalPages = authors.TotalPages,
                previousPageLink,
                nextPageLink
            };

            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(paginationMetadata));

            var links = CreateLinksForAuthors(authorsResourceParameters, authors.HasNext, authors.HasPrevious);
            var shapedAuthors = _mapper.Map<IEnumerable<Author>>(authors).ShapeData(authorsResourceParameters.Fields);
            var shapedAuthorsWithLinks = shapedAuthors.Select(author =>
            {
                var authorDictionary = author as IDictionary<string, object>;
                var authorLinks = CreateLinksForAuthor((Guid)authorDictionary["Id"], null);
                authorDictionary.Add("links", authorLinks);
                return authorDictionary;
            });

            var resourceToReturn = new
            {
                value = shapedAuthorsWithLinks,
                links
            };

            return Ok(resourceToReturn);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authorId"></param>
        /// <returns></returns>
        [Produces("application/json",
            "application/vnd.marvin.hateoas+json",
            "application/vnd.marvin.author.full+json",
            "application/vnd.marvin.author.full.hateoas+json",
            "application/vnd.marvin.author.friendly+json",
            "application/vnd.marvin.author.friendly.hateoas+json")]
        [HttpGet("{authorId}", Name = "GetAuthor")]
        public ActionResult<Dto.Author> GetAuthor(Guid authorId, string fields, [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue parsedMediaType))
            {
                return BadRequest();
            }

            if (!_propertyCheckerService.IsTypeHasProperties<Author>(fields))
            {
                return BadRequest();
            }

            var author = _courseRepository.GetAuthor(authorId);
            if (author == null)
            {
                return NotFound();
            }

            var hasToIncludeLinks = parsedMediaType.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);

            IEnumerable<Link> links = new List<Link>();
            if (hasToIncludeLinks)
            {
                links = CreateLinksForAuthor(authorId, fields);
            }

            var primaryMediaType = hasToIncludeLinks ?
                parsedMediaType.SubTypeWithoutSuffix.Substring(0, parsedMediaType.SubTypeWithoutSuffix.Length - 8)
                : parsedMediaType.SubTypeWithoutSuffix;

            //full author
            if (primaryMediaType == "vnd.marvin.author.full")
            {
                var fullResourceToReturn = _mapper.Map<AuthorMain>(author)
                   .ShapeData(fields) as IDictionary<string, object>;

                if (hasToIncludeLinks)
                {
                    fullResourceToReturn.Add("links", links);
                }

                return Ok(fullResourceToReturn);
            }

            // friendly author
            var friendlyResourceToReturn = _mapper.Map<Author>(author)
                .ShapeData(fields) as IDictionary<string, object>;

            if (hasToIncludeLinks)
            {
                friendlyResourceToReturn.Add("links", links);
            }

            return Ok(friendlyResourceToReturn);

            //if (parsedMediaType.MediaType == "application/vnd.marvin.hateoas+json")
            //{
            //    var links = CreateLinksForAuthor(authorId, fields);
            //    var resourseToReturn = _mapper.Map<Dto.Author>(author).ShapeData(fields) as IDictionary<string, object>;
            //    resourseToReturn.Add("links", links);
            //    return Ok(resourseToReturn);
            //}

            //return Ok(_mapper.Map<Dto.Author>(author).ShapeData(fields));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="author"></param>
        /// <returns></returns>
        [HttpPost(Name = "CreateAuthorWithDateOfDeath")]
        [RequestHeaderMatchesMediaType("Content-Type",
            "application/vnd.marvin.authorforcreationwithdateofdeath+json")]
        [Consumes("application/vnd.marvin.authorforcreationwithdateofdeath+json")]
        public IActionResult CreateAuthorWithDateOfDeath(CreateWithDateOfDeath author)
        {
            var authorEntity = _mapper.Map<Entities.Author>(author);
            _courseRepository.AddAuthor(authorEntity);
            _courseRepository.Save();

            var authorToReturn = _mapper.Map<Author>(authorEntity);
            var links = CreateLinksForAuthor(authorToReturn.Id, null);
            var linkedResourceToReturn = authorToReturn.ShapeData(null) as IDictionary<string, object>;
            linkedResourceToReturn.Add("links", links);

            return CreatedAtRoute("GetAuthor", new { authorId = linkedResourceToReturn["Id"] }, linkedResourceToReturn);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost(Name = "CreateAuthor")]
        [RequestHeaderMatchesMediaType("Content-Type",
            "application/json",
            "application/vnd.marvin.authorforcreation+json")]
        [Consumes("application/json",
            "application/vnd.marvin.authorforcreation+json")]
        public ActionResult<Author> Create(Create model)
        {
            Entities.Author entity = _mapper.Map<Entities.Author>(model);
            _courseRepository.AddAuthor(entity);
            _courseRepository.Save();

            var authorToReturn = _mapper.Map<Author>(entity);
            var links = CreateLinksForAuthor(authorToReturn.Id, null);
            var resourseToReturn = authorToReturn.ShapeData(null) as IDictionary<string, object>;
            resourseToReturn.Add("links", links);

            return CreatedAtRoute("GetAuthor", new { authorId = resourseToReturn["Id"] }, resourseToReturn);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpOptions]
        public IActionResult GetAuthorsOptions()
        {
            Response.Headers.Add("Allow", "GET,OPTIONS,POST");
            return Ok();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authorId"></param>
        /// <returns></returns>
        [HttpDelete("{authorId}", Name = "Delete")]
        public ActionResult Delete(Guid authorId)
        {
            var author = _courseRepository.GetAuthor(authorId);

            if (author == null)
            {
                return NotFound();
            }

            _courseRepository.DeleteAuthor(author);
            _courseRepository.Save();

            return NoContent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authorsResourceParameters"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private string CreateAuthorsResourceUri(AuthorsResourceParameters authorsResourceParameters, ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetAuthors",
                      new
                      {
                          fields = authorsResourceParameters.Fields,
                          orderBy = authorsResourceParameters.OrderBy,
                          pageNumber = authorsResourceParameters.PageNumber - 1,
                          pageSize = authorsResourceParameters.PageSize,
                          mainCategory = authorsResourceParameters.MainCategory,
                          searchQuery = authorsResourceParameters.SearchQuery
                      });
                case ResourceUriType.NextPage:
                    return Url.Link("GetAuthors",
                      new
                      {
                          fields = authorsResourceParameters.Fields,
                          orderBy = authorsResourceParameters.OrderBy,
                          pageNumber = authorsResourceParameters.PageNumber + 1,
                          pageSize = authorsResourceParameters.PageSize,
                          mainCategory = authorsResourceParameters.MainCategory,
                          searchQuery = authorsResourceParameters.SearchQuery
                      });
                case ResourceUriType.Current:
                default:
                    return Url.Link("GetAuthors",
                    new
                    {
                        fields = authorsResourceParameters.Fields,
                        orderBy = authorsResourceParameters.OrderBy,
                        pageNumber = authorsResourceParameters.PageNumber,
                        pageSize = authorsResourceParameters.PageSize,
                        mainCategory = authorsResourceParameters.MainCategory,
                        searchQuery = authorsResourceParameters.SearchQuery
                    });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authorId"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        private IEnumerable<Link> CreateLinksForAuthor(Guid authorId, string fields)
        {
            var links = new List<Link>();
            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(new Link(Url.Link("GetAuthor", new { authorId }), "self", "GET"));
            }
            else
            {
                links.Add(new Link(Url.Link("GetAuthor", new { authorId, fields }), "self", "GET"));
            }

            links.Add(new Link(Url.Link("Delete", new { authorId }), "delete", "DELETE"));

            links.Add(new Link(Url.Link("Create", new { authorId }), "create", "POST"));

            links.Add(new Link(Url.Link("GetCoursesForAuthor", new { authorId }), "courses", "GET"));

            return links;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authorsResourceParameters"></param>
        /// <param name="hasNext"></param>
        /// <param name="hasPrevious"></param>
        /// <returns></returns>
        private IEnumerable<Link> CreateLinksForAuthors(AuthorsResourceParameters authorsResourceParameters, bool hasNext, bool hasPrevious)
        {
            var links = new List<Link>();

            // self 
            links.Add(new Link(CreateAuthorsResourceUri(authorsResourceParameters, ResourceUriType.Current), "self", "GET"));

            if (hasNext)
            {
                links.Add(
                  new Link(CreateAuthorsResourceUri(authorsResourceParameters, ResourceUriType.NextPage), "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                links.Add(new Link(CreateAuthorsResourceUri(authorsResourceParameters, ResourceUriType.PreviousPage), "previousPage", "GET"));
            }

            return links;
        }
    }
}