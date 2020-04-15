using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CourseLibrary.Api.Entities
{
    public class Author
    {
        /// <summary>
        /// 
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public DateTimeOffset DateOfBirth { get; set; }          

        /// <summary>
        /// 
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string MainCategory { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTimeOffset? DateOfDeath { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<Course> Courses { get; set; }
            = new List<Course>();
    }
}
