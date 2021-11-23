namespace DiscordRPG.Core.Commands.Activities;

public class DeleteActivityCommand : Command
{
    public DeleteActivityCommand(string id)
    {
        Id = id;
    }

    public string Id { get; private set; }
}