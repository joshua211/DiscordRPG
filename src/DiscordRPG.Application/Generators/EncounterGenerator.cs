using DiscordRPG.Domain.Aggregates.Guild.ValueObjects;
using DiscordRPG.Domain.DomainServices.Generators;
using DiscordRPG.Domain.Entities.Character.Enums;
using DiscordRPG.Domain.Entities.Character.ValueObjects;
using DiscordRPG.Domain.Entities.Dungeon;

namespace DiscordRPG.Application.Generators;

public class EncounterGenerator : GeneratorBase, IEncounterGenerator
{
    private readonly ILogger logger;
    private readonly IWorthCalculator worthCalculator;

    public EncounterGenerator(ILogger logger, IWorthCalculator worthCalculator)
    {
        this.worthCalculator = worthCalculator;
        this.logger = logger.WithContext(GetType());
    }

    public Encounter CreateDungeonEncounter(Dungeon dungeon)
    {
        var worth = worthCalculator.CalculateWorth(dungeon.Rarity, dungeon.Level.Value);

        var health = (int) (worth * (1 + random.Next(-1, 2) * 0.1f));
        health = health < 50 ? 50 : health;
        var dmg = worth / 3;
        var armor = worth / 2;
        var dmgType = GetRandomDamageType();
        var agi = worth / 5;
        var str = worth / 5;
        var intel = worth / 5;
        var luck = worth / 5;
        var vit = worth / 5;

        Encounter encounter =
            new Encounter(new Damage(dmgType, dmg), health, armor / 2, armor / 2, dungeon.Level.Value, agi, str, vit,
                intel, luck);

        logger.Verbose("Generated encounter {@Encounter} for dungeon Level {DungeonLevel}", encounter,
            dungeon.Level.Value);

        return encounter;
    }

    private DamageType GetRandomDamageType() => random.Next(2) == 0 ? DamageType.Physical : DamageType.Magical;
}