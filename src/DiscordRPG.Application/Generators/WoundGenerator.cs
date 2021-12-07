using DiscordRPG.Core.DomainServices;
using DiscordRPG.Core.DomainServices.Generators;

namespace DiscordRPG.Application.Generators;

public class WoundGenerator : GeneratorBase, IWoundGenerator
{
    private readonly ILogger logger;
    private readonly INameGenerator nameGenerator;

    public WoundGenerator(ILogger logger, INameGenerator nameGenerator)
    {
        this.nameGenerator = nameGenerator;
        this.logger = logger.WithContext(GetType());
    }

    public IEnumerable<Wound> GenerateWounds(Character character, Encounter encounter)
    {
        var wounds = new List<Wound>();
        var totalDmg = 0;
        while (encounter.Health > 0 && character.CurrentHealth > totalDmg)
        {
            var dmgToEncounter = character.TotalDamage.Value - (character.TotalDamage.DamageType == DamageType.Physical
                ? encounter.Armor
                : encounter.MagicArmor);
            dmgToEncounter = dmgToEncounter <= 0 ? 1 : dmgToEncounter;
            encounter.Health -= dmgToEncounter;

            if (encounter.Health <= 0)
                continue;

            var dmgToCharacter = encounter.Damage.Value - (encounter.Damage.DamageType == DamageType.Physical
                ? character.Armor
                : character.MagicArmor);
            if (dmgToCharacter > 0)
            {
                wounds.Add(new Wound(nameGenerator.GenerateWoundName(), dmgToCharacter));
                totalDmg += dmgToCharacter;
            }

            if (totalDmg > character.CurrentHealth)
                continue;
        }

        logger.Here().Verbose("Generated wounds {@Wounds} for encounter {@Encounter}", wounds, encounter);

        return wounds;
    }
}