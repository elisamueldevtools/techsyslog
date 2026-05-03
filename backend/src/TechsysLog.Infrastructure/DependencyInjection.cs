using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechsysLog.Application.Common.Interfaces;
using TechsysLog.Infrastructure.Auth;
using TechsysLog.Infrastructure.External;
using TechsysLog.Infrastructure.Persistence;
using TechsysLog.Infrastructure.Persistence.Repositories;
using TechsysLog.Infrastructure.Realtime;

namespace TechsysLog.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        MongoConventions.Register();

        services.Configure<MongoOptions>(configuration.GetSection("Mongo"));
        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
        services.Configure<CepOptions>(configuration.GetSection("Cep"));
        services.Configure<PasswordOptions>(configuration.GetSection("Password"));

        services.AddSingleton<MongoContext>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IDeliveryRepository, DeliveryRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        services.AddSingleton<IPasswordHasher, PepperedBCryptPasswordHasher>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IRealtimeNotifier, SignalRRealtimeNotifier>();

        var cepBaseUrl = configuration.GetSection("Cep")["BaseUrl"] ?? "https://viacep.com.br/ws/";
        services.AddHttpClient(ViaCepService.HttpClientName, c => c.BaseAddress = new Uri(cepBaseUrl));
        services.AddScoped<ICepService, ViaCepService>();

        return services;
    }
}
