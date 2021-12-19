using System.Reflection;
using Discord.Commands;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Client.Handlers;

namespace DiscordRPG.Client.Modules;

[Group("rpg")]
[RequireOwner]
public class SettingsModule : ModuleBase<SocketCommandContext>
{
    private readonly IGuildService guildService;
    private readonly ServerHandler serverHandler;

    public SettingsModule(ServerHandler serverHandler, IGuildService guildService)
    {
        this.serverHandler = serverHandler;
        this.guildService = guildService;
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