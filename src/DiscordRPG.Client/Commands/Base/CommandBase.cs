using System.Reflection;
using Discord;
using Discord.WebSocket;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Application.Models;
using DiscordRPG.Client.Commands.Attributes;
using DiscordRPG.Common;
using DiscordRPG.Common.Extensions;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Character;
using DiscordRPG.Domain.Entities.Dungeon;
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
    protected readonly IShopService shopService;

    protected CommandBase(DiscordSocketClient client, ILogger logger, IActivityService activityService,
        ICharacterService characterService, IDungeonService dungeonService, IGuildService guildService,
        IShopService shopService)
    {
        this.client = client;
        this.logger = logger.WithContext(GetType());
        this.activityService = activityService;
        this.characterService = characterService;
        this.dungeonService = dungeonService;
        this.guildService = guildService;
        this.shopService = shopService;

        client.SelectMenuExecuted += c => GetHandlerAsync(c, true);
        client.ButtonExecuted += c => GetHandlerAsync(c);
    }

    public abstract string CommandName { get; }

    public abstract Task InstallAsync(SocketGuild guild);

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
        var requireShop =
            (RequireShopAttribute) Attribute.GetCustomAttribute(GetType(), typeof(RequireShopAttribute))!;

        if (requireName is not null)
        {
            if (command.Channel.Name != requireName.ChannelName)
            {
                await command.RespondAsync($"This command can only be used in the {requireName.ChannelName} channel!",
                    ephemeral: true);
                return;
            }
        }

        var context = TransactionContext.New();

        GuildReadModel guild = null;
        if (requireGuild is not null)
        {
            var channel = command.Channel as IGuildChannel;
            var guildResult = await guildService.GetGuildAsync(new GuildId(channel.Guild.Id.ToString()), context);
            if (guildResult.Value is null)
            {
                await command.RespondAsync("No Guild was setup for this server!", ephemeral: true);
                return;
            }

            guild = guildResult.Value;
        }

        DungeonReadModel dungeon = null;
        if (requireDungeon is not null)
        {
            var dungeonResult =
                await dungeonService.GetDungeonAsync(new DungeonId(command.Channel.Id.ToString()), context);
            if (dungeonResult.Value is null)
            {
                await command.RespondAsync("This command can only be used in a dungeon!", ephemeral: true);
                return;
            }

            dungeon = dungeonResult.Value;
        }

        CharacterReadModel character = null;
        if (requireChar is not null)
        {
            var user = command.User as IGuildUser;
            var charResult = await characterService.GetCharacterAsync(new CharacterId(user.Id.ToString()), context);
            if (charResult.Value is null)
            {
                await command.RespondAsync("Please create a character first!", ephemeral: true);
                return;
            }

            character = charResult.Value;
        }


        if (requireNoActivity is not null)
        {
            var currentActivityResult =
                await activityService.GetCharacterActivityAsync(new CharacterId(character.Id), context);
            if (currentActivityResult.Value is not null)
            {
                var timeLeft = ((currentActivityResult.Value.StartTime +
                                 TimeSpan.FromMinutes((int) currentActivityResult.Value.Duration)) -
                                DateTime.UtcNow);
                await command.RespondAsync(
                    $"You're already on an adventure, try again in {(int) timeLeft.TotalMinutes} minutes!",
                    ephemeral: true);
                return;
            }
        }

        ActivityReadModel activity = null;
        if (requireActivity is not null)
        {
            var currentActivityResult =
                await activityService.GetCharacterActivityAsync(new CharacterId(character.Id), context);
            if (currentActivityResult.Value is null)
            {
                await command.RespondAsync($"You are currently not doing anything", ephemeral: true);
                return;
            }

            activity = currentActivityResult.Value;
        }

        ShopReadModel shop = null;
        if (requireShop is not null)
        {
            var shopResult = await shopService.GetGuildShopAsync(new GuildId(guild.Id), context);
            if (shopResult.Value is null)
            {
                await command.RespondAsync($"There is no shop for this guild!", ephemeral: true);
                return;
            }

            shop = shopResult.Value;
        }

        await HandleAsync(command, new GuildCommandContext(character, activity, dungeon, guild, context, shop));
    }

    private async Task GetHandlerAsync(SocketMessageComponent component, bool isSelection = false)
    {
        var idSplit = component.Data.CustomId.Split('.');
        if (idSplit[0] != CommandName)
            return;

        var methodInfo = GetType().GetMethods()
            .FirstOrDefault(m =>
            {
                var attr = m.GetCustomAttribute(typeof(HandlerAttribute), true) as HandlerAttribute;
                return attr is not null && attr.Id == idSplit[1];
            });

        if (methodInfo is null)
            return;

        if (isSelection)
            await HandleSelectionAsync(component, methodInfo);
        else
            await HandleButtonAsync(component, methodInfo);
    }

    protected virtual async Task HandleButtonAsync(SocketMessageComponent component, MethodInfo method)
    {
        await (Task) method.Invoke(this, new object[] {component});
    }

    protected virtual async Task HandleSelectionAsync(SocketMessageComponent component, MethodInfo method)
    {
        await (Task) method.Invoke(this, new object[] {component});
    }

    public string GetCommandId(string idName) => $"{CommandName}.{idName}";

    protected abstract Task HandleAsync(SocketSlashCommand command, GuildCommandContext context);
}