using Microsoft.Extensions.DependencyInjection;
using Application.Common.Interfaces;
using Infrastructure.Features.Bio.Repositories;
using Infrastructure.Features.Projects.Repositories;
using Infrastructure.Features.Auth.Repositories;
using Infrastructure.Features.Auth.Security;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IBioRepository, BioRepository>();
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenGenerator, JwtTokenGenerator>();

        return services;
    }
}