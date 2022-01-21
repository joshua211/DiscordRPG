using DiscordRPG.Application.Generators;
using DiscordRPG.Application.Interfaces;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Application.Models;
using DiscordRPG.Application.Services;
using DiscordRPG.Domain.DomainServices.Generators;
using EventFlow;
using EventFlow.Extensions;
using EventFlow.MongoDB.Extensions;
using EventFlow.ReadStores;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordRPG.Application;

public static class Application
{
    public static IServiceCollection AddApplication(this IServiceCollection collection)
    {
        return collection.AddTransient<ICharacterService, CharacterService>()
            .AddTransient<IActivityService, ActivityService>()
            .AddTransient<IDungeonService, DungeonService>()
            .AddTransient<IGuildService, GuildService>()
            .AddTransient<IShopService, ShopService>()
            .AddTransient<IChannelManager, ChannelManager>()
            .AddTransient<AspectGenerator>()
            .AddTransient<DungeonGenerator>()
            .AddTransient<IEncounterGenerator, EncounterGenerator>()
            .AddTransient<IExperienceGenerator, ExperienceGenerator>()
            .AddTransient<IItemGenerator, ItemGenerator>()
            .AddTransient<NameGenerator>()
            .AddTransient<RarityGenerator>()
            .AddTransient<RecipeGenerator>()
            .AddTransient<IWorthCalculator, WorthCalculator>()
            .AddTransient<IRandomizer, Randomizer>()
            .AddTransient<IReadModelLocator, ReadModelLocator>()
            .AddTransient<IWoundGenerator, WoundGenerator>();
    }

    public static IEventFlowOptions ConfigureApplication(this IEventFlowOptions options)
    {
        return options.AddDefaults(typeof(Application).Assembly)
            .UseMongoDbReadModel<ActivityReadModel, IReadModelLocator>()
            .UseMongoDbReadModel<CharacterReadModel, IReadModelLocator>()
            .UseMongoDbReadModel<DungeonReadModel, IReadModelLocator>()
            .UseMongoDbReadModel<ShopReadModel, IReadModelLocator>()
            .UseMongoDbReadModel<GuildReadModel>();
    }
}