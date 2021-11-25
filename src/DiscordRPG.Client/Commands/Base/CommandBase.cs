using Discord;
using Discord.WebSocket;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Client.Commands.Attributes;
using DiscordRPG.Core.Entities;
using Serilog;

namespace DiscordRPG.Client.Commands.Base;

public abstract class CommandBase : IGuildCommand
{
    protected readonly IActivityService activityService;
    protected readonly ICharacterService characterService;
    protected readonly DiscordSocketClient client;
    protected readonly ILogger logger;

    protected CommandBase(DiscordSocketClient client, ILogger logger, IActivityService activityService,
        ICharacterService characterService)
    {
        this.client = client;
        this.logger = logger;
        this.activityService = activityService;
        this.characterService = characterService;

        client.SelectMenuExecuted += HandleSelectionAsync;
        client.ButtonExecuted += HandleButtonAsync;
    }

    public abstract string CommandName { get; }

    public abstract Task InstallAsync(SocketGuild guild);

    public abstract Task HandleAsync(SocketSlashCommand command);

    public async Task<bool> ShouldExecuteAsync(SocketSlashCommand command)
    {
        //TODO save char, dungeon and guild for commands
        var requireName =
            (RequireChannelNameAttribute) Attribute.GetCustomAttribute(GetType(), typeof(RequireChannelNameAttribute))!;
        var requireChar =
            (RequireCharacterAttribute) Attribute.GetCustomAttribute(GetType(), typeof(RequireCharacterAttribute))!;
        var requireNoActivity =
            (RequireNoCurrentActivityAttribute) Attribute.GetCustomAttribute(GetType(),
                typeof(RequireNoCurrentActivityAttribute))!;

        if (requireName is not null)
        {
            if (command.Channel.Name != requireName.ChannelName)
            {
                await command.RespondAsync($"This command can only be used in the {requireName.ChannelName} channel!");
                return false;
            }
        }

        Character character = null;
        if (requireChar is not null)
        {
            var user = command.User as IGuildUser;
            var charResult = await characterService.GetCharacterAsync(user.Id, user.Guild.Id);
            if (!charResult.WasSuccessful)
            {
                await command.RespondAsync("Please create a character first!");
                return false;
            }

            character = charResult.Value;
        }

        if (requireNoActivity is not null)
        {
            var currentActivityResult =
                await activityService.GetCharacterActivityAsync(character.ID);
            if (currentActivityResult.WasSuccessful)
            {
                var timeLeft = ((currentActivityResult.Value.StartTime + currentActivityResult.Value.Duration) -
                                DateTime.UtcNow);
                await command.RespondAsync($"You're already on an adventure, try again in {timeLeft.Minutes} minutes!");
                return false;
            }
        }

        return true;
    }

    protected virtual Task HandleSelection(SocketMessageComponent component, string id)
    {
        return Task.CompletedTask;
    }

    protected virtual Task HandleButton(SocketMessageComponent component, string id)
    {
        return Task.CompletedTask;
    }

    private async Task HandleSelectionAsync(SocketMessageComponent component)
    {
        var idSplit = component.Data.CustomId.Split('.');
        if (idSplit[0] != CommandName)
            return;

        await HandleSelection(component, idSplit[1]);
    }

    private async Task HandleButtonAsync(SocketMessageComponent component)
    {
        var idSplit = component.Data.CustomId.Split('.');
        if (idSplit[0] != CommandName)
            return;

        await HandleButton(component, idSplit[1]);
    }
}