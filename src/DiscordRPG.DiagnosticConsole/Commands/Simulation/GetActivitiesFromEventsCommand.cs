using DiscordRPG.Core.Enums;
using DiscordRPG.Core.Events;
using DiscordRPG.DiagnosticConsole.Importers;
using Spectre.Console;

namespace DiscordRPG.DiagnosticConsole.Commands.Simulation;

public class GetActivitiesFromEventsCommand : ICommand
{
    private readonly IEventImporter eventImporter;

    public GetActivitiesFromEventsCommand(IEventImporter eventImporter)
    {
        this.eventImporter = eventImporter;
    }

    public static string Command => "ev activities";
    public string CommandName => Command;

    public async Task ExecuteAsync(IEnumerable<ICommand> commands)
    {
        AnsiConsole.Clear();
        string choice;
        IEnumerable<object> events = null;
        await AnsiConsole.Status().StartAsync("Loading...",
            async context => { events = await eventImporter.GetEventsAsync(); });
        var activityType = ActivityType.Dungeon;
        do
        {
            var activityCreateds = events.OfType<ActivityCreated>().Where(e => e.Activity.Type == activityType);

            var grouped = activityCreateds.GroupBy(e => e.Activity.Duration);
            var chart = new BarChart();
            chart.Label = $"{activityType} distribution %";
            foreach (var group in grouped)
            {
                var percent = Math.Round((double) group.Count() / (double) activityCreateds.Count() * 100, 2);
                chart.AddItem(group.Key.ToString(), percent, Color.Navy);
            }

            AnsiConsole.Write(chart);

            choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title(CommandName)
                .AddChoices(new[]
                {
                    "type",
                    "back"
                }));

            if (choice == "back")
                continue;

            activityType = AnsiConsole.Prompt(new SelectionPrompt<ActivityType>().AddChoices(new[]
            {
                ActivityType.Dungeon, ActivityType.Rest, ActivityType.SearchDungeon
            }));
        } while (choice != "back");
    }
}