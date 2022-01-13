using DiscordRPG.Application.Models;
using DiscordRPG.Domain.Aggregates.Guild.ValueObjects;
using DiscordRPG.Domain.Entities.Character.Enums;
using DiscordRPG.Domain.Entities.Character.ValueObjects;

namespace DiscordRPG.Application.Generators;

public class WoundGenerator : GeneratorBase
{
    private readonly ILogger logger;
    private readonly NameGenerator nameGenerator;
    private readonly IRandomizer randomizer;

    public WoundGenerator(ILogger logger, NameGenerator nameGenerator, IRandomizer randomizer)
    {
        this.nameGenerator = nameGenerator;
        this.randomizer = randomizer;
        this.logger = logger.WithContext(GetType());
    }

    public IEnumerable<Wound> GenerateWounds(CharacterReadModel character, Encounter encounter)
    {
        var wounds = new List<Wound>();
        var totalDmg = 0;
        var encounterHealth = encounter.Health;
        var charHasFirstStrike = randomizer.GetRandomized(character.Agility, 0.1f) >
                                 randomizer.GetRandomized(encounter.Agility, 0.1f);

        while (encounterHealth > 0 && character.CurrentHealth > totalDmg)
        {
            var dmgToEncounter = (int) (character.TotalDamage.Value *
                                        (character.TotalDamage.DamageType == DamageType.Magical
                                            ? 1 - (float) encounter.MagicArmor / (encounter.Level * 10)
                                            : 1 - (float) encounter.Armor / (encounter.Level * 10)));
            dmgToEncounter = dmgToEncounter < 1 ? 1 : randomizer.GetRandomized(dmgToEncounter, 0.2f);

            var dmgToChar = (int) (encounter.Damage.Value * (encounter.Damage.DamageType == DamageType.Magical
                ? 1 - (float) character.MagicArmor / (character.Level.CurrentLevel * 10)
                : 1 - (float) character.Armor / (character.Level.CurrentLevel * 10)));
            dmgToChar = dmgToChar < 1 ? 1 : randomizer.GetRandomized(dmgToChar, 0.2f);

            if (charHasFirstStrike)
            {
                encounterHealth -= dmgToEncounter;
                if (encounterHealth <= 0)
                    continue;

                if (dmgToChar > 0)
                {
                    totalDmg += dmgToChar;
                }
            }
            else
            {
                if (dmgToChar > 0)
                {
                    totalDmg += dmgToChar;
                }

                encounterHealth -= dmgToEncounter;
                if (encounterHealth <= 0)
                    continue;
            }

            charHasFirstStrike = !charHasFirstStrike;
        }

        logger.Verbose("Generated wounds {@Wounds} for encounter {@Encounter}", wounds, encounter);

        return new[] {new Wound(nameGenerator.GenerateWoundName(), totalDmg)};
    }
}