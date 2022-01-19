using DiscordRPG.Domain.DomainServices;
using DiscordRPG.Domain.Services;
using EventFlow;
using EventFlow.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordRPG.Domain;

public static class Domain
{
    public static IServiceCollection AddDomain(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddTransient<IExperienceCurve, ExperienceCurve>()
            .AddTransient<ICraftingService, CraftingService>()
            .AddTransient<IAdventureResultService, AdventureResultService>();
    }

    public static IEventFlowOptions ConfigureDomain(this IEventFlowOptions options)
    {
        return options.AddDefaults(typeof(Domain).Assembly);
    }
}