using DiscordRPG.Core.DomainServices.Generators;
using DiscordRPG.DiagnosticConsole.Models;
using DiscordRPG.DiagnosticConsole.Writers;
using Spectre.Console;

namespace DiscordRPG.DiagnosticConsole.Commands.Simulation;

public class GenerateDungeonItemsCommand : ICommand
{
    private readonly IItemGenerator itemGenerator;
    private readonly ConsoleState state;

    public GenerateDungeonItemsCommand(ConsoleState state, IItemGenerator itemGenerator)
    {
        this.state = state;
        this.itemGenerator = itemGenerator;
    }

    public static string Command => "item result";
    public string CommandName => Command;

    public async Task ExecuteAsync(IEnumerable<ICommand> commands)
    {
        string choice;
        do
        {
            var items = itemGenerator.GenerateItems(state.SelectedDungeon).ToList();
            AnsiConsole.Clear();
            ItemWriter.Write(items);

            choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title(CommandName)
                .AddChoices("generate", "back"));
        } while (choice != "back");
    }
}