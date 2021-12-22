using Spectre.Console;

namespace DiscordRPG.DiagnosticConsole.Commands.Simulation;

public class GenerateCommand : ICommand
{
    public static string Command => "generate";
    public string CommandName => Command;

    public async Task ExecuteAsync(IEnumerable<ICommand> commands)
    {
        string choice;
        do
        {
            AnsiConsole.Clear();
            choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title(CommandName)
                .AddChoices(new[]
                {
                    GenerateItemCommand.Command,
                    GenerateDungeonCommand.Command,
                    GenerateCharacterCommand.Command,
                    "back"
                }));
            if (choice == "back")
                continue;

            var command = commands.First(c => c.CommandName == choice);
            await command.ExecuteAsync(commands);
        } while (choice != "back");
    }
}