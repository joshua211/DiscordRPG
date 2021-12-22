using DiscordRPG.Common;
using DiscordRPG.Core.DomainServices;
using DiscordRPG.Core.DomainServices.Generators;
using DiscordRPG.Core.Entities;
using DiscordRPG.Core.Enums;
using DiscordRPG.Core.ValueObjects;
using DiscordRPG.DiagnosticConsole.Models;
using DiscordRPG.DiagnosticConsole.Writers;
using Spectre.Console;

namespace DiscordRPG.DiagnosticConsole.Commands.Simulation;

public class GenerateCharacterCommand : ICommand
{
    private readonly IClassService classService;
    private readonly IItemGenerator itemGenerator;
    private readonly IRaceService raceService;
    private readonly ConsoleState state;

    public GenerateCharacterCommand(IClassService classService, IRaceService raceService, ConsoleState state,
        IItemGenerator itemGenerator)
    {
        this.classService = classService;
        this.raceService = raceService;
        this.state = state;
        this.itemGenerator = itemGenerator;
    }

    public static string Command => "generate character";
    public string CommandName => Command;

    public async Task ExecuteAsync(IEnumerable<ICommand> commands)
    {
        string choice;
        AnsiConsole.Clear();
        do
        {
            Character character = null;
            var choices = new List<string> {"generate"};
            if (state.SelectedEncounter is not null && state.SelectedCharacter is not null)
                choices.Add(GenerateWoundResultCommand.Command);
            choices.Add("back");

            choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title(CommandName)
                .AddChoices(choices));

            switch (choice)
            {
                case "back":
                    continue;
                case "generate":
                {
                    string secondChoice;
                    do
                    {
                        AnsiConsole.Clear();
                        var level = AnsiConsole.Prompt(new TextPrompt<uint>("Level: "));
                        var charClass = AnsiConsole.Prompt(new SelectionPrompt<int>().AddChoices(1, 2).UseConverter(i =>
                        {
                            var cl = classService.GetClass(i);
                            return cl.ClassName;
                        }));

                        var charRace = AnsiConsole.Prompt(new SelectionPrompt<int>().AddChoices(1, 2).UseConverter(i =>
                        {
                            var ra = raceService.GetRace(i);
                            return ra.RaceName;
                        }));

                        var rarity = AnsiConsole.Prompt(new SelectionPrompt<Rarity>().Title("Equipment rarity")
                            .AddChoices(Rarity.Common,
                                Rarity.Uncommon, Rarity.Rare, Rarity.Unique, Rarity.Legendary, Rarity.Mythic,
                                Rarity.Divine));
                        var weaponCategory = AnsiConsole.Prompt(new SelectionPrompt<EquipmentCategory>().Title("Weapon")
                            .AddChoices(EquipmentCategory.Sword, EquipmentCategory.Dagger, EquipmentCategory.Mace,
                                EquipmentCategory.Scepter, EquipmentCategory.Spear, EquipmentCategory.Staff,
                                EquipmentCategory.Bow));
                        var aspect = new Aspect("DEBUG", new[] {"DEBUG"});

                        var equipLevel = AnsiConsole.Prompt(new TextPrompt<uint>("Equip level: "));

                        var helmet =
                            itemGenerator.GenerateEquipment(rarity, equipLevel, aspect, EquipmentCategory.Helmet);
                        var armor = itemGenerator.GenerateEquipment(rarity, equipLevel, aspect,
                            EquipmentCategory.Armor);
                        var pants = itemGenerator.GenerateEquipment(rarity, equipLevel, aspect,
                            EquipmentCategory.Pants);
                        var amulet =
                            itemGenerator.GenerateEquipment(rarity, equipLevel, aspect, EquipmentCategory.Amulet);
                        var ring = itemGenerator.GenerateEquipment(rarity, equipLevel, aspect, EquipmentCategory.Ring);
                        var weapon = itemGenerator.GenerateWeapon(rarity, equipLevel, aspect, weaponCategory);
                        var equipInfo = new EquipmentInfo(weapon, helmet, armor, pants, amulet, ring);

                        character = new Character(new DiscordId("DBG"), new Identity("DBG"), "DEBUG CHAR", charClass,
                            charRace,
                            new Level(level, 0, 0), equipInfo,
                            new List<Item>(),
                            new List<Wound>(), 0);

                        CharacterWriter.Write(character);

                        secondChoice =
                            AnsiConsole.Prompt(
                                new SelectionPrompt<string>().AddChoices("generate new", "take", "back"));
                        switch (secondChoice)
                        {
                            case "back":
                                continue;
                            case "take":
                                state.SelectedCharacter = character;
                                break;
                        }
                    } while (secondChoice != "back" && secondChoice != "take");

                    break;
                }
                default:
                    var cmd = commands.First(c => c.CommandName == choice);
                    await cmd.ExecuteAsync(commands);
                    break;
            }
        } while (choice != "back");
    }
}