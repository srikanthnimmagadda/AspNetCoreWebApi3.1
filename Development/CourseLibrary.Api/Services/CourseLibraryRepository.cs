using CourseLibrary.Api.Data;
using CourseLibrary.Api.Dto;
using CourseLibrary.Api.Extensions;
using CourseLibrary.Api.Services.Interfaces;
using CourseLibrary.Api.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CourseLibrary.Api.Services
{
    public class CourseLibraryRepository : ICourseLibraryRepository, IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly CourseLibraryDbContext _context;

        /// <summary>
        /// 
        /// </summary>
        private readonly IPropertyMappingService _propertyMappingService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public CourseLibraryRepository(CourseLibraryDbContext context, IPropertyMappingService propertyMappingService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authorId"></param>
        /// <param name="course"></param>
        public void AddCourse(Guid authorId, Entities.Course course)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            if (course == null)
            {
                throw new ArgumentNullException(nameof(course));
            }
            // always set the AuthorId to the passed-in authorId
            course.AuthorId = authorId;
            _context.Courses.Add(course);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="course"></param>
        public void DeleteCourse(Entities.Course course)
        {
            _context.Courses.Remove(course);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authorId"></param>
        /// <param name="courseId"></param>
        /// <returns></returns>
        public Entities.Course GetCourse(Guid authorId, Guid courseId)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            if (courseId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(courseId));
            }

            return _context.Courses
              .Where(c => c.AuthorId == authorId && c.Id == courseId).FirstOrDefault();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authorId"></param>
        /// <returns></returns>
        public IEnumerable<Entities.Course> GetCourses(Guid authorId)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            return _context.Courses
                        .Where(c => c.AuthorId == authorId)
                        .OrderBy(c => c.Title).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="course"></param>
        public void UpdateCourse(Entities.Course course)
        {
            // no code in this implementation
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="author"></param>
        public void AddAuthor(Entities.Author author)
        {
            if (author == null)
            {
                throw new ArgumentNullException(nameof(author));
            }

            // the repository fills the id (instead of using identity columns)
            author.Id = Guid.NewGuid();

            foreach (var course in author.Courses)
            {
                course.Id = Guid.NewGuid();
            }

            _context.Authors.Add(author);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authorId"></param>
        /// <returns></returns>
        public bool IsAuthorExists(Guid authorId)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            return _context.Authors.Any(a => a.Id == authorId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="author"></param>
        public void DeleteAuthor(Entities.Author author)
        {
            if (author == null)
            {
                throw new ArgumentNullException(nameof(author));
            }

            _context.Authors.Remove(author);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authorId"></param>
        /// <returns></returns>
        public Entities.Author GetAuthor(Guid authorId)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            return _context.Authors.FirstOrDefault(a => a.Id == authorId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Entities.Author> GetAuthors()
        {
            return _context.Authors.ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mainCategory"></param>
        /// <param name="searchQuery"></param>
        /// <returns></returns>
        public IEnumerable<Entities.Author> GetAuthors(string mainCategory, string searchQuery)
        {
            if (string.IsNullOrEmpty(mainCategory) && string.IsNullOrEmpty(searchQuery))
            {
                return GetAuthors();
            }

            var collection = _context.Authors as IQueryable<Entities.Author>;

            if (!string.IsNullOrWhiteSpace(mainCategory))
            {
                mainCategory = mainCategory.Trim().ToLower();
                collection = collection.Where(a => a.MainCategory.ToLower() == mainCategory);
            }

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                searchQuery = searchQuery.Trim();
                collection = collection.Where(a => a.MainCategory.Contains(searchQuery)
                    || a.FirstName.Contains(searchQuery)
                    || a.LastName.Contains(searchQuery));
            }
            return collection.ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authorsResourceParameters"></param>
        /// <returns></returns>
        public PagedList<Entities.Author> GetAuthors(AuthorsResourceParameters authorsResourceParameters)
        {
            if (authorsResourceParameters == null)
            {
                throw new ArgumentNullException(nameof(authorsResourceParameters));
            }

            var collection = _context.Authors.AsQueryable();

            if (!string.IsNullOrWhiteSpace(authorsResourceParameters.MainCategory))
            {
                var mainCategory = authorsResourceParameters.MainCategory.Trim();
                collection = collection.Where(a => a.MainCategory.Equals(mainCategory, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(authorsResourceParameters.SearchQuery))
            {
                var searchQuery = authorsResourceParameters.SearchQuery.Trim();
                collection = collection.Where(a => a.MainCategory.Contains(searchQuery)
                    || a.FirstName.Contains(searchQuery)
                    || a.LastName.Contains(searchQuery));
            }

            if (!string.IsNullOrWhiteSpace(authorsResourceParameters.OrderBy))
            {
                // get property mapping dictionary
                var authorPropertyMappingDictionary =
                    _propertyMappingService.GetPropertyMapping<Author, Entities.Author>();

                collection = collection.ApplySort(authorsResourceParameters.OrderBy, authorPropertyMappingDictionary);
            }

            return PagedList<Entities.Author>.Create(collection, authorsResourceParameters.PageNumber, authorsResourceParameters.PageSize);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authorIds"></param>
        /// <returns></returns>
        public IEnumerable<Entities.Author> GetAuthors(IEnumerable<Guid> authorIds)
        {
            if (authorIds == null)
            {
                throw new ArgumentNullException(nameof(authorIds));
            }

            return _context.Authors.Where(a => authorIds.Contains(a.Id))
                .OrderBy(a => a.FirstName)
                .OrderBy(a => a.LastName)
                .ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="author"></param>
        public void UpdateAuthor(Entities.Author author)
        {
            // no code in this implementation
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // dispose resources when needed
            }
        }
    }
}
