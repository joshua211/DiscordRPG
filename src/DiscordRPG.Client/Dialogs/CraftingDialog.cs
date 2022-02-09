using DiscordRPG.Application.Models;
using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Character.Enums;

namespace DiscordRPG.Client.Dialogs;

public class CraftingDialog : Dialog
{
    public CraftingDialog(ulong userId, TransactionContext context) : base(userId, context)
    {
    }


    public CharacterReadModel Character { get; set; }

    /*public Recipe SelectedRecipe { get; set; }*/
    public EquipmentCategory EquipmentCategory { get; set; }
    public int CurrentPage { get; set; }
    public bool IsEquipment { get; set; }
    public int RecipeCount { get; set; }
}