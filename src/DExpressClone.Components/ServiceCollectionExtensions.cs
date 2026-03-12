using DExpressClone.Components.Interop;
using DExpressClone.Components.Layout;
using Microsoft.Extensions.DependencyInjection;

namespace DExpressClone.Components;

/// <summary>
/// Extension methods for registering DExpressClone component services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds DExpressClone component services to the service collection.
    /// </summary>
    public static IServiceCollection AddDxComponents(this IServiceCollection services)
    {
        services.AddScoped<JsInteropService>();
        services.AddScoped<DxToastService>();
        return services;
    }
}
