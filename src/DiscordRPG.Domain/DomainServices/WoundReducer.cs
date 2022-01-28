using DiscordRPG.Domain.Entities.Character.ValueObjects;

namespace DiscordRPG.Domain.DomainServices;

public class WoundReducer : IWoundReducer
{
    public IEnumerable<Wound> ReduceDamageFromWounds(IEnumerable<Wound> existing, int hpToReduce)
    {
        var newWounds = existing.ToList();
        while (hpToReduce > 0)
        {
            var wound = newWounds.LastOrDefault();
            if (wound is null)
                break;

            if (wound.DamageValue <= hpToReduce)
                newWounds.Remove(wound);
            else
            {
                newWounds.Remove(wound);
                newWounds.Add(wound.DecreaseDamage(hpToReduce));
            }

            hpToReduce -= wound.DamageValue;
        }

        return newWounds;
    }
}

public interface IWoundReducer
{
    IEnumerable<Wound> ReduceDamageFromWounds(IEnumerable<Wound> existing, int hpToReduce);
}