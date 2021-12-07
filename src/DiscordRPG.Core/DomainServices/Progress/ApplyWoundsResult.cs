namespace DiscordRPG.Core.DomainServices.Progress;

public class ApplyWoundsResult
{
    public ApplyWoundsResult(bool hasDied, Wound? finalWound, IEnumerable<Wound> woundsGained, int newHealth)
    {
        HasDied = hasDied;
        FinalWound = finalWound;
        WoundsGained = woundsGained;
        NewHealth = newHealth;
    }

    public bool HasDied { get; private set; }
    public Wound? FinalWound { get; private set; }
    public IEnumerable<Wound> WoundsGained { get; private set; }
    public int NewHealth { get; private set; }
}