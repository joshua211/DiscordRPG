using DiscordRPG.Application.Models;
using DiscordRPG.Domain.Aggregates.Character.Enums;
using DiscordRPG.Domain.Aggregates.Character.ValueObjects;
using DiscordRPG.Domain.Aggregates.Guild.ValueObjects;
using DiscordRPG.Domain.DomainServices.Generators;

namespace DiscordRPG.Application.Generators;

public class WoundGenerator : GeneratorBase, IWoundGenerator
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

    public IEnumerable<Wound> GenerateWounds(Character characterEntity, Encounter encounter)
    {
        var character = new CharacterReadModel
        {
            Class = characterEntity.Class,
            Inventory = characterEntity.Inventory,
            Level = characterEntity.CharacterLevel,
            Money = characterEntity.Money,
            Name = characterEntity.Name,
            Race = characterEntity.Race,
            Wounds = characterEntity.Wounds
        };

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

        wounds.Add(new Wound(nameGenerator.GenerateWoundName(), totalDmg));
        logger.Verbose("Generated wounds {@Wounds} for encounter {@Encounter}", wounds, encounter);

        return wounds;
    }
}