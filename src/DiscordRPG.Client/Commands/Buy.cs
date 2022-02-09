using Discord;
using Discord.WebSocket;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Client.Commands.Attributes;
using DiscordRPG.Client.Commands.Base;
using DiscordRPG.Client.Commands.Helpers;
using DiscordRPG.Client.Dialogs;
using DiscordRPG.Common.Extensions;
using DiscordRPG.Domain.Aggregates.Character.ValueObjects;
using DiscordRPG.Domain.Aggregates.Guild;
using Serilog;

namespace DiscordRPG.Client.Commands;

[RequireGuild]
[RequireCharacter]
[RequireShop]
public class Buy : DialogCommandBase<BuyDialog>
{
    public Buy(DiscordSocketClient client, ILogger logger, IActivityService activityService,
        ICharacterService characterService, IDungeonService dungeonService, IGuildService guildService,
        IShopService shopService) : base(client,
        logger, activityService, characterService, dungeonService, guildService, shopService)
    {
    }

    public override string CommandName => "buy";

    public override async Task InstallAsync(SocketGuild guild)
    {
        try
        {
            var command = new SlashCommandBuilder()
                .WithName(CommandName)
                .WithDescription("Buy a new item")
                .Build();

            await guild.CreateApplicationCommandAsync(command);
        }
        catch (Exception e)
        {
            logger.Here().Error(e, "Failed to install command {Name}", CommandName);
        }
    }

    protected override async Task HandleDialogAsync(SocketSlashCommand command, GuildCommandContext context,
        BuyDialog dialog)
    {
        dialog.CharacterId = new CharacterId(context.Character.Id);
        dialog.GuildId = new GuildId(context.Guild.Id);
        dialog.Money = context.Character.Money.Value;
        dialog.AvailableItems =
            context.Shop.Inventory.FirstOrDefault(i => i.CharacterId == dialog.CharacterId).ItemsForSale;
        dialog.EquippedItems = context.Character.Inventory.Where(i => i.IsEquipped);

        var menu = GetMenu(dialog);
        var embeds = GetDisplayEmbeds(dialog);

        await command.RespondAsync(embeds: embeds, component: menu, ephemeral: true);
    }

    [Handler("select-item")]
    public async Task HandleSelectItem(SocketMessageComponent component, BuyDialog dialog)
    {
        var id = new ItemId(component.Data.Values.First());
        var item = dialog.AvailableItems.First(i => i.Id == id);
        dialog.SelectedItem = item;

        var menu = GetMenu(dialog);
        var embeds = GetDisplayEmbeds(dialog);

        await component.UpdateAsync(properties =>
        {
            properties.Components = menu;
            properties.Embeds = embeds;
        });
    }

    [Handler("buy")]
    public async Task HandleBuy(SocketMessageComponent component, BuyDialog dialog)
    {
        var result = await characterService.BuyItemAsync(dialog.GuildId, dialog.CharacterId, dialog.SelectedItem.Id,
            dialog.Context);

        if (!result.WasSuccessful)
        {
            await component.UpdateAsync(properties =>
            {
                properties.Components = null;
                properties.Embeds = null;
                properties.Embed = new EmbedBuilder().WithTitle("Something went wrong!")
                    .WithDescription(result.ErrorMessage).WithColor(Color.Red).Build();
            });
        }

        var characterTask = characterService.GetCharacterAsync(dialog.CharacterId, dialog.Context);
        var shopTask = shopService.GetGuildShopAsync(dialog.GuildId, dialog.Context);
        await Task.WhenAll(characterTask, shopTask).ConfigureAwait(false);

        dialog.Money = characterTask.Result.Value.Money.Value;
        dialog.AvailableItems =
            shopTask.Result.Value.Inventory.First(i => i.CharacterId == dialog.CharacterId).ItemsForSale;

        var menu = GetMenu(dialog);
        var embeds = GetDisplayEmbeds(dialog);

        await component.UpdateAsync(properties =>
        {
            properties.Components = menu;
            properties.Embeds = embeds;
        });
    }

    public MessageComponent GetMenu(BuyDialog dialog)
    {
        var selectionBuilder = new SelectMenuBuilder();
        selectionBuilder.WithCustomId(GetCommandId("select-item"));
        selectionBuilder.WithPlaceholder("Select an item to buy it");
        foreach (var item in dialog.AvailableItems)
        {
            selectionBuilder.AddOption(item.ToString(), item.Id.Value);
        }

        var componentBuilder = new ComponentBuilder();
        if (selectionBuilder.Options.Any())
            componentBuilder.WithSelectMenu(selectionBuilder);
        componentBuilder.WithButton("Buy", GetCommandId("buy"), ButtonStyle.Primary,
            disabled: (dialog.SelectedItem is null || dialog.SelectedItem.Worth > dialog.Money));
        componentBuilder.WithButton("Cancel", GetCommandId("cancel"), ButtonStyle.Secondary);

        return componentBuilder.Build();
    }

    public Embed[] GetDisplayEmbeds(BuyDialog dialog)
    {
        var money = EmbedHelper.GetMoneyAsEmbed(dialog.Money);
        if (dialog.SelectedItem is null)
            return new[] {money};

        var comparisonItem = dialog.EquippedItems.FirstOrDefault(i => i.Position == dialog.SelectedItem.Position);
        return new[] {money, EmbedHelper.GetItemAsEmbed(dialog.SelectedItem, comparison: comparisonItem)};
    }
}