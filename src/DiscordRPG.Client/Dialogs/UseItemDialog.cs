using DiscordRPG.Application.Models;
using DiscordRPG.Common;
using DiscordRPG.Domain.Entities.Character.ValueObjects;

namespace DiscordRPG.Client.Dialogs;

public class UseItemDialog : Dialog
{
    public UseItemDialog(ulong userId, TransactionContext context) : base(userId, context)
    {
    }


    public CharacterReadModel Character { get; set; }
    public Item? SelectedItem { get; set; }
}