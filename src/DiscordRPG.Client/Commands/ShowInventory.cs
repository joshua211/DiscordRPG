﻿using System.Text;
using Discord;
using Discord.WebSocket;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Client.Commands.Attributes;
using DiscordRPG.Client.Commands.Base;
using DiscordRPG.Client.Dialogs;
using DiscordRPG.Common.Extensions;
using DiscordRPG.Core.Enums;
using DiscordRPG.Core.ValueObjects;
using Serilog;

namespace DiscordRPG.Client.Commands;

[RequireCharacter]
[RequireGuild]
public class ShowInventory : DialogCommandBase<InventoryDialog>
{
    public ShowInventory(DiscordSocketClient client, ILogger logger, IActivityService activityService,
        ICharacterService characterService, IDungeonService dungeonService, IGuildService guildService) : base(client,
        logger, activityService, characterService, dungeonService, guildService)
    {
    }

    public override string CommandName => "inventory";

    public override async Task InstallAsync(SocketGuild guild)
    {
        try
        {
            var command = new SlashCommandBuilder()
                .WithName(CommandName)
                .WithDescription("Open your inventory")
                .Build();

            await guild.CreateApplicationCommandAsync(command);
        }
        catch (Exception e)
        {
            logger.Here().Error(e, "Failed to install command {Name}", CommandName);
        }
    }

    protected override async Task HandleDialogAsync(SocketSlashCommand command, GuildCommandContext context,
        InventoryDialog dialog)
    {
        dialog.Character = context.Character;
        dialog.CurrentPage = 1;

        var component = GetMenu(dialog);
        var text = GetDisplayText(dialog);

        await command.RespondAsync(text, component: component, ephemeral: true);
    }

    private MessageComponent GetMenu(InventoryDialog dialog)
    {
        var menuBuilder = new SelectMenuBuilder()
            .WithPlaceholder("Filter Category")
            .WithCustomId(GetCommandId("select-category"))
            .AddOption("Items", "items")
            .AddOption("Weapons", "weapons")
            .AddOption("Helmets", "helmets")
            .AddOption("Armor", "armor")
            .AddOption("Pants", "pants")
            .AddOption("Rings", "rings")
            .AddOption("Amulets", "amulets");

        return new ComponentBuilder()
            .WithSelectMenu(menuBuilder)
            .WithButton("<", GetCommandId("prev-page"), disabled: dialog.CurrentPage <= 1)
            .WithButton(">", GetCommandId("next-page"),
                disabled: dialog.CurrentPage >= dialog.MaxPagesOfCurrentCategory)
            .WithButton("Close Inventory", GetCommandId("cancel"), ButtonStyle.Secondary, row: 3)
            .Build();
    }

    private string GetDisplayText(InventoryDialog dialog)
    {
        var items = dialog.CurrentCategory switch
        {
            "weapons" => dialog.Character.Inventory.Where(i => i is Weapon).OrderByDescending(i => i.Level),
            "helmets" => dialog.Character.Inventory.Where(i => i is Equipment {Position: EquipmentPosition.Helmet})
                .OrderByDescending(i => i.Level),
            "armor" => dialog.Character.Inventory.Where(i => i is Equipment {Position: EquipmentPosition.Armor})
                .OrderByDescending(i => i.Level),
            "pants" => dialog.Character.Inventory.Where(i => i is Equipment {Position: EquipmentPosition.Pants})
                .OrderByDescending(i => i.Level),
            "rings" => dialog.Character.Inventory.Where(i => i is Equipment {Position: EquipmentPosition.Ring})
                .OrderByDescending(i => i.Level),
            "amulets" => dialog.Character.Inventory.Where(i => i is Equipment {Position: EquipmentPosition.Amulet})
                .OrderByDescending(i => i.Level),
            _ => dialog.Character.Inventory.Where(i => i is not Equipment).OrderByDescending(i => i.Level)
        };

        dialog.MaxPagesOfCurrentCategory = items.Count() / 10;
        if (items.Count() % 10 != 0)
            dialog.MaxPagesOfCurrentCategory++;

        var sb = new StringBuilder();
        foreach (var item in items.Skip((dialog.CurrentPage - 1) * 10).Take(10))
        {
            if (item is Equipment e)
            {
                sb.Append(
                    $"[{item.Rarity.ToString()}] {item.Name} (Lvl: {item.Level} | {item.Worth}$) Armor: {e.Armor} MArmor: {e.MagicArmor} ");
                sb.Append('(');
                if (e.Strength > 0)
                    sb.Append($"STR: {e.Strength} ");
                if (e.Vitality > 0)
                    sb.Append($"VIT: {e.Vitality} ");
                if (e.Agility > 0)
                    sb.Append($"AGI: {e.Agility} ");
                if (e.Intelligence > 0)
                    sb.Append($"INT: {e.Intelligence} ");
                sb.Append(')');
                sb.Append('\n');
            }
            else if (item is Weapon w)
            {
                sb.Append(
                    $"[{item.Rarity.ToString()}] {item.Name} (Lvl: {item.Level} | {item.Worth}$) Dmg: {w.DamageValue} {w.DamageType} ");
                sb.Append('(');
                if (w.Strength > 0)
                    sb.Append($"STR: {w.Strength} ");
                if (w.Vitality > 0)
                    sb.Append($"VIT: {w.Vitality} ");
                if (w.Agility > 0)
                    sb.Append($"AGI: {w.Agility} ");
                if (w.Intelligence > 0)
                    sb.Append($"INT: {w.Intelligence} ");
                sb.Append(')');
                sb.Append('\n');
            }
            else
                sb.AppendLine(
                    $"[{item.Rarity.ToString()}] {item.Name} (Lvl: {item.Level} | {item.Worth}$) x {item.Amount}");
        }

        if (sb.Length == 0)
            sb.AppendLine("Nothing to show here");

        return sb.ToString();
    }

    [Handler("select-category")]
    public async Task HandleCategory(SocketMessageComponent component, InventoryDialog dialog)
    {
        var category = component.Data.Values.FirstOrDefault();
        dialog.CurrentCategory = category;

        var menu = GetMenu(dialog);
        var text = GetDisplayText(dialog);

        await component.UpdateAsync(properties =>
        {
            properties.Components = menu;
            properties.Content = text;
        });
    }


    [Handler("next-page")]
    public async Task HandleNextPage(SocketMessageComponent component, InventoryDialog dialog)
    {
        dialog.CurrentPage++;
        var menu = GetMenu(dialog);
        var text = GetDisplayText(dialog);

        await component.UpdateAsync(properties =>
        {
            properties.Components = menu;
            properties.Content = text;
        });
    }

    [Handler("prev-page")]
    public async Task HandlePrevPage(SocketMessageComponent component, InventoryDialog dialog)
    {
        dialog.CurrentPage--;
        var menu = GetMenu(dialog);
        var text = GetDisplayText(dialog);

        await component.UpdateAsync(properties =>
        {
            properties.Components = menu;
            properties.Content = text;
        });
    }
}