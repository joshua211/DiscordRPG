using DiscordRPG.DiagnosticConsole.Importers;
using DiscordRPG.DiagnosticConsole.Writers;
using DiscordRPG.Domain.Aggregates.Guild;
using Spectre.Console;

namespace DiscordRPG.DiagnosticConsole.Commands.View;

public class ViewAggregates : ICommand
{
    private readonly AggregateImporter aggregateImporter;

    public ViewAggregates(AggregateImporter aggregateImporter)
    {
        this.aggregateImporter = aggregateImporter;
    }

    public static string Command => "aggregates";
    public string CommandName => Command;

    public async Task ExecuteAsync(IEnumerable<ICommand> commands)
    {
        AnsiConsole.Clear();
        string result = "";
        while (result != "back")
        {
            IEnumerable<string> ids = null;
            await AnsiConsole.Status().StartAsync("Loading aggregates...",
                async context => { ids = await aggregateImporter.LoadAllAggregateIds(); });
            result = AnsiConsole.Prompt<string>(new SelectionPrompt<string>().AddChoices(ids.Append("back")));
            if (result == "back")
                continue;

            GuildAggregate aggregate = null;
            await AnsiConsole.Status().StartAsync($"Loading {result}...",
                async context =>
                {
                    aggregate = await aggregateImporter.LoadAggregate<GuildAggregate, GuildId>(new GuildId(result));
                });

            AggregateWriter.Write(aggregate);
        }
    }
}