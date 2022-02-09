using DiscordRPG.Application.Models;
using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Character.ValueObjects;
using DiscordRPG.Domain.Aggregates.Guild;

namespace DiscordRPG.Client.Dialogs;

public class UseItemDialog : Dialog, IPageableDialog
{
    public UseItemDialog(ulong userId, TransactionContext context) : base(userId, context)
    {
    }


    public CharacterReadModel Character { get; set; }
    public Item? SelectedItem { get; set; }
    public GuildId GuildId { get; set; }
    public int CurrentPage { get; set; } = 1;
}