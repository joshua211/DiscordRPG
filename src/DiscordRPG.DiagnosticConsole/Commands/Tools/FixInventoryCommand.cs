using DiscordRPG.Core.Entities;
using DiscordRPG.Core.ValueObjects;
using DiscordRPG.DiagnosticConsole.Importers;
using Spectre.Console;

namespace DiscordRPG.DiagnosticConsole.Commands.Tools;

public class FixInventoryCommand : ICommand
{
    private readonly ICharacterImporter characterImporter;

    public FixInventoryCommand(ICharacterImporter characterImporter)
    {
        this.characterImporter = characterImporter;
    }

    public static string Command => "fix inventory";
    public string CommandName => Command;

    public async Task ExecuteAsync(IEnumerable<ICommand> commands)
    {
        string choice;
        do
        {
            choice = AnsiConsole.Prompt(new SelectionPrompt<string>().AddChoices(new[] {"fix", "back"}));
            if (choice == "back")
                continue;

            var faultyCharacters = new HashSet<Character>();
            int result = 0;
            await AnsiConsole.Status().StartAsync("Loading Characters...", async context =>
            {
                var characters = await characterImporter.GetCharactersAsync();
                context.Status($"Fixing inventory for {characters.Count()} entities...");
                foreach (var character in characters)
                {
                    var itemsToRemove = new List<Item>();
                    var existing = new List<Item>();
                    foreach (var item in character.Inventory)
                    {
                        if (existing.All(i => i.GetItemCode() != item.GetItemCode()))
                            existing.Add(item);
                        else
                        {
                            faultyCharacters.Add(character);
                            var existingItem = character.Inventory.First(i => i.GetItemCode() == item.GetItemCode());
                            existingItem.Amount += item.Amount;
                            itemsToRemove.Add(item);
                        }
                    }

                    itemsToRemove.ForEach(i => character.Inventory.Remove(i));
                }

                context.Status("Updating characters in database...");
                result = await characterImporter.UpdateCharactersAsync(faultyCharacters);
            });

            AnsiConsole.MarkupLine($"Fixed [green]{result}[/] characters");
        } while (choice != "back");
    }
}