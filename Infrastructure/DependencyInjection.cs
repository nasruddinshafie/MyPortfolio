using Microsoft.Extensions.DependencyInjection;
using Application.Common.Interfaces;
using Infrastructure.Features.Bio.Repositories;
using Infrastructure.Features.Projects.Repositories;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IBioRepository, BioRepository>();
        services.AddScoped<IProjectRepository, ProjectRepository>();

        return services;
    }
}