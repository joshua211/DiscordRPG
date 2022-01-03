using System.Collections.Concurrent;
using DiscordRPG.Common;
using DiscordRPG.Core.DomainServices;
using DiscordRPG.Core.DomainServices.Generators;
using DiscordRPG.Core.Entities;
using DiscordRPG.Core.Enums;
using DiscordRPG.Core.ValueObjects;
using Spectre.Console;

namespace DiscordRPG.DiagnosticConsole.Commands.Simulation;

public class DungeonClearingSimulation : ICommand
{
    private readonly IAdventureResultService adventureResultService;
    private readonly IClassService classService;
    private readonly IDungeonGenerator dungeonGenerator;
    private readonly IItemGenerator itemGenerator;
    private readonly IRaceService raceService;
    private ConcurrentBag<(uint level, Rarity rarity, ActivityDuration duration, bool died)> results;

    public DungeonClearingSimulation(IItemGenerator itemGenerator, IClassService classService, IRaceService raceService,
        IDungeonGenerator dungeonGenerator, IAdventureResultService adventureResultService)
    {
        this.itemGenerator = itemGenerator;
        this.classService = classService;
        this.raceService = raceService;
        this.dungeonGenerator = dungeonGenerator;
        this.adventureResultService = adventureResultService;
        results = new ConcurrentBag<(uint level, Rarity rarity, ActivityDuration duration, bool died)>();
    }

    public static string Command => "dungeon simulation";
    public string CommandName => Command;

    public async Task ExecuteAsync(IEnumerable<ICommand> commands)
    {
        string choice;
        do
        {
            AnsiConsole.Clear();
            choice = AnsiConsole.Prompt(new SelectionPrompt<string>().AddChoices("generate", "back"));
            if (choice == "back")
                continue;

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

            AnsiConsole.Status().Start("Calculating...", context =>
            {
                var res = Parallel.For(1, 100, (int e) =>
                {
                    var i = (uint) e;
                    var category = charClass switch
                    {
                        1 => EquipmentCategory.Staff,
                        2 => EquipmentCategory.Sword,
                        _ => EquipmentCategory.Dagger
                    };

                    var equip = GetEquipment(rarity, i, category);
                    var character = new Character(new DiscordId("debug"), new Identity("debug"), "DEBUG", charClass,
                        charRace,
                        new Level(i, 1, 1), equip, new List<Item>(), new List<Wound>(), 0);
                    character.RaceService = raceService;
                    character.ClassService = classService;

                    foreach (var r in Enum.GetValues<Rarity>())
                    {
                        var dungeon =
                            dungeonGenerator.GenerateRandomDungeon(new DiscordId(""), new DiscordId(""), i,
                                new Aspect("DEBUG", new[] {"DEBUG"}),
                                r);
                        foreach (var duration in Enum.GetValues<ActivityDuration>())
                        {
                            if (duration == ActivityDuration.ExtraLong)
                                continue;

                            for (int j = 0; j < 100; j++)
                            {
                                var result =
                                    adventureResultService.CalculateAdventureResult(character, dungeon, duration);
                                results.Add((i, r, duration,
                                    result.Wounds.Sum(w => w.DamageValue) >= character.MaxHealth));
                            }
                        }
                    }
                });
                if (res.IsCompleted) ;
            });
            string secondChoice;
            do
            {
                secondChoice = AnsiConsole.Prompt(new SelectionPrompt<string>().AddChoices("select", "back"));
                if (secondChoice == "back")
                    continue;

                var showLevel = AnsiConsole.Prompt(new TextPrompt<int>("Level:"));
                var showRarity = AnsiConsole.Prompt(new SelectionPrompt<Rarity>().AddChoices(Rarity.Common,
                    Rarity.Uncommon,
                    Rarity.Rare, Rarity.Unique, Rarity.Legendary, Rarity.Mythic, Rarity.Divine));

                var chart = new BarChart();
                var items = results.Where(r => r.level == showLevel && r.rarity == showRarity);
                chart.Label($"Lvl. {showLevel}");

                foreach (var actual in items.GroupBy(i => i.duration))
                {
                    chart.AddItem($"{showRarity} {actual.Key}",
                        Math.Round((double) actual.Count(i => !i.died) / actual.Count() * 100, 2),
                        Color.Green);
                    chart.AddItem($"{showRarity} {actual.Key}(D)",
                        Math.Round((double) actual.Count(i => i.died) / actual.Count() * 100, 2), Color.Red);
                }

                AnsiConsole.Write(chart);
            } while (secondChoice != "back");
        } while (choice != "back");
    }

    private EquipmentInfo GetEquipment(Rarity rarity, uint level, EquipmentCategory weaponCategory)
    {
        var aspect = new Aspect("DEBUG", new[] {"DEBUG"});
        var helmet =
            itemGenerator.GenerateEquipment(rarity, level, aspect, EquipmentCategory.Helmet);
        var armor = itemGenerator.GenerateEquipment(rarity, level, aspect,
            EquipmentCategory.Armor);
        var pants = itemGenerator.GenerateEquipment(rarity, level, aspect,
            EquipmentCategory.Pants);
        var amulet =
            itemGenerator.GenerateEquipment(rarity, level, aspect, EquipmentCategory.Amulet);
        var ring = itemGenerator.GenerateEquipment(rarity, level, aspect, EquipmentCategory.Ring);
        var weapon = itemGenerator.GenerateWeapon(rarity, level, aspect, weaponCategory);

        return new EquipmentInfo(weapon, helmet, armor, pants, amulet, ring);
    }
}