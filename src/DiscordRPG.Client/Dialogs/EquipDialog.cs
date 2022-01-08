using Discord;
using DiscordRPG.Core.Entities;
using DiscordRPG.Core.Enums;
using DiscordRPG.Core.ValueObjects;

namespace DiscordRPG.Client.Dialogs;

public class EquipDialog : Dialog, IShareableDialog, IPageableDialog
{
    public EquipDialog(ulong userId) : base(userId)
    {
    }

    public Character Character { get; set; }
    public Item CurrentItem { get; set; }
    public EquipmentPosition Position { get; set; }
    public int CurrentPage { get; set; } = 1;
    public Embed ShareableEmbed { get; set; }
}