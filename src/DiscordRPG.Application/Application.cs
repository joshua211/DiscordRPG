using DiscordRPG.Application.Data;
using DiscordRPG.Application.Generators;
using DiscordRPG.Application.Interfaces;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Application.Repositories;
using DiscordRPG.Application.Services;
using DiscordRPG.Application.Worker;
using DiscordRPG.Common;
using DiscordRPG.Core.DomainServices;
using DiscordRPG.Core.DomainServices.Generators;
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
            .AddTransient<IShopService, ShopService>()
            .AddTransient<IRepository<Character>, CharacterRepository>()
            .AddTransient<IRepository<Guild>, GuildRepository>()
            .AddTransient<IRepository<Activity>, ActivityRepository>()
            .AddTransient<IRepository<Dungeon>, DungeonRepository>()
            .AddTransient<IRepository<Shop>, ShopRepository>()
            .AddTransient<IChannelManager, ChannelManager>()
            .AddTransient<IRarityGenerator, RarityGenerator>()
            .AddTransient<IDungeonGenerator, DungeonGenerator>()
            .AddTransient<INameGenerator, NameGenerator>()
            .AddTransient<IItemGenerator, ItemGenerator>()
            .AddTransient<IWoundGenerator, WoundGenerator>()
            .AddTransient<IAspectGenerator, AspectGenerator>()
            .AddTransient<IExperienceGenerator, ExperienceGenerator>()
            .AddTransient<IEncounterGenerator, EncounterGenerator>()
            .AddSingleton<IClassService, Classes>()
            .AddSingleton<IRaceService, Races>()
            .AddSingleton<INameGenerator, NameGenerator>()
            .AddSingleton<DiagnosticsWorker>();
    }
}