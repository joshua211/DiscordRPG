using System.Reflection;
using Discord.Commands;
using Discord.WebSocket;
using DiscordRPG.Common.Extensions;
using Serilog;

namespace DiscordRPG.Client.Handlers;

public class MessageCommandHandler : IHandler
{
    private readonly DiscordSocketClient client;
    private readonly CommandService commandService;
    private readonly ILogger logger;
    private readonly IServiceProvider provider;

    public MessageCommandHandler(DiscordSocketClient client, CommandService commandService, IServiceProvider provider,
        ILogger logger)
    {
        this.client = client;
        this.commandService = commandService;
        this.provider = provider;
        this.logger = logger.WithContext(GetType());
    }

    public async Task InstallAsync()
    {
        logger.Here().Information("Installing MessageCommandHandler");
        client.MessageReceived += HandleCommandAsync;

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
}