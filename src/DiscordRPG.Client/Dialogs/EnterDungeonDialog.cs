using DiscordRPG.Core.Entities;
using DiscordRPG.Core.Enums;

namespace DiscordRPG.Client.Dialogs;

public class EnterDungeonDialog : Dialog
{
    public EnterDungeonDialog(ulong userId) : base(userId)
    {
    }


    public Dungeon Dungeon { get; set; }
    public string CharId { get; set; }
    public ActivityDuration Duration { get; set; }
}