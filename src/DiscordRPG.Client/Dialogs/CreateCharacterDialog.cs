using DiscordRPG.Common;
using DiscordRPG.Domain.Entities.Character.ValueObjects;

namespace DiscordRPG.Client.Dialogs;

public class CreateCharacterDialog : Dialog
{
    public CreateCharacterDialog(ulong userId, TransactionContext context) : base(userId, context)
    {
    }


    public string GuildId { get; set; }
    public string? Name { get; set; }
    public CharacterRace Race { get; set; }
    public CharacterClass Class { get; set; }
}