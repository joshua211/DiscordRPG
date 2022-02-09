using DiscordRPG.Application.Models;
using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Activity.Enums;

namespace DiscordRPG.Client.Dialogs;

public class EnterDungeonDialog : Dialog
{
    public EnterDungeonDialog(ulong userId, TransactionContext context) : base(userId, context)
    {
    }

    public string GuildId { get; set; }
    public DungeonReadModel Dungeon { get; set; }
    public string CharId { get; set; }
    public ActivityDuration Duration { get; set; }
}