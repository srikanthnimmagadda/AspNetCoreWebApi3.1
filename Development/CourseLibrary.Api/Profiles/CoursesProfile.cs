using AutoMapper;
using CourseLibrary.Api.Entities;

namespace CourseLibrary.Api.Profiles
{
    public class CoursesProfile : Profile
    {
        public CoursesProfile()
        {
            CreateMap<Course, Dto.Course>();
            CreateMap<Models.Course.Create, Course>();
            CreateMap<Models.Course.Update, Course>();
            CreateMap<Course, Models.Course.Update>();
        }
    }
}
