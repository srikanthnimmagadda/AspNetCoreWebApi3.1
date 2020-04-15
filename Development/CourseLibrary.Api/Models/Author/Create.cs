using System;
using System.Collections.Generic;

namespace CourseLibrary.Api.Models.Author
{
    public class Create
    {
        /// <summary>
        /// 
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTimeOffset DateOfBirth { get; set; }

       

        /// <summary>
        /// 
        /// </summary>
        public string MainCategory { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<Course.Create> Courses { get; set; }
          = new List<Course.Create>();
    }
}
