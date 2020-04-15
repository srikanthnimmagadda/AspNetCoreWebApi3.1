using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseLibrary.Api.Entities
{
    public class Course
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
        [MaxLength(100)]
        public string Title { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [MaxLength(1500)]
        public string Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ForeignKey("AuthorId")]
        public Author Author { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid AuthorId { get; set; }
    }
}
