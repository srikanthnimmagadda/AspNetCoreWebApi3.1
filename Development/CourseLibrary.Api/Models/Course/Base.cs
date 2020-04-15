using CourseLibrary.Api.Attributes.Validation;
using System.ComponentModel.DataAnnotations;

namespace CourseLibrary.Api.Models.Course
{
    [MustBeDifferent(ErrorMessage = "The Title must be different from Description.")]
    public abstract class Base
    {
        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "The Title field is required.")]
        [MaxLength(100, ErrorMessage = "The Title must not have more than 100 characters.")]
        public string Title { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [MaxLength(1500, ErrorMessage = "The Description must not have more than 1500 characters.")]
        public virtual string Description { get; set; }
    }
}
