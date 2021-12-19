using Discord;
using Discord.WebSocket;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Client.Commands.Attributes;
using DiscordRPG.Common.Extensions;
using DiscordRPG.Core.Entities;
using DiscordRPG.Core.Enums;
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
        var requireActivity =
            (RequireActivityAttribute) Attribute.GetCustomAttribute(GetType(),
                typeof(RequireActivityAttribute))!;
        var requireDungeon =
            (RequireDungeonAttribute) Attribute.GetCustomAttribute(GetType(), typeof(RequireDungeonAttribute))!;

        if (requireName is not null)
        {
            if (command.Channel.Name != requireName.ChannelName)
            {
                await command.RespondAsync($"This command can only be used in the {requireName.ChannelName} channel!",
                    ephemeral: true);
                return;
            }
        }

        Guild guild = null;
        if (requireGuild is not null)
        {
            var channel = command.Channel as IGuildChannel;
            var guildResult = await guildService.GetGuildWithDiscordIdAsync(channel.Guild.Id.ToString());
            if (!guildResult.WasSuccessful)
            {
                await command.RespondAsync("No Guild was setup for this server!", ephemeral: true);
                return;
            }

            guild = guildResult.Value;
        }

        Dungeon dungeon = null;
        if (requireDungeon is not null)
        {
            var dungeonResult = await dungeonService.GetDungeonFromChannelIdAsync(command.Channel.Id.ToString());
            if (!dungeonResult.WasSuccessful)
            {
                await command.RespondAsync("This command can only be used in a dungeon!", ephemeral: true);
                return;
            }

            dungeon = dungeonResult.Value;
        }

        Character character = null;
        if (requireChar is not null)
        {
            var user = command.User as IGuildUser;
            var charResult = await characterService.GetUsersCharacterAsync(user.Id.ToString(), guild.ID);
            if (!charResult.WasSuccessful)
            {
                await command.RespondAsync("Please create a character first!", ephemeral: true);
                return;
            }

            character = charResult.Value;
        }


        if (requireNoActivity is not null)
        {
            var currentActivityResult =
                await activityService.GetCharacterActivityAsync(character.ID);
            if (currentActivityResult.WasSuccessful)
            {
                var timeLeft = ((currentActivityResult.Value.StartTime +
                                 TimeSpan.FromMinutes((int) currentActivityResult.Value.Duration)) -
                                DateTime.UtcNow);
                await command.RespondAsync(
                    $"You're already on an adventure, try again in {timeLeft.TotalMinutes} minutes!",
                    ephemeral: true);
                return;
            }
        }

        Activity activity = null;
        if (requireActivity is not null)
        {
            var currentActivityResult =
                await activityService.GetCharacterActivityAsync(character.ID);
            if (!currentActivityResult.WasSuccessful)
            {
                await command.RespondAsync($"You are currently not doing anything", ephemeral: true);
                return;
            }

            activity = currentActivityResult.Value;
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

    protected static SlashCommandOptionBuilder GetActivityDurationBuilder(string description)
    {
        return new SlashCommandOptionBuilder()
            .WithName("duration")
            .WithDescription(description)
            .WithType(ApplicationCommandOptionType.Integer)
            .WithRequired(true)
            .AddChoice("quick", (int) ActivityDuration.Quick)
            .AddChoice("short", (int) ActivityDuration.Short)
            .AddChoice("medium", (int) ActivityDuration.Medium)
            .AddChoice("long", (int) ActivityDuration.Long);
    }
}