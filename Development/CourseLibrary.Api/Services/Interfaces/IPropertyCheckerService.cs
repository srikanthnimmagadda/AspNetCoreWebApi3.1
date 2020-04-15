namespace CourseLibrary.Api.Services.Interfaces
{
    public interface IPropertyCheckerService
    {
        bool IsTypeHasProperties<T>(string fields);
    }
}