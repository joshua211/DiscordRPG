using DiscordRPG.Application.Data;
using DiscordRPG.Application.Interfaces;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Application.Repositories;
using DiscordRPG.Application.Services;
using DiscordRPG.Common;
using DiscordRPG.Core.DomainServices;
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
            .AddTransient<IRepository<Character>, CharacterRepository>()
            .AddTransient<IRepository<Guild>, GuildRepository>()
            .AddTransient<IRepository<Activity>, ActivityRepository>()
            .AddTransient<IRepository<Dungeon>, DungeonRepository>()
            .AddTransient<IChannelManager, ChannelManager>()
            .AddSingleton<IClassService, Classes>()
            .AddSingleton<IRaceService, Races>();
    }
}