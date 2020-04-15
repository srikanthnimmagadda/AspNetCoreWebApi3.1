using System;
using System.ComponentModel.DataAnnotations;

namespace CourseLibrary.Api.Attributes.Validation
{
    /// <summary>
    /// 
    /// </summary>
    public class MustBeDifferentAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var course = (Models.Course.Base)validationContext.ObjectInstance;

            if (course.Title.Equals(course.Description, StringComparison.OrdinalIgnoreCase))
            {
                return new ValidationResult(ErrorMessage,
                    new[] { nameof(Models.Course.Create) });
            }

            return ValidationResult.Success;
        }
    }
}
