using DiscordRPG.Common;
using DiscordRPG.Domain.Entities.Character.ValueObjects;

namespace DiscordRPG.Client.Dialogs;

public class RecipesDialog : Dialog, IPageableDialog
{
    public RecipesDialog(ulong userId, TransactionContext context) : base(userId, context)
    {
    }

    public List<Recipe> AllRecipes { get; set; }
    public Recipe? SelectedRecipe { get; set; }
    public List<Item> PlayerInventory { get; set; }

    public int CurrentPage { get; set; } = 1;
}