using AutoMapper;
using CourseLibrary.Api.Services.Interfaces;
using CourseLibrary.Api.Utility;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CourseLibrary.Api.Controllers
{
    [ApiController]
    [Route("api/authorcollections")]
    public class AuthorCollectionsController : ControllerBase
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly ICourseLibraryRepository _repository;

        /// <summary>
        /// 
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="courseLibraryRepository"></param>
        /// <param name="mapper"></param>
        public AuthorCollectionsController(ICourseLibraryRepository courseLibraryRepository, IMapper mapper)
        {
            _repository = courseLibraryRepository ?? throw new ArgumentNullException(nameof(courseLibraryRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpGet("({ids})", Name = "GetAuthorCollection")]
        public IActionResult GetAuthorCollection(
        [FromRoute]
        [ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
        {
            if (ids == null)
            {
                return BadRequest();
            }

            var authorEntities = _repository.GetAuthors(ids);

            if (ids.Count() != authorEntities.Count())
            {
                return NotFound();
            }

            var authorsToReturn = _mapper.Map<IEnumerable<Dto.Author>>(authorEntities);

            return Ok(authorsToReturn);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authorCollection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<IEnumerable<Dto.Author>> CreateAuthorCollection(
            IEnumerable<Models.Author.Create> authorCollection)
        {
            var authorEntities = _mapper.Map<IEnumerable<Entities.Author>>(authorCollection);

            foreach (var author in authorEntities)
            {
                _repository.AddAuthor(author);
            }

            _repository.Save();

            var authorCollectionToReturn = _mapper.Map<IEnumerable<Dto.Author>>(authorEntities);
            var idsAsString = string.Join(",", authorCollectionToReturn.Select(a => a.Id));
            
            return CreatedAtRoute("GetAuthorCollection",
             new { ids = idsAsString },
             authorCollectionToReturn);
        }
    }
}

