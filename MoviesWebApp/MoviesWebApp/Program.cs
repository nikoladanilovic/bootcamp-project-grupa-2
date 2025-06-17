
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MoviesWebApp.Repository;
using MoviesWebApp.Repository.Common;
using MoviesWebApp.Service;
using MoviesWebApp.Service.Common;


var builder = WebApplication.CreateBuilder(args);


builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
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
});


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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
