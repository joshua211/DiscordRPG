namespace DiscordRPG.Client.Dialogs;

public class CreateCharacterDialog : Dialog
{
    public CreateCharacterDialog(ulong id) : base(id)
    {
    }

    public CreateCharacterDialog()
    {
    }

    public string GuildId { get; set; }
    public string? Name { get; set; }
    public string? Race { get; set; }
    public string? Class { get; set; }
}