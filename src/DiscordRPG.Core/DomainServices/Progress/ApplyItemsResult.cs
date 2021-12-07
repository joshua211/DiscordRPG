namespace DiscordRPG.Core.DomainServices.Progress;

public class ApplyItemsResult
{
    public ApplyItemsResult(IEnumerable<Item> itemsGained)
    {
        ItemsGained = itemsGained;
    }

    public IEnumerable<Item> ItemsGained { get; private set; }
}