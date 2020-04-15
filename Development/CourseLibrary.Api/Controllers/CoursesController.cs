using System;
using System.Collections.Generic;
using AutoMapper;
using CourseLibrary.Api.Models.Course;
using CourseLibrary.Api.Services.Interfaces;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CourseLibrary.Api.Controllers
{
    [Route("api/authors/{authorId}/courses")]
    [ApiController]
    //[ResponseCache(CacheProfileName = "240SecondsCacheProfile")]
    [HttpCacheExpiration(CacheLocation = CacheLocation.Public)]
    [HttpCacheValidation(MustRevalidate = true)]
    public class CoursesController : ControllerBase
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly ICourseLibraryRepository _courseRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="courseRepository"></param>
        public CoursesController(ICourseLibraryRepository courseRepository, IMapper mapper)
        {
            _courseRepository = courseRepository ?? throw new ArgumentNullException(nameof(courseRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(courseRepository));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authorId"></param>
        /// <returns></returns>
        [HttpGet(Name = "GetCoursesForAuthor")]
        public ActionResult<IEnumerable<Dto.Course>> GetAuthorCourses(Guid authorId)
        {
            if (!_courseRepository.IsAuthorExists(authorId))
            {
                return NotFound();
            }

            var courses = _courseRepository.GetCourses(authorId);
            return Ok(_mapper.Map<IEnumerable<Dto.Course>>(courses));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authorId"></param>
        /// <param name="courseId"></param>
        /// <returns></returns>
        [HttpGet("{courseId}", Name = "GetCourseForAuthor")]
        //[ResponseCache(Duration = 120)]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 1000)]
        [HttpCacheValidation(MustRevalidate = false)]
        public ActionResult<Dto.Course> GetAuthorCourse(Guid authorId, Guid courseId)
        {
            if (!_courseRepository.IsAuthorExists(authorId))
            {
                return NotFound();
            }

            var course = _courseRepository.GetCourse(authorId, courseId);

            if (course == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<Dto.Course>(course));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authorId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost(Name = "Create")]
        public ActionResult<Dto.Course> Create(Guid authorId, Create model)
        {
            if (!_courseRepository.IsAuthorExists(authorId))
            {
                return NotFound();
            }

            var entity = _mapper.Map<Entities.Course>(model);
            _courseRepository.AddCourse(authorId, entity);
            _courseRepository.Save();

            var courseToReturn = _mapper.Map<Dto.Course>(entity);
            return CreatedAtRoute("GetCourseForAuthor",
                new { authorId = authorId, courseId = courseToReturn.Id },
                courseToReturn);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authorId"></param>
        /// <param name="courseId"></param>
        /// <returns></returns>
        [HttpDelete("{courseId}")]
        public ActionResult Delete(Guid authorId, Guid courseId)
        {
            if (!_courseRepository.IsAuthorExists(authorId))
            {
                return NotFound();
            }

            var courseForAuthorFromRepo = _courseRepository.GetCourse(authorId, courseId);
            if (courseForAuthorFromRepo == null)
            {
                return NotFound();
            }

            _courseRepository.DeleteCourse(courseForAuthorFromRepo);
            _courseRepository.Save();
            return NoContent();
        }

        [HttpPut("{courseId}")]
        public IActionResult Update(Guid authorId, Guid courseId, Update model)
        {
            if (!_courseRepository.IsAuthorExists(authorId))
            {
                return NotFound();
            }

            var course = _courseRepository.GetCourse(authorId, courseId);
            if (course == null)
            {
                var courseToAdd = _mapper.Map<Entities.Course>(model);
                courseToAdd.Id = courseId;

                _courseRepository.AddCourse(authorId, courseToAdd);
                _courseRepository.Save();

                var courseToReturn = _mapper.Map<Dto.Course>(courseToAdd);
                return CreatedAtRoute("GetCourseForAuthor",
                    new { authorId, courseId = courseToReturn.Id },
                    courseToReturn);
            }

            // map the entity to a CourseForUpdateDto
            // apply the updated field values to that dto
            // map the CourseForUpdateDto back to an entity
            _mapper.Map(model, course);
            _courseRepository.UpdateCourse(course);
            _courseRepository.Save();

            return NoContent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authorId"></param>
        /// <param name="courseId"></param>
        /// <param name="patchDocument"></param>
        /// <returns></returns>
        [HttpPatch("{courseId}")]
        public ActionResult PartiallyUpdate(Guid authorId, Guid courseId, JsonPatchDocument<Update> patchDocument)
        {
            if (!_courseRepository.IsAuthorExists(authorId))
            {
                return NotFound();
            }

            var course = _courseRepository.GetCourse(authorId, courseId);

            if (course == null)
            {
                var courseDto = new Update();
                patchDocument.ApplyTo(courseDto, ModelState);

                if (!TryValidateModel(courseDto))
                {
                    return ValidationProblem(ModelState);
                }

                var courseToAdd = _mapper.Map<Entities.Course>(courseDto);
                courseToAdd.Id = courseId;

                _courseRepository.AddCourse(authorId, courseToAdd);
                _courseRepository.Save();

                var courseToReturn = _mapper.Map<Dto.Course>(courseToAdd);

                return CreatedAtRoute("GetCourseForAuthor",
                    new { authorId, courseId = courseToReturn.Id },
                    courseToReturn);
            }

            var courseToPatch = _mapper.Map<Update>(course);
            // add validation
            patchDocument.ApplyTo(courseToPatch, ModelState);

            if (!TryValidateModel(courseToPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(courseToPatch, course);

            _courseRepository.UpdateCourse(course);
            _courseRepository.Save();

            return NoContent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelStateDictionary"></param>
        /// <returns></returns>
        public override ActionResult ValidationProblem([ActionResultObjectValue] ModelStateDictionary modelStateDictionary)
        {
            var options = HttpContext.RequestServices
                .GetRequiredService<IOptions<ApiBehaviorOptions>>();
            return (ActionResult)options.Value.InvalidModelStateResponseFactory(ControllerContext);
        }
    }
}