using DiscordRPG.Core.DomainServices;
using DiscordRPG.Core.DomainServices.Generators;

namespace DiscordRPG.Application.Generators;

public class EncounterGenerator : GeneratorBase, IEncounterGenerator
{
    private readonly ILogger logger;

    public EncounterGenerator(ILogger logger)
    {
        this.logger = logger.WithContext(GetType());
    }

    public Encounter CreateDungeonEncounter(Dungeon dungeon)
    {
        //TODO random values based on level and rarity
        var health = 10 + dungeon.DungeonLevel * 10;
        var dmg = 10 + dungeon.DungeonLevel * 10;
        var armor = 10 + dungeon.DungeonLevel * 10;
        var mArmor = 10 + dungeon.DungeonLevel * 10;

        var encounter = new Encounter(new Damage(DamageType.Physical, (int) dmg), (int) health, (int) armor,
            (int) mArmor, dungeon.DungeonLevel);
        logger.Here().Verbose("Generated encounter {@Encounter} for dungeon Level {DungeonLevel}", encounter,
            dungeon.DungeonLevel);

        return encounter;
    }
}