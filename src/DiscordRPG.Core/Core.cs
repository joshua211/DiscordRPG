using DiscordRPG.Core.DomainServices;
using DiscordRPG.Core.DomainServices.Progress;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordRPG.Core;

public static class Core
{
    public static IServiceCollection AddCore(this IServiceCollection collection)
    {
        return collection
            .AddTransient<IExperienceCurve, ExperienceCurve>()
            .AddTransient<IProgressService, ProgressService>()
            .AddTransient<IAdventureResultService, AdventureResultService>();
    }
}