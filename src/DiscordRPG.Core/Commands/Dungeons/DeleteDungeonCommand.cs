namespace DiscordRPG.Core.Commands.Dungeons;

public class DeleteDungeonCommand : Command
{
    public DeleteDungeonCommand(Identity id)
    {
        Id = id;
    }

    public Identity Id { get; private set; }
}