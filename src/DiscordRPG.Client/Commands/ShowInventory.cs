using System.Text;
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

        var component = GetDialogComponent(dialog);

        await command.RespondAsync("Your inventory", component: component, ephemeral: true);
    }

    private MessageComponent GetDialogComponent(InventoryDialog dialog)
    {
        var menuBuilder = new SelectMenuBuilder()
            .WithPlaceholder("Filter Category")
            .WithCustomId(CommandName + ".category")
            .AddOption("Items", "items")
            .AddOption("Weapons", "weapons")
            .AddOption("Helmets", "helmets")
            .AddOption("Armor", "armor")
            .AddOption("Pants", "pants")
            .AddOption("Rings", "rings")
            .AddOption("Amulets", "amulets");

        return new ComponentBuilder()
            .WithSelectMenu(menuBuilder)
            .WithButton("<", CommandName + ".prev", disabled: dialog.CurrentPage <= 1)
            .WithButton(">", CommandName + ".next", disabled: dialog.CurrentPage >= dialog.MaxPagesOfCurrentCategory)
            .WithButton("Close Inventory", CommandName + ".close", ButtonStyle.Secondary, row: 3)
            .Build();
    }

    protected override Task HandleSelection(SocketMessageComponent component, string id, InventoryDialog dialog) =>
        id switch
        {
            "category" => HandleCategory(component, dialog, component.Data.Values.FirstOrDefault()),
            _ => Task.CompletedTask,
        };

    private async Task HandleCategory(SocketMessageComponent component, InventoryDialog dialog, string category)
    {
        dialog.CurrentCategory = category;

        var task = category switch
        {
            "items" => ShowItems(component, dialog),
            "weapons" => ShowWeapons(component, dialog),
            "helmets" => ShowEquipment(component, dialog, EquipmentCategory.Helmet),
            "armor" => ShowEquipment(component, dialog, EquipmentCategory.Armor),
            "pants" => ShowEquipment(component, dialog, EquipmentCategory.Pants),
            "rings" => ShowEquipment(component, dialog, EquipmentCategory.Ring),
            "amulets" => ShowEquipment(component, dialog, EquipmentCategory.Amulet),
            _ => Task.CompletedTask
        };

        await task;
    }

    private async Task ShowItems(SocketMessageComponent component, InventoryDialog dialog)
    {
        var items = dialog.Character.Inventory.Where(i => i is not Equipment).OrderByDescending(i => i.Level);
        ;

        dialog.MaxPagesOfCurrentCategory = items.Count() / 10;
        if (items.Count() % 10 != 0)
            dialog.MaxPagesOfCurrentCategory++;

        var sb = new StringBuilder();
        foreach (var item in items.Skip((dialog.CurrentPage - 1) * 10).Take(10))
        {
            sb.AppendLine(
                $"[{item.Rarity.ToString()}] {item.Name} (Lvl: {item.Level} | {item.Worth}$) x {item.Amount}");
        }

        if (sb.Length == 0)
            sb.AppendLine("Nothing to show here");

        var messageComponent = GetDialogComponent(dialog);

        await component.UpdateAsync(properties =>
        {
            properties.Components = messageComponent;
            properties.Content = sb.ToString();
        });
    }

    private async Task ShowWeapons(SocketMessageComponent component, InventoryDialog dialog)
    {
        var items = dialog.Character.Inventory.Where(i => i is Weapon).OrderByDescending(i => i.Level);

        dialog.MaxPagesOfCurrentCategory = items.Count() / 10;
        if (items.Count() % 10 != 0)
            dialog.MaxPagesOfCurrentCategory++;

        var sb = new StringBuilder();
        foreach (Weapon item in items.Skip((dialog.CurrentPage - 1) * 10).Take(10))
        {
            sb.Append(
                $"[{item.Rarity.ToString()}] {item.Name} (Lvl: {item.Level} | {item.Worth}$) Dmg: {item.DamageValue} {item.DamageType} ");
            sb.Append('(');
            if (item.Strength > 0)
                sb.Append($"STR: {item.Strength} ");
            if (item.Vitality > 0)
                sb.Append($"VIT: {item.Vitality} ");
            if (item.Agility > 0)
                sb.Append($"AGI: {item.Agility} ");
            if (item.Intelligence > 0)
                sb.Append($"INT: {item.Intelligence} ");
            sb.Append(')');
            sb.Append('\n');
        }

        if (sb.Length == 0)
            sb.AppendLine("Nothing to show here");

        var messageComponent = GetDialogComponent(dialog);

        await component.UpdateAsync(properties =>
        {
            properties.Components = messageComponent;
            properties.Content = sb.ToString();
        });
    }

    private async Task ShowEquipment(SocketMessageComponent component, InventoryDialog dialog,
        EquipmentCategory category)
    {
        var items = dialog.Character.Inventory.Where(i =>
            i is Equipment equipment && equipment.EquipmentCategory == category).OrderByDescending(i => i.Level);
        ;

        dialog.MaxPagesOfCurrentCategory = items.Count() / 10;
        if (items.Count() % 10 != 0)
            dialog.MaxPagesOfCurrentCategory++;

        var sb = new StringBuilder();
        foreach (Equipment item in items.Skip((dialog.CurrentPage - 1) * 10).Take(10))
        {
            sb.Append(
                $"[{item.Rarity.ToString()}] {item.Name} (Lvl: {item.Level} | {item.Worth}$) Armor: {item.Armor} MArmor: {item.MagicArmor} ");
            sb.Append('(');
            if (item.Strength > 0)
                sb.Append($"STR: {item.Strength} ");
            if (item.Vitality > 0)
                sb.Append($"VIT: {item.Vitality} ");
            if (item.Agility > 0)
                sb.Append($"AGI: {item.Agility} ");
            if (item.Intelligence > 0)
                sb.Append($"INT: {item.Intelligence} ");
            sb.Append(')');
            sb.Append('\n');
        }

        var messageComponent = GetDialogComponent(dialog);

        if (sb.Length == 0)
            sb.AppendLine("Nothing to show here");

        await component.UpdateAsync(properties =>
        {
            properties.Components = messageComponent;
            properties.Content = sb.ToString();
        });
    }

    protected override Task HandleButton(SocketMessageComponent component, string id, InventoryDialog dialog) =>
        id switch
        {
            "close" => HandleClose(component, dialog),
            "prev" => HandlePrevPage(component, dialog),
            "next" => HandleNextPage(component, dialog)
        };

    private Task HandleNextPage(SocketMessageComponent component, InventoryDialog dialog)
    {
        dialog.CurrentPage++;
        return HandleCategory(component, dialog, dialog.CurrentCategory);
    }

    private Task HandlePrevPage(SocketMessageComponent component, InventoryDialog dialog)
    {
        dialog.CurrentPage--;
        return HandleCategory(component, dialog, dialog.CurrentCategory);
    }

    private async Task HandleClose(SocketMessageComponent component, InventoryDialog dialog)
    {
        EndDialog(dialog.UserId);
        component.UpdateAsync(properties =>
        {
            properties.Components = null;
            properties.Content = "You closed your inventory";
        });
    }
}