namespace DiscordRPG.DiagnosticConsole.Commands;

public interface ICommand
{
    string CommandName { get; }
    Task ExecuteAsync(IEnumerable<ICommand> commands);
}