using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CacheSmartProject.Migrations.ServiceRegistrations;

public static class MigrationServiceRegistration
{
    public static IServiceCollection AddMigrationDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        return services;
    }
}
