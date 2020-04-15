using System.ComponentModel.DataAnnotations;

namespace CourseLibrary.Api.Models.Course
{
    public class Update : Base
    {
        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "The Description field is required.")]
        public override string Description { get => base.Description; set => base.Description = value; }
    }
}
