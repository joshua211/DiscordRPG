using DiscordRPG.Core.Commands.Shops;
using DiscordRPG.Core.DomainServices;
using DiscordRPG.Core.DomainServices.Progress;
using DiscordRPG.Core.Entities;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

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

    public static LoggerConfiguration CoreLogging(this LoggerConfiguration logger)
    {
        return logger.Destructure.ByTransforming<Character>(c => new
        {
            Id = c.ID,
            Name = c.CharacterName,
            Level = c.Level,
            Strength = c.Stength,
            Agility = c.Agility,
            Vitality = c.Vitality,
            Intelligence = c.Intelligence,
            ItemCount = c.Inventory.Count,
            ClassName = c.CharacterClass.ClassName,
            RaceName = c.CharacterRace.RaceName,
            Damage = c.TotalDamage,
            MaxHealth = c.MaxHealth,
            Health = c.CurrentHealth,
            Armor = c.Armor,
            MArmor = c.MagicArmor
        }).Destructure.ByTransforming<Shop>(s => new
        {
            Id = s.ID,
            GuildId = s.GuildId,
            Inventory = s.ItemsForSale.Select(i => new {CharacterId = i.CharId, ItemCount = i.Equipment.Count})
        }).Destructure.ByTransforming<UpdateShopCommand>(u => new
        {
            Shop = u.Shop,
            CharId = u.Character.ID
        });
    }
}