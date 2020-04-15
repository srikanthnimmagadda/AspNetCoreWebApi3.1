﻿using CourseLibrary.Api.Entities;
using CourseLibrary.Api.Utility;
using System;
using System.Collections.Generic;

namespace CourseLibrary.Api.Services.Interfaces
{
    public interface ICourseLibraryRepository
    {
        IEnumerable<Course> GetCourses(Guid authorId);
        Course GetCourse(Guid authorId, Guid courseId);
        void AddCourse(Guid authorId, Course course);
        void UpdateCourse(Course course);
        void DeleteCourse(Course course);
        IEnumerable<Author> GetAuthors();
        IEnumerable<Author> GetAuthors(string mainCategory, string searchQuery);
        PagedList<Author> GetAuthors(Dto.AuthorsResourceParameters authorsResourceParameters);
        Author GetAuthor(Guid authorId);
        IEnumerable<Author> GetAuthors(IEnumerable<Guid> authorIds);
        void AddAuthor(Author author);
        void DeleteAuthor(Author author);
        void UpdateAuthor(Author author);
        bool IsAuthorExists(Guid authorId);
        bool Save();
    }
}
