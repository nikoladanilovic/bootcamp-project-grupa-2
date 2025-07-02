using AutoMapper;
using MoviesWebApp.Model;
using MoviesWebApp.RESTModels;

namespace MoviesWebApp
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Genre, GenreREST>().ReverseMap();
            CreateMap<Director, DirectorREST>().ReverseMap();
            CreateMap<Actor, ActorREST>().ReverseMap();
            CreateMap<Movie, MovieREST>().ReverseMap();
        }
    }
}