using DiscordRPG.Application.Data;
using DiscordRPG.Core.DomainServices.Generators;
using DiscordRPG.Core.Enums;
using DiscordRPG.Core.ValueObjects;
using DiscordRPG.DiagnosticConsole.Models;
using DiscordRPG.DiagnosticConsole.Writers;
using Spectre.Console;

namespace DiscordRPG.DiagnosticConsole.Commands.Simulation;

public class GenerateDungeonCommand : ICommand
{
    private readonly IDungeonGenerator dungeonGenerator;
    private readonly ConsoleState state;

    public GenerateDungeonCommand(IDungeonGenerator dungeonGenerator, ConsoleState state)
    {
        this.dungeonGenerator = dungeonGenerator;
        this.state = state;
    }

    public static string Command => "generate dungeon";
    public string CommandName => Command;

    public async Task ExecuteAsync(IEnumerable<ICommand> commands)
    {
        AnsiConsole.Clear();
        string optionChoice;
        do
        {
            var choices = new List<string>();
            choices.Add("generate");
            if (state.SelectedDungeon is not null)
            {
                choices.Add(GenerateDungeonItemsCommand.Command);
                choices.Add(GenerateEncounterResultCommand.Command);
                if (state.SelectedCharacter is not null)
                    choices.Add(GenerateAdventureResultCommand.Command);
            }

            choices.Add("back");

            optionChoice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title(CommandName)
                .AddChoices(choices));
            switch (optionChoice)
            {
                case "back":
                    continue;
                case "generate":
                {
                    var rarity = AnsiConsole.Prompt(new SelectionPrompt<Rarity>().AddChoices(Rarity.Common,
                        Rarity.Uncommon,
                        Rarity.Rare, Rarity.Unique, Rarity.Legendary, Rarity.Mythic, Rarity.Divine));
                    var level = AnsiConsole.Prompt(new TextPrompt<uint>("Level: "));

                    var aspect = Aspects.DebugAspect;
                    string choice;
                    do
                    {
                        var dungeon =
                            dungeonGenerator.GenerateRandomDungeon(new DiscordId(""), new DiscordId(""), level, aspect,
                                rarity);

                        AnsiConsole.Clear();
                        DungeonWriter.Write(dungeon);

                        choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                            .AddChoices(
                                "generate new",
                                "take",
                                "back"));

                        switch (choice)
                        {
                            case "back":
                                continue;
                            case "take":
                                state.SelectedDungeon = dungeon;
                                break;
                        }
                    } while (choice != "back" && choice != "take");

                    break;
                }
                default:
                    var cmd = commands.First(c => c.CommandName == optionChoice);
                    await cmd.ExecuteAsync(commands);
                    break;
            }
        } while (optionChoice != "back");
    }
}