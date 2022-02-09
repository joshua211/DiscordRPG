using DiscordRPG.Application.Models;
using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Activity.Enums;

namespace DiscordRPG.Client.Dialogs;

public class SearchDungeonDialog : Dialog
{
    public SearchDungeonDialog(ulong userId, TransactionContext context) : base(userId, context)
    {
    }


    public CharacterReadModel Character { get; set; }
    public ulong ServerId { get; set; }
    public ActivityDuration Duration { get; set; }
}