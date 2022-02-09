using System.Reflection;
using Discord.Commands;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Application.Models;
using DiscordRPG.Client.Handlers;
using DiscordRPG.Common;
using DiscordRPG.Common.Extensions;
using DiscordRPG.Domain.Aggregates.Character.Commands;
using DiscordRPG.Domain.Aggregates.Guild;
using EventFlow;
using EventFlow.ReadStores;
using Serilog;

namespace DiscordRPG.Client.Modules;

[Group("rpg")]
[RequireOwner]
public class SettingsModule : ModuleBase<SocketCommandContext>
{
    private readonly ICommandBus bus;
    private readonly IGuildService guildService;
    private readonly ILogger logger;
    private readonly IReadModelPopulator populator;
    private readonly ServerHandler serverHandler;

    public SettingsModule(ServerHandler serverHandler, IGuildService guildService, IReadModelPopulator populator,
        ILogger logger, ICommandBus bus)
    {
        this.serverHandler = serverHandler;
        this.guildService = guildService;
        this.populator = populator;
        this.bus = bus;
        this.logger = logger.WithContext(GetType());
    }

    [Command("refresh-readmodels")]
    public async Task ForceReadModels()
    {
        await populator.PopulateAsync<CharacterReadModel>(CancellationToken.None);
        await populator.PopulateAsync<DungeonReadModel>(CancellationToken.None);
        await populator.PopulateAsync<ShopReadModel>(CancellationToken.None);
        await populator.PopulateAsync<GuildReadModel>(CancellationToken.None);
        await populator.PopulateAsync<ActivityReadModel>(CancellationToken.None);
    }

    [Command("uninstallcommands")]
    public async Task UninstallCommands()
    {
        await Context.Guild.DeleteApplicationCommandsAsync();
        logger.Here().Debug("Uninstalled Commands");
    }

    [Command("kill")]
    public async Task KillCharacter(long id)
    {
        var guildId = new GuildId(Context.Guild.Id.ToString());
        var charId = new CharacterId(Context.User.Id.ToString());
        var cmd = new KillCharacterCommand(guildId, charId, TransactionContext.New());
        await bus.PublishAsync(cmd, CancellationToken.None);

        await ReplyAsync("Killed " + id);
    }

    [Command("delete")]
    public async Task DeleteGuildAsync()
    {
        await serverHandler.CleanServer(Context.Guild);
        await ReplyAsync("Cleanup complete");
    }

    [Command("create")]
    public async Task CreateGuildAsync()
    {
        await serverHandler.SetupServer(Context.Guild);
        await ReplyAsync("Created guild");
    }

    [Command("installcommands")]
    public async Task InstallCommands()
    {
        await serverHandler.InstallGuildCommands(Context.Guild);
        await ReplyAsync("Installed commands");
    }

    [Command("version")]
    public async Task Version()
    {
        var version = Assembly.GetEntryAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            .InformationalVersion;
        await ReplyAsync($"Game Version: {version}");
    }
}