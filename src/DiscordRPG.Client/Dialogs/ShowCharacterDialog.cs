using Discord;

namespace DiscordRPG.Client.Dialogs;

public class ShowCharacterDialog : Dialog, IShareableDialog
{
    public ShowCharacterDialog(ulong userId) : base(userId)
    {
    }

    public Embed ShareableEmbed { get; set; }
}