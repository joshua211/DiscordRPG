using System.Reflection;
using Discord.Commands;
using Discord.WebSocket;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Client.Commands;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace DiscordRPG.Client;

public class CommandHandler
{
    private readonly DiscordSocketClient client;
    private readonly CommandService commandService;
    private readonly IGuildService guildService;
    private readonly Dictionary<string, IGuildCommand> loadedCommands;
    private readonly ILogger logger;
    private readonly IServiceProvider provider;

    public CommandHandler(DiscordSocketClient client, CommandService commandService, IServiceProvider provider)
    {
        this.client = client;
        this.commandService = commandService;
        this.provider = provider;
        guildService = provider.GetService<IGuildService>();
        logger = Log.Logger;
        loadedCommands = new Dictionary<string, IGuildCommand>();
    }

    public async Task InstallAsync()
    {
        client.JoinedGuild += SetupGuildAsync;
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

    public async Task SetupGuildAsync(SocketGuild socketGuild)
    {
        await InstallSlashCommands(socketGuild);
        await CreateGuildAsync(socketGuild);
    }

    private async Task CreateGuildAsync(SocketGuild socketGuild)
    {
        try
        {
            logger.Information("Setting up channels for Guild {Id}", socketGuild.Id);
            var category = await socketGuild.CreateCategoryChannelAsync("--RPG--");

            var guildHall =
                await socketGuild.CreateTextChannelAsync("Guild Hall",
                    properties => properties.CategoryId = category.Id);
            var dungeonHall =
                await socketGuild.CreateTextChannelAsync("Dungeon Hall",
                    properties => properties.CategoryId = category.Id);

            var result =
                await guildService.CreateGuildAsync(socketGuild.Id, socketGuild.Name, guildHall.Id, dungeonHall.Id);
            if (result.WasSuccessful)
            {
                logger.Information("Created Guild {Id}", socketGuild.Id);

                return;
            }

            logger.Warning("Failed to create Guild. {Msg}", result.ErrorMessage);
        }
        catch (Exception e)
        {
            logger.Error(e, "Failed to create Guild");
        }
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