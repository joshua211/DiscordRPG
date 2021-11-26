using Discord;
using Discord.WebSocket;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Client.Commands.Attributes;
using DiscordRPG.Common.Extensions;
using DiscordRPG.Core.Entities;
using Serilog;

namespace DiscordRPG.Client.Commands.Base;

public abstract class CommandBase : IGuildCommand
{
    protected readonly IActivityService activityService;
    protected readonly ICharacterService characterService;
    protected readonly DiscordSocketClient client;
    protected readonly IDungeonService dungeonService;
    protected readonly IGuildService guildService;
    protected readonly ILogger logger;

    protected CommandBase(DiscordSocketClient client, ILogger logger, IActivityService activityService,
        ICharacterService characterService, IDungeonService dungeonService, IGuildService guildService)
    {
        this.client = client;
        this.logger = logger.WithContext(GetType());
        this.activityService = activityService;
        this.characterService = characterService;
        this.dungeonService = dungeonService;
        this.guildService = guildService;

        client.SelectMenuExecuted += HandleSelectionAsync;
        client.ButtonExecuted += HandleButtonAsync;
    }

    public abstract string CommandName { get; }

    public abstract Task InstallAsync(SocketGuild guild);

    //TODO maybe handle this in parallel?
    public async Task HandleAsync(SocketSlashCommand command)
    {
        var requireName =
            (RequireChannelNameAttribute) Attribute.GetCustomAttribute(GetType(), typeof(RequireChannelNameAttribute))!;
        var requireGuild =
            (RequireGuildAttribute) Attribute.GetCustomAttribute(GetType(), typeof(RequireGuildAttribute))!;
        var requireChar =
            (RequireCharacterAttribute) Attribute.GetCustomAttribute(GetType(), typeof(RequireCharacterAttribute))!;
        var requireNoActivity =
            (RequireNoCurrentActivityAttribute) Attribute.GetCustomAttribute(GetType(),
                typeof(RequireNoCurrentActivityAttribute))!;
        var requireDungeon =
            (RequireDungeonAttribute) Attribute.GetCustomAttribute(GetType(), typeof(RequireDungeonAttribute))!;

        if (requireName is not null)
        {
            if (command.Channel.Name != requireName.ChannelName)
            {
                await command.RespondAsync($"This command can only be used in the {requireName.ChannelName} channel!");
                return;
            }
        }

        Guild guild = null;
        if (requireGuild is not null)
        {
            var channel = command.Channel as IGuildChannel;
            var guildResult = await guildService.GetGuildWithDiscordIdAsync(channel.Guild.Id);
            if (!guildResult.WasSuccessful)
            {
                await command.RespondAsync("No Guild was setup for this server!");
                return;
            }

            guild = guildResult.Value;
        }

        Dungeon dungeon = null;
        if (requireDungeon is not null)
        {
            var dungeonResult = await dungeonService.GetDungeonFromChannelIdAsync(command.Channel.Id);
            if (!dungeonResult.WasSuccessful)
            {
                await command.RespondAsync("This command can only be used in a dungeon!");
                return;
            }

            dungeon = dungeonResult.Value;
        }

        Character character = null;
        if (requireChar is not null)
        {
            var user = command.User as IGuildUser;
            var charResult = await characterService.GetCharacterAsync(user.Id, guild.ID);
            if (!charResult.WasSuccessful)
            {
                await command.RespondAsync("Please create a character first!");
                return;
            }

            character = charResult.Value;
        }

        Activity activity = null;
        if (requireNoActivity is not null)
        {
            var currentActivityResult =
                await activityService.GetCharacterActivityAsync(character.ID);
            if (currentActivityResult.WasSuccessful)
            {
                var timeLeft = ((currentActivityResult.Value.StartTime + currentActivityResult.Value.Duration) -
                                DateTime.UtcNow);
                await command.RespondAsync($"You're already on an adventure, try again in {timeLeft.Minutes} minutes!");
                return;
            }
        }

        await HandleAsync(command, new GuildCommandContext(character, activity, dungeon, guild));
    }

    private async Task HandleSelectionAsync(SocketMessageComponent component)
    {
        var idSplit = component.Data.CustomId.Split('.');
        if (idSplit[0] != CommandName)
            return;

        await HandleSelectionAsync(component, idSplit[1]);
    }

    private async Task HandleButtonAsync(SocketMessageComponent component)
    {
        var idSplit = component.Data.CustomId.Split('.');
        if (idSplit[0] != CommandName)
            return;

        await HandleButtonAsync(component, idSplit[1]);
    }

    protected abstract Task HandleAsync(SocketSlashCommand command, GuildCommandContext context);

    protected virtual Task HandleSelectionAsync(SocketMessageComponent component, string id)
        => Task.CompletedTask;

    protected virtual Task HandleButtonAsync(SocketMessageComponent component, string id) => Task.CompletedTask;
}