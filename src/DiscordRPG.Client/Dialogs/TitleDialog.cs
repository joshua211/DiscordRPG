using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Character;
using DiscordRPG.Domain.Entities.Character.ValueObjects;

namespace DiscordRPG.Client.Dialogs;

public class TitleDialog : Dialog, IPageableDialog
{
    public TitleDialog(ulong userId, TransactionContext context) : base(userId, context)
    {
    }

    public GuildId GuildId { get; set; }
    public CharacterId CharacterId { get; set; }
    public IEnumerable<Title> AllTitles { get; set; }
    public Title? CurrentTitle { get; set; }
    public Title? SelectedTitle { get; set; }

    public int CurrentPage { get; set; } = 1;
}