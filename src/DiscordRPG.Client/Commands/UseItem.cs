using Discord;
using Discord.WebSocket;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Client.Commands.Attributes;
using DiscordRPG.Client.Commands.Base;
using DiscordRPG.Client.Dialogs;
using DiscordRPG.Common.Extensions;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Character;
using DiscordRPG.Domain.Entities.Character.Enums;
using DiscordRPG.Domain.Entities.Character.ValueObjects;
using Serilog;

namespace DiscordRPG.Client.Commands;

[RequireGuild]
[RequireCharacter]
public class UseItem : DialogCommandBase<UseItemDialog>
{
    public UseItem(DiscordSocketClient client, ILogger logger, IActivityService activityService,
        ICharacterService characterService, IDungeonService dungeonService, IGuildService guildService,
        IShopService shopService) : base(client,
        logger, activityService, characterService, dungeonService, guildService, shopService)
    {
    }

    public override string CommandName => "use-item";

    public override async Task InstallAsync(SocketGuild guild)
    {
        try
        {
            var command = new SlashCommandBuilder()
                .WithName(CommandName)
                .WithDescription("Use an item from your inventory")
                .Build();

            await guild.CreateApplicationCommandAsync(command);
        }
        catch (Exception e)
        {
            logger.Here().Error(e, "Failed to install command {Name}", CommandName);
        }
    }

    protected override async Task HandleDialogAsync(SocketSlashCommand command, GuildCommandContext context,
        UseItemDialog dialog)
    {
        dialog.Character = context.Character;
        dialog.GuildId = new GuildId(context.Guild.Id);
        var menu = GetMenu(dialog);
        var embeds = GetDisplayEmbeds(dialog);

        await command.RespondAsync(string.Empty, embeds, component: menu, ephemeral: true);
    }

    private Embed[] GetDisplayEmbeds(UseItemDialog dialog)
    {
        if (dialog.SelectedItem is null)
            return new[] {new EmbedBuilder().WithDescription("Choose which item you want to use").Build()};

        return new[]
        {
            new EmbedBuilder()
                .WithTitle(dialog.SelectedItem.Name)
                .WithDescription(dialog.SelectedItem.Description)
                .AddField("Amount", dialog.SelectedItem.Amount)
                .Build()
        };
    }

    private MessageComponent GetMenu(UseItemDialog dialog)
    {
        var menuBuilder = new SelectMenuBuilder();
        menuBuilder.WithCustomId(CommandName + ".select-item");
        var pagedConsumables = dialog.Character.Inventory.Where(i => i.ItemType == ItemType.Consumable)
            .Skip((dialog.CurrentPage - 1) * 10).Take(10).OrderByDescending(i => i.Level)
            .ThenByDescending(i => i.Rarity);

        foreach (var item in pagedConsumables)
        {
            menuBuilder.AddOption(item.Name, item.Id.Value, item.Description);
        }

        var componentBuilder = new ComponentBuilder();
        if (menuBuilder.Options.Any())
            componentBuilder.WithSelectMenu(menuBuilder);
        componentBuilder.WithButton("Use Item", GetCommandId("use"),
            disabled: dialog.SelectedItem is null);
        componentBuilder.WithButton("Cancel", GetCommandId("cancel"), ButtonStyle.Secondary);

        return componentBuilder.Build();
    }


    [Handler("select-item")]
    public async Task HandleSelectItem(SocketMessageComponent component, UseItemDialog dialog)
    {
        var id = new ItemId(component.Data.Values.First());
        dialog.SelectedItem = dialog.Character.Inventory.FirstOrDefault(i => i.Id == id);

        var menu = GetMenu(dialog);
        var embeds = GetDisplayEmbeds(dialog);

        await component.UpdateAsync(properties =>
        {
            properties.Components = menu;
            properties.Embeds = embeds;
        });
    }

    [Handler("back")]
    public async Task HandleBack(SocketMessageComponent component, UseItemDialog dialog)
    {
        dialog.SelectedItem = null;

        var menu = GetMenu(dialog);
        var embeds = GetDisplayEmbeds(dialog);

        await component.UpdateAsync(properties =>
        {
            properties.Components = menu;
            properties.Embeds = embeds;
            properties.Embed = null;
        });
    }

    [Handler("use")]
    public async Task HandleUseItem(SocketMessageComponent component, UseItemDialog dialog)
    {
        var result = await characterService.UseItemAsync(dialog.GuildId, new CharacterId(dialog.Character.Id),
            dialog.SelectedItem.Id, dialog.Context);
        Embed embed;
        if (!result.WasSuccessful)
            embed = new EmbedBuilder().WithTitle("Something went wrong!").WithDescription(result.ErrorMessage).Build();
        else
            embed = new EmbedBuilder().WithTitle("Success!")
                .WithDescription($"You have successfully used {dialog.SelectedItem.Name}!").Build();

        var character = await characterService.GetCharacterAsync(new CharacterId(dialog.Character.Id), dialog.Context);
        dialog.Character = character.Value;

        var menu = new ComponentBuilder().WithButton("Back", GetCommandId("back"), ButtonStyle.Secondary)
            .WithButton("Close", GetCommandId("cancel"), ButtonStyle.Secondary).Build();

        await component.UpdateAsync(properties =>
        {
            properties.Components = menu;
            properties.Content = null;
            properties.Embeds = null;
            properties.Embed = embed;
        });
    }
}