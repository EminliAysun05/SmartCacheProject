using CacheSmartProject.Application.ServiceRegistrations;
using CacheSmartProject.Infrastructure.Caching;
using CacheSmartProject.Infrastructure.ServiceRegistrations;
using CacheSmartProject.Middlewares;
using CacheSmartProject.Migrations.ServiceRegistrations;
using CacheSmartProject.Persistence.ServiceRegistrations;

namespace CacheSmartProject;

public class Program
{
    public async static Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var env = builder.Environment.EnvironmentName;
        builder.Configuration
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env}.json", optional: true)
            .AddEnvironmentVariables();

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddMigrationDbContext(builder.Configuration);
        builder.Services.AddPersistenceServices();
        builder.Services.AddInfrastructureServices(builder.Configuration);
        builder.Services.AddApplicationServices();
        var app = builder.Build();
        using (var scope = app.Services.CreateScope())
        {
            var warmer = scope.ServiceProvider.GetRequiredService<CacheWarmingService>();
            await warmer.WarmAllAsync(); 
        }
        app.UseMiddleware<ExceptionHandlingMiddleware>();
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
