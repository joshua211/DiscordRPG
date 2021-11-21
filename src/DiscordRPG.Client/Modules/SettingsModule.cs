using Discord.Commands;

namespace DiscordRPG.Client.Modules;

public class SettingsModule : ModuleBase<SocketCommandContext>
{
    [Command("rpg-upgrade")]
    [RequireOwner]
    public async Task UpgradeAsync()
    {
        await Program.CommandHandler.InstallSlashCommands(Context.Guild);
        await ReplyAsync("Upgraded");
    }
}