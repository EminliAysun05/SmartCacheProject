

using CacheSmartProject.Persistence.Repositories;
using CacheSmartProject.Persistence.Repositories.Implementations;
using CacheSmartProject.Persistence.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CacheSmartProject.Persistence.ServiceRegistrations;

public static class PersistanceServiceRegistration
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services)
    {
      
        services.AddScoped<IServiceRepository, ServiceRepository>();
        services.AddScoped<IStoryRepository, StoryRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();

        return services;
    }
}
