using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using MoviesWebApp.Repository;
using MoviesWebApp.Repository.Common;
using MoviesWebApp.Service;
using MoviesWebApp.Service.Common;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole(); // or AddFile(), AddDebug() etc.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MappingProfile));


builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    var configuration = builder.Configuration;
    var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    containerBuilder.RegisterInstance(connectionString).As<string>();

    containerBuilder.RegisterType<GenreService>().As<IGenreService>().InstancePerLifetimeScope();
    containerBuilder.RegisterType<GenreRepository>().As<IGenreRepository>().InstancePerLifetimeScope();
    containerBuilder.RegisterType<DirectorRepository>().As<IDirectorRepository>().InstancePerLifetimeScope();
    containerBuilder.RegisterType<DirectorService>().As<IDirectorService>().InstancePerLifetimeScope();
    containerBuilder.RegisterType<UsersService>().As<IUsersService>().InstancePerLifetimeScope();
    containerBuilder.RegisterType<UsersRepository>().As<IUsersRepository>().InstancePerLifetimeScope();
    containerBuilder.RegisterType<MovieService>().As<IMovieService>().InstancePerLifetimeScope();
    containerBuilder.RegisterType<MovieRepository>().As<IMovieRepository>().InstancePerLifetimeScope();
    containerBuilder.RegisterType<ReviewService>().As<IReviewService>().InstancePerLifetimeScope();
    containerBuilder.RegisterType<ReviewRepository>().As<IReviewRepository>().InstancePerLifetimeScope();
    containerBuilder.RegisterType<MovieGenreService>().As<IMovieGenreService>().InstancePerLifetimeScope();
    containerBuilder.RegisterType<MovieGenreRepository>().As<IMovieGenreRepository>().InstancePerLifetimeScope();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();