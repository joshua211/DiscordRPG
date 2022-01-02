namespace DiscordRPG.Core.Commands.Characters;

public class DeleteCharacterCommand : Command
{
    public DeleteCharacterCommand(Identity charId)
    {
        CharId = charId;
    }

    public Identity CharId { get; private set; }
}