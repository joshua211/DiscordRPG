using DiscordRPG.Core.Entities;
using DiscordRPG.Core.Enums;
using DiscordRPG.Core.ValueObjects;

namespace DiscordRPG.Client.Dialogs;

public class CraftingDialog : Dialog
{
    public CraftingDialog(ulong id) : base(id)
    {
    }

    public CraftingDialog()
    {
    }

    public Character Character { get; set; }
    public Recipe SelectedRecipe { get; set; }
    public EquipmentCategory EquipmentCategory { get; set; }
    public int CurrentPage { get; set; }
    public bool IsEquipment { get; set; }
}