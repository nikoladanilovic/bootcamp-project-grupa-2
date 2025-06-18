using AutoMapper;
using MoviesWebApp.RESTModels;
using MoviesWebApp.Model;

namespace MoviesWebApp
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Movie mappings
            CreateMap<MovieREST, Movie>();
            CreateMap<Movie, MovieREST>();
            // Director mappings
            CreateMap<DirectorREST, Director>();
            CreateMap<Director, DirectorREST>();
        }
    }
}
