using CacheSmartProject.Application.ServiceRegistrations;
using CacheSmartProject.Infrastructure.ServiceRegistrations;
using CacheSmartProject.Middlewares;
using CacheSmartProject.Migrations.ServiceRegistrations;
using CacheSmartProject.Persistence.ServiceRegistrations;
namespace CacheSmartProject;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var env = builder.Environment.EnvironmentName;
        //var env = "Production";

        builder.Configuration
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env}.json", optional: true)
            .AddEnvironmentVariables();
        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddMigrationDbContext(builder.Configuration);
        builder.Services.AddPersistenceServices();
        builder.Services.AddInfrastructureServices(builder.Configuration);
        builder.Services.AddApplicationServices();
        var app = builder.Build();
        app.UseMiddleware<ExceptionHandlingMiddleware>();
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
    }
}
