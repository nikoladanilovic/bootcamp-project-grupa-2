using AutoMapper;
using MoviesWebApp.Model;
using MoviesWebApp.RESTModels;

namespace MoviesWebApp
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Genre, GenreREST>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name ?? "Unknown"));
            CreateMap<GenreREST, Genre>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.MovieGenres, opt => opt.Ignore());
           
        }
    }
}