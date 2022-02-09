using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Character.Enums;
using DiscordRPG.Domain.Aggregates.Character.ValueObjects;
using DiscordRPG.Domain.Aggregates.Guild;

namespace DiscordRPG.Client.Dialogs;

public class ForgeDialog : Dialog, IPageableDialog
{
    public ForgeDialog(ulong userId, TransactionContext context) : base(userId, context)
    {
    }

    public CharacterId CharacterId { get; set; }
    public GuildId GuildId { get; set; }
    public uint Level { get; set; }
    public List<Item> AvailableItems { get; set; } = new();
    public Item? SelectedItem { get; set; }
    public List<(Item item, int amount)> Ingredients { get; } = new();
    public EquipmentCategory? Category { get; set; }

    public int CurrentPage { get; set; } = 1;

    public void IncreaseIngredient(Item item)
    {
        if (Ingredients.Sum(s => s.amount) >= 20)
            return;

        var existing = Ingredients.FirstOrDefault(i => i.item.Id == item.Id);
        if (existing.Equals(default))
            Ingredients.Add(new(item, 1));
        else
        {
            if (item.Amount > existing.amount)
                Ingredients[Ingredients.IndexOf(existing)] = new(existing.item, existing.amount + 1);
        }
    }

    public void DecreaseIngredient(Item item)
    {
        var existing = Ingredients.FirstOrDefault(i => i.item.Id == item.Id);
        if (existing.Equals(default))
            return;

        if (existing.amount > 1)
            Ingredients[Ingredients.IndexOf(existing)] = new(existing.item, existing.amount - 1);
        else
            Ingredients.Remove(existing);
    }
}