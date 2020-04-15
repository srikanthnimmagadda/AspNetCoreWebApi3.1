using CourseLibrary.Api.Attributes.Validation;
namespace CourseLibrary.Api.Models.Course
{
    public class Create : Base //: IValidatableObject
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    if (Title.Equals(Description, StringComparison.OrdinalIgnoreCase))
        //    {
        //        yield return new ValidationResult("The provided description must be different from title.",
        //            new[] { "Create" });
        //    }
        //}
    }
}
