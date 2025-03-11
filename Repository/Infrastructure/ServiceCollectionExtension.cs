using Microsoft.Extensions.DependencyInjection;

namespace Repository.Infrastructure;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddSingleton<IAdCompanyRepository, AdCompanyRepository>();

        return services;
    }
}