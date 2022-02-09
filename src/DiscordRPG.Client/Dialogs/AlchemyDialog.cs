using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Character.Enums;
using DiscordRPG.Domain.Aggregates.Character.ValueObjects;
using DiscordRPG.Domain.Aggregates.Guild;

namespace DiscordRPG.Client.Dialogs;

public class AlchemyDialog : Dialog, IPageableDialog
{
    public AlchemyDialog(ulong userId, TransactionContext context) : base(userId, context)
    {
    }

    public RecipeCategory? RecipeCategory { get; set; }
    public List<Recipe> AvailableRecipes { get; set; }
    public Recipe SelectedRecipe { get; set; }
    public GuildId GuildId { get; set; }
    public CharacterId CharacterId { get; set; }
    public int CurrentPage { get; set; } = 1;
}