using AutoMapper;
using CourseLibrary.Api.Extensions;

namespace CourseLibrary.Api.Profiles
{
    public class AuthorsProfile : Profile
    {
        /// <summary>
        /// 
        /// </summary>
        public AuthorsProfile()
        {
            CreateMap<Entities.Author, Dto.Author>()
                .ForMember(
                    dest => dest.Name, 
                    opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(
                    dest => dest.Age, 
                    opt => opt.MapFrom(src => src.DateOfBirth.GetCurrentAge(src.DateOfDeath)));

            CreateMap<Models.Author.Create, Entities.Author>();
            CreateMap<Entities.Author, Dto.AuthorMain>();
            CreateMap<Entities.Author, Models.Author.CreateWithDateOfDeath>();
        }
    }
}
