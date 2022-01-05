using DiscordRPG.Application.Data;
using DiscordRPG.Core.DomainServices.Generators;
using DiscordRPG.Core.Enums;
using DiscordRPG.DiagnosticConsole.Writers;
using Spectre.Console;

namespace DiscordRPG.DiagnosticConsole.Commands.Simulation;

public class GenerateItemCommand : ICommand
{
    private readonly IItemGenerator itemGenerator;

    public GenerateItemCommand(IItemGenerator itemGenerator)
    {
        this.itemGenerator = itemGenerator;
    }

    public static string Command => "generate item";
    public string CommandName => Command;

    public async Task ExecuteAsync(IEnumerable<ICommand> commands)
    {
        AnsiConsole.Clear();
        string itemChoice;
        do
        {
            itemChoice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title(CommandName)
                .AddChoices(new[]
                {
                    "item",
                    "equipment",
                    "weapon",
                    "back"
                }));
            if (itemChoice == "back")
                continue;

            var rarity = AnsiConsole.Prompt(new SelectionPrompt<Rarity>().AddChoices(Rarity.Common, Rarity.Uncommon,
                Rarity.Rare, Rarity.Unique, Rarity.Legendary, Rarity.Mythic, Rarity.Divine));
            var level = AnsiConsole.Prompt(new TextPrompt<uint>("Level: "));


            var aspect = Aspects.DebugAspect;
            string choice;
            do
            {
                switch (itemChoice)
                {
                    case "item":
                        var item = itemGenerator.GenerateRandomItem(rarity, level, 1);
                        AnsiConsole.Clear();
                        ItemWriter.Write(item);
                        break;
                    case "equipment":
                        var equipment = itemGenerator.GenerateRandomEquipment(rarity, level, aspect);
                        AnsiConsole.Clear();
                        ItemWriter.Write(equipment);
                        break;
                    case "weapon":
                        var weapon = itemGenerator.GenerateRandomWeapon(rarity, level, aspect);
                        AnsiConsole.Clear();
                        ItemWriter.Write(weapon);
                        break;
                }

                choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                    .AddChoices(
                        "generate new",
                        "take",
                        "back"));

                if (choice == "back")
                    continue;
            } while (choice != "back" && choice != "take");
        } while (itemChoice != "back");
    }
}