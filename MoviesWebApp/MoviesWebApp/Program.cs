
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MoviesWebApp;
using MoviesWebApp.Repository;
using MoviesWebApp.Repository.Common;
using MoviesWebApp.Service;
using MoviesWebApp.Service.Common;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    var configuration = builder.Configuration;
    var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    containerBuilder.RegisterInstance(connectionString).As<string>();
    containerBuilder.RegisterType<GenreService>().As<IGenreService>().InstancePerLifetimeScope();
    containerBuilder.RegisterType<GenreRepository>().As<IGenreRepository>().InstancePerLifetimeScope();
    containerBuilder.RegisterType<MoviesWebApp.Repository.DirectorRepository>()
        .As<MoviesWebApp.Repository.Common.IDirectorRepository>()
        .InstancePerLifetimeScope();
    containerBuilder.RegisterType<MoviesWebApp.Service.DirectorService>()
        .As<MoviesWebApp.Service.Common.IDirectorService>()
        .InstancePerLifetimeScope();
    containerBuilder.RegisterType<UsersService>().As<IUsersService>().InstancePerLifetimeScope();
    containerBuilder.RegisterType<UsersRepository>().As<IUsersRepository>().InstancePerLifetimeScope();
    containerBuilder.RegisterType<MovieService>().As<IMovieService>().InstancePerLifetimeScope();
    containerBuilder.RegisterType<MovieRepository>().As<IMovieRepository>().InstancePerLifetimeScope();
    containerBuilder.RegisterType<ReviewService>().As<IReviewService>().InstancePerLifetimeScope();
    containerBuilder.RegisterType<ReviewRepository>().As<IReviewRepository>().InstancePerLifetimeScope();
    containerBuilder.RegisterType<MovieGenreRepository>().As<IMovieGenreRepository>().InstancePerLifetimeScope();
    containerBuilder.RegisterType<MovieGenreService>().As<IMovieGenreService>().InstancePerLifetimeScope();
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
