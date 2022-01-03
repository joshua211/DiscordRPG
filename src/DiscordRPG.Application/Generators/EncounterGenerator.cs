using DiscordRPG.Core.DomainServices;
using DiscordRPG.Core.DomainServices.Generators;

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
        var worth = worthCalculator.CalculateWorth(dungeon.Rarity, dungeon.DungeonLevel) * 5;

        var health = (int) (worth * (1 + random.Next(-1, 2) * 0.1f));
        var dmg = worth / 3;
        var armor = worth / 2;
        var dmgType = GetRandomDamageType();

        Encounter encounter =
            new Encounter(new Damage(dmgType, dmg), health, armor / 2, armor / 2, dungeon.DungeonLevel);

        logger.Here().Verbose("Generated encounter {@Encounter} for dungeon Level {DungeonLevel}", encounter,
            dungeon.DungeonLevel);

        return encounter;
    }

    private DamageType GetRandomDamageType() => random.Next(2) == 0 ? DamageType.Physical : DamageType.Magical;
}