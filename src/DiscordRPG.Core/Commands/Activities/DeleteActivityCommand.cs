namespace DiscordRPG.Core.Commands.Activities;

public class DeleteActivityCommand : Command
{
    public DeleteActivityCommand(Identity id)
    {
        Id = id;
    }

    public Identity Id { get; private set; }
}