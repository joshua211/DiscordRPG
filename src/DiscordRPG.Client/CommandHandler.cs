using System.Reflection;
using Discord.Commands;
using Discord.WebSocket;
using DiscordRPG.Client.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordRPG.Client;

public class CommandHandler
{
    private readonly DiscordSocketClient client;
    private readonly CommandService commandService;
    private readonly Dictionary<string, IGuildCommand> loadedCommands;
    private readonly IServiceProvider provider;

    public CommandHandler(DiscordSocketClient client, CommandService commandService, IServiceProvider provider)
    {
        this.client = client;
        this.commandService = commandService;
        this.provider = provider;
        loadedCommands = new Dictionary<string, IGuildCommand>();
    }

    public async Task InstallAsync()
    {
        client.JoinedGuild += SetupGuild;
        client.MessageReceived += HandleCommandAsync;
        client.SlashCommandExecuted += HandleSlashCommandAsync;

        LoadSlashCommands();
        await commandService.AddModulesAsync(Assembly.GetEntryAssembly(), provider);
    }

    private async Task HandleCommandAsync(SocketMessage messageParam)
    {
        var message = messageParam as SocketUserMessage;
        if (message == null) return;

        int argPos = 0;
        if (!(message.HasCharPrefix('/', ref argPos) || message.HasMentionPrefix(client.CurrentUser, ref argPos)) ||
            message.Author.IsBot)
            return;

        var context = new SocketCommandContext(client, message);

        await commandService.ExecuteAsync(context, argPos, provider);
    }

    private async Task HandleSlashCommandAsync(SocketSlashCommand command)
    {
        if (loadedCommands.TryGetValue(command.Data.Name, out var cmd))
        {
            await cmd.HandleAsync(command);
        }
        else
        {
            Console.WriteLine($"No Command {command.Data.Name} found");
        }
    }

    private void LoadSlashCommands()
    {
        var commands = provider.GetServices<IGuildCommand>();
        foreach (var command in commands)
            loadedCommands[command.CommandName] = command;
    }

    public async Task SetupGuild(SocketGuild guild)
    {
        await InstallSlashCommands(guild);
    }

    public async Task InstallSlashCommands(SocketGuild guild)
    {
        foreach (var cmd in loadedCommands.Values)
        {
            await cmd.InstallAsync(guild);
        }
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