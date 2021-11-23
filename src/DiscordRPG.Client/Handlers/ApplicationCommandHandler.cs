using System.Reflection;
using Discord.WebSocket;
using DiscordRPG.Client.Commands.Base;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace DiscordRPG.Client.Handlers;

public class ApplicationCommandHandler : IHandler
{
    private readonly DiscordSocketClient client;
    private readonly Dictionary<string, IGuildCommand> loadedCommands;
    private readonly ILogger logger;
    private readonly IServiceProvider provider;

    public ApplicationCommandHandler(DiscordSocketClient client, IServiceProvider provider, ILogger logger)
    {
        this.client = client;
        this.provider = provider;
        this.logger = logger;
        loadedCommands = new Dictionary<string, IGuildCommand>();
    }

    public IEnumerable<IGuildCommand> LoadedCommands => loadedCommands.Values;

    public Task InstallAsync()
    {
        logger.Information("Installing ApplicationCommandHandler");
        client.SlashCommandExecuted += HandleSlashCommandAsync;
        LoadSlashCommands();

        return Task.CompletedTask;
    }

    private async Task HandleSlashCommandAsync(SocketSlashCommand command)
    {
        if (loadedCommands.TryGetValue(command.Data.Name, out var cmd))
        {
            await cmd.HandleAsync(command);
        }
        else
        {
            logger.Warning("No guildCommand with Name {Name} found", command.Data.Name);
        }
    }

    private void LoadSlashCommands()
    {
        var commands = provider.GetServices<IGuildCommand>();
        foreach (var command in commands)
            loadedCommands[command.CommandName] = command;
    }

    public static void AddCommands(IServiceCollection collection)
    {
        var assembly = Assembly.GetEntryAssembly();
        foreach (var type in assembly.ExportedTypes.Where(t =>
                     !t.IsInterface && !t.IsAbstract && t.GetInterface(nameof(IGuildCommand)) != null))
        {
            collection.AddSingleton(typeof(IGuildCommand), type);
        }
    }
}