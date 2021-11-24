using DiscordRPG.Application.Interfaces;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Application.Repositories;
using DiscordRPG.Application.Services;
using DiscordRPG.Core.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordRPG.Application;

public static class Application
{
    public static IServiceCollection AddApplication(this IServiceCollection collection)
    {
        return collection
            .AddTransient<ICharacterService, CharacterService>()
            .AddTransient<IGuildService, GuildService>()
            .AddTransient<IActivityService, ActivityService>()
            .AddTransient<IDungeonService, DungeonService>()
            .AddTransient<ICharacterRepository, CharacterRepository>()
            .AddTransient<IGuildRepository, GuildRepository>()
            .AddTransient<IActivityRepository, ActivityRepository>()
            .AddTransient<IDungeonRepository, DungeonRepository>()
            .AddTransient<IChannelManager, ChannelManager>();
    }
}