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
        var health = dungeon.DungeonLevel * random.Next(8, 12);
        var dmg = dungeon.DungeonLevel * random.Next(5, 10);
        var armor = dungeon.DungeonLevel * random.Next(5, 10);
        var dmgType = GetRandomDamageType();

        Encounter encounter;
        if (random.Next(2) == 0)
        {
            encounter = new Encounter(new Damage(dmgType, (int) dmg), (int) health, (int) armor,
                0, dungeon.DungeonLevel);
        }
        else
        {
            encounter = new Encounter(new Damage(dmgType, (int) dmg), (int) health, 0,
                (int) armor, dungeon.DungeonLevel);
        }

        logger.Here().Verbose("Generated encounter {@Encounter} for dungeon Level {DungeonLevel}", encounter,
            dungeon.DungeonLevel);

        return encounter;
    }

    private DamageType GetRandomDamageType() => random.Next(2) == 0 ? DamageType.Physical : DamageType.Magical;
}