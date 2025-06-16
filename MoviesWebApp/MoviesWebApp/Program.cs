using Autofac;
using Autofac.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

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
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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
