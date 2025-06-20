using AutoMapper;
using MoviesWebApp.Model;
using MoviesWebApp.RESTModels;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserREST>()
            .ReverseMap()
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

        CreateMap<Review, ReviewREST>()
            .ReverseMap()
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
        CreateMap<Movie, MovieREST>();
        CreateMap<MovieREST, Movie>();
        CreateMap<Director, DirectorREST>();
        CreateMap<DirectorREST, Director>();

    }
}