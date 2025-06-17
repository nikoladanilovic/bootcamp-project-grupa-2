<<<<<<< HEAD
using Autofac.Extensions.DependencyInjection;
using Autofac;
using Microsoft.AspNetCore.Builder;
using MoviesWebApp.Service.Common;
using MoviesWebApp.Service;
using MoviesWebApp.Repository.Common;
using MoviesWebApp.Repository;
=======
using Autofac;
using Autofac.Extensions.DependencyInjection;
<<<<<<< HEAD
<<<<<<< HEAD
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MoviesWebApp.Repository;
using MoviesWebApp.Repository.Common;
using MoviesWebApp.Service;
using MoviesWebApp.Service.Common;
=======
=======
>>>>>>> 02fe9ee09262a5d9ac50323e39b5c2ab7b6f7070
>>>>>>> 56073b666f02b0182f71bffc2c413ae16e2fa742

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

<<<<<<< HEAD
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder.RegisterType<GenreService>().As<IGenreService>().InstancePerLifetimeScope();
    containerBuilder.RegisterType<GenreRepository>().As<IGenreRepository>().InstancePerLifetimeScope();
        });

=======
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    // Register your services here
    containerBuilder.RegisterType<MoviesWebApp.Repository.DirectorRepository>()
        .As<MoviesWebApp.Repository.Common.IDirectorRepository>()
        .InstancePerLifetimeScope();
    containerBuilder.RegisterType<MoviesWebApp.Service.DirectorService>()
        .As<MoviesWebApp.Service.Common.IDirectorService>()
        .InstancePerLifetimeScope();
});
>>>>>>> 56073b666f02b0182f71bffc2c413ae16e2fa742
builder.Services.AddControllers();
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder.RegisterType<UsersService>().As<IUsersService>().InstancePerLifetimeScope();
    containerBuilder.RegisterType<UsersRepository>().As<IUsersRepository>().InstancePerLifetimeScope();
});
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
