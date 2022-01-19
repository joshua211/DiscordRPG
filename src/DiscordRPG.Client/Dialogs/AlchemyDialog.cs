using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Character;
using DiscordRPG.Domain.Entities.Character.Enums;
using DiscordRPG.Domain.Entities.Character.ValueObjects;

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