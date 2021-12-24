﻿using Discord;
using Discord.WebSocket;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Client.Commands.Attributes;
using DiscordRPG.Client.Commands.Base;
using DiscordRPG.Client.Commands.Helpers;
using DiscordRPG.Client.Dialogs;
using DiscordRPG.Common.Extensions;
using DiscordRPG.Core.ValueObjects;
using Serilog;

namespace DiscordRPG.Client.Commands;

[RequireGuild]
[RequireCharacter]
public class Shop : DialogCommandBase<ShopDialog>
{
    private readonly IShopService shopService;

    public Shop(DiscordSocketClient client, ILogger logger, IActivityService activityService,
        ICharacterService characterService, IDungeonService dungeonService, IGuildService guildService,
        IShopService shopService) : base(client,
        logger, activityService, characterService, dungeonService, guildService)
    {
        this.shopService = shopService;
    }

    public override string CommandName => "shop";

    public override async Task InstallAsync(SocketGuild guild)
    {
        try
        {
            var optionBuilder = new SlashCommandOptionBuilder()
                .WithName("interaction")
                .WithDescription("Buy or sell items")
                .WithRequired(true)
                .WithType(ApplicationCommandOptionType.String)
                .AddChoice("buy", "buy")
                .AddChoice("sell", "sell");

            var command = new SlashCommandBuilder()
                .WithName(CommandName)
                .WithDescription("Open the shop")
                .AddOption(optionBuilder);

            await guild.CreateApplicationCommandAsync(command.Build());
        }
        catch (Exception e)
        {
            logger.Here().Error(e, "Failed to install command {Name}", CommandName);
        }
    }

    protected override async Task HandleDialogAsync(SocketSlashCommand command, GuildCommandContext context,
        ShopDialog dialog)
    {
        dialog.Character = context.Character;
        dialog.GuildId = context.Guild.ID;

        var data = command.Data.Options.FirstOrDefault().Value as string;
        switch (data)
        {
            case "buy":
                await HandleBuy(command, dialog);
                break;
            case "sell":
                await HandleSell(command, dialog);
                break;
        }
    }

    private async Task HandleBuy(SocketSlashCommand command, ShopDialog dialog)
    {
        dialog.IsBuying = true;
        var shopResult = await shopService.GetGuildShopAsync(dialog.GuildId);
        if (!shopResult.WasSuccessful)
        {
            EndDialog(dialog.UserId);
            await command.RespondAsync("There is no shop available for this guild!");
            return;
        }

        var playerShop = shopResult.Value[dialog.Character.ID];
        if (playerShop is null || !playerShop.Any())
        {
            EndDialog(dialog.UserId);
            await command.RespondAsync("The shop does not have anything to sell to you!");
            return;
        }

        dialog.GuildShop = shopResult.Value;
        dialog.PlayerShop = playerShop;

        var menu = GetMenu(dialog);
        var embeds = GetDisplayEmbeds(dialog);

        await command.RespondAsync(ephemeral: true, component: menu, embeds: embeds.ToArray());
    }

    private async Task HandleSell(SocketSlashCommand command, ShopDialog dialog)
    {
        dialog.IsBuying = false;
        var menu = GetMenu(dialog);
        var embeds = GetDisplayEmbeds(dialog);

        await command.RespondAsync(ephemeral: true, component: menu, embeds: embeds.ToArray());
    }

    protected override Task HandleSelection(SocketMessageComponent component, string id, ShopDialog dialog) => id switch
    {
        "inspect" => HandleInspectItem(component, dialog)
    };

    private async Task HandleInspectItem(SocketMessageComponent component, ShopDialog dialog)
    {
        var id = component.Data.Values.FirstOrDefault();
        if (dialog.IsBuying)
        {
            var equip = dialog.PlayerShop.FirstOrDefault(e => e.GetItemCode() == id);
            dialog.SelectedItem = equip;
        }
        else
        {
            var item = dialog.Character.Inventory.FirstOrDefault(i => i.GetItemCode() == id);
            dialog.SelectedItem = item;
        }

        var menu = GetMenu(dialog);

        await component.UpdateAsync(properties =>
        {
            properties.Content = null;
            properties.Embeds = GetDisplayEmbeds(dialog).ToArray();
            properties.Components = menu;
        });
    }

    protected override Task HandleButton(SocketMessageComponent component, string id, ShopDialog dialog) => id switch
    {
        "buy" => HandleBuyEquip(component, dialog),
        "sell" => HandleSellItem(component, dialog),
        "cancel" => HandleCancel(component, dialog)
    };

    private async Task HandleSellItem(SocketMessageComponent component, ShopDialog dialog)
    {
        var result = await shopService.SellItemAsync(dialog.Character, dialog.SelectedItem);
        if (!result.WasSuccessful)
        {
            await component.UpdateAsync(properties =>
            {
                properties.Components = null;
                properties.Embeds = null;
                properties.Content = result.ErrorMessage;
            });
            EndDialog(dialog.UserId);

            return;
        }

        dialog.Character = result.Value;
        dialog.SelectedItem = null;

        await component.UpdateAsync(properties =>
        {
            properties.Embeds = GetDisplayEmbeds(dialog).ToArray();
            properties.Components = GetMenu(dialog);
        });
    }

    private async Task HandleCancel(SocketMessageComponent component, ShopDialog dialog)
    {
        EndDialog(dialog.UserId);
        await component.UpdateAsync(properties =>
        {
            properties.Components = null;
            properties.Embeds = null;
            properties.Content = "Maybe another time";
        });
    }

    private async Task HandleBuyEquip(SocketMessageComponent component, ShopDialog dialog)
    {
        var result =
            await shopService.BuyEquipAsync(dialog.GuildShop, dialog.Character, (Equipment) dialog.SelectedItem);
        if (!result.WasSuccessful)
        {
            await component.UpdateAsync(properties =>
            {
                properties.Components = null;
                properties.Embeds = null;
                properties.Content = result.ErrorMessage;
            });
            EndDialog(dialog.UserId);

            return;
        }

        var newPlayerShop = result.Value.Item1[dialog.Character.ID];
        dialog.PlayerShop = newPlayerShop;
        dialog.Character = result.Value.Item2;
        dialog.SelectedItem = null;

        await component.UpdateAsync(properties =>
        {
            properties.Embeds = GetDisplayEmbeds(dialog).ToArray();
            properties.Components = GetMenu(dialog);
        });
    }

    private MessageComponent GetMenu(ShopDialog dialog)
    {
        var selectionBuilder = new SelectMenuBuilder();
        selectionBuilder.WithCustomId(CommandName + ".inspect");
        List<Item> tradeableItems;
        if (dialog.IsBuying)
        {
            tradeableItems = dialog.PlayerShop.Select(e => e as Item).ToList();
            foreach (var equipment in tradeableItems)
            {
                selectionBuilder.AddOption(equipment.ToString(), equipment.GetItemCode());
            }
        }
        else
        {
            tradeableItems = dialog.Character.Inventory
                .Where(i => !dialog.Character.Equipment.CurrentEquipment.Values.Contains(i)).ToList();
            foreach (var item in dialog.Character.Inventory)
            {
                selectionBuilder.AddOption(item.ToString(), item.GetItemCode());
            }
        }

        var componentBuilder = new ComponentBuilder();
        if (tradeableItems.Any())
            componentBuilder.WithSelectMenu(selectionBuilder);

        var label = dialog.IsBuying ? "Buy" : "Sell";
        var id = dialog.IsBuying ? ".buy" : ".sell";

        componentBuilder
            .WithButton(label, CommandName + id, disabled: dialog.SelectedItem is null)
            .WithButton("Cancel", CommandName + ".cancel", ButtonStyle.Secondary);

        return componentBuilder.Build();
    }

    private IEnumerable<Embed> GetDisplayEmbeds(ShopDialog dialog)
    {
        yield return EmbedHelper.GetMoneyAsEmbed(dialog.Character.Money);

        if (dialog.SelectedItem is null)
            yield break;

        yield return EmbedHelper.GetItemAsEmbed(dialog.SelectedItem, dialog.IsBuying ? 1 : 0.7f);
    }
}