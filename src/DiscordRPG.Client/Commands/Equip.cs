using Discord;
using Discord.WebSocket;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Client.Commands.Attributes;
using DiscordRPG.Client.Commands.Base;
using DiscordRPG.Client.Commands.Helpers;
using DiscordRPG.Client.Dialogs;
using DiscordRPG.Common.Extensions;
using DiscordRPG.Core.Enums;
using DiscordRPG.Core.ValueObjects;
using Serilog;

namespace DiscordRPG.Client.Commands;

[RequireCharacter]
[RequireGuild]
public class Equip : DialogCommandBase<EquipDialog>
{
    public Equip(DiscordSocketClient client, ILogger logger, IActivityService activityService,
        ICharacterService characterService, IDungeonService dungeonService, IGuildService guildService) : base(client,
        logger, activityService, characterService, dungeonService, guildService)
    {
    }

    public override string CommandName => "equip";

    public override async Task InstallAsync(SocketGuild guild)
    {
        try
        {
            var optionBuilder = new SlashCommandOptionBuilder()
                .WithName("type")
                .WithDescription("Choose which equipment you want to replace")
                .WithType(ApplicationCommandOptionType.Integer)
                .AddChoice("weapon", (int) EquipmentPosition.Weapon)
                .AddChoice("helmet", (int) EquipmentPosition.Helmet)
                .AddChoice("armor", (int) EquipmentPosition.Armor)
                .AddChoice("pants", (int) EquipmentPosition.Pants)
                .AddChoice("amulet", (int) EquipmentPosition.Amulet)
                .AddChoice("ring", (int) EquipmentPosition.Ring);

            var command = new SlashCommandBuilder()
                .WithName(CommandName)
                .WithDescription("Display or change your equipment")
                .AddOption(optionBuilder);

            await guild.CreateApplicationCommandAsync(command.Build());
        }
        catch (Exception e)
        {
            logger.Here().Error(e, "Failed to install command {Name}", CommandName);
        }
    }

    protected override async Task HandleDialogAsync(SocketSlashCommand command, GuildCommandContext context,
        EquipDialog dialog)
    {
        dialog.Character = context.Character;

        var category = command.Data.Options.FirstOrDefault()?.Value;
        if (category is null)
        {
            var equip = context.Character.Equipment;
            var embed = new EmbedBuilder()
                .WithTitle($"{context.Character.CharacterName}'s equipment")
                .WithColor(Color.Green)
                .AddField("Weapon", equip.Weapon?.ToString() ?? "Nothing equipped")
                .AddField("Helmet", equip.Helmet?.ToString() ?? "Nothing equipped")
                .AddField("Armor", equip.Armor?.ToString() ?? "Nothing equipped")
                .AddField("Pants", equip.Pants?.ToString() ?? "Nothing equipped")
                .AddField("Amulet", equip.Amulet?.ToString() ?? "Nothing equipped")
                .AddField("Ring", equip.Ring?.ToString() ?? "Nothing equipped")
                .AddField("\u200B", "\u200B")
                .AddField("Total Damage",
                    $"{context.Character.TotalDamage.Value} {context.Character.TotalDamage.DamageType}", true)
                .AddField("Total Armor", context.Character.Armor, true)
                .AddField("Total Magic Armor", context.Character.MagicArmor, true)
                .Build();

            EndDialog(dialog.UserId);
            await command.RespondAsync(embed: embed, ephemeral: true);
        }
        else
        {
            var value = (long) category;
            var position = (EquipmentPosition) (int) value;
            await HandleChangeEquipment(command, dialog, position);
        }
    }

    private Task HandleChangeEquipment(SocketSlashCommand command, EquipDialog dialog, EquipmentPosition position) =>
        position switch
        {
            EquipmentPosition.Weapon => SelectWeapon(command, dialog),
            _ => SelectEquip(command, dialog, position)
        };

    private async Task SelectEquip(SocketSlashCommand command, EquipDialog dialog, EquipmentPosition position)
    {
        var equipment = dialog.Character.Equipment.CurrentEquipment[position];
        var embed = EmbedHelper.GetEquipAsEmbed(equipment);

        var messageComponent = GetEquipmentSelectionMenu(dialog, equipment, position);

        await command.RespondAsync(embed: embed, component: messageComponent, ephemeral: true);
    }

    private async Task SelectWeapon(SocketSlashCommand command, EquipDialog dialog)
    {
        var weapon = dialog.Character.Equipment.Weapon;
        var embed = EmbedHelper.GetEquipAsEmbed(weapon);

        var messageComponent = GetWeaponSelectionMenu(dialog, weapon);

        await command.RespondAsync(embed: embed, component: messageComponent, ephemeral: true);
    }

    protected override Task HandleSelection(SocketMessageComponent component, string id, EquipDialog dialog) =>
        id switch
        {
            "change-weapon" => HandleChangeWeapon(component, dialog),
            _ => HandleChangeEquipment(component, dialog)
        };

    private async Task HandleChangeWeapon(SocketMessageComponent component, EquipDialog dialog)
    {
        var itemCode = component.Data.Values.First();
        var item = dialog.Character.Inventory.FirstOrDefault(i => i.GetItemCode() == itemCode) as Weapon;

        dialog.Character.Equipment.Weapon = item;
        dialog.CurrentItem = item;

        await component.UpdateAsync(properties =>
        {
            properties.Embed = EmbedHelper.GetEquipAsEmbed(item);
            properties.Components = GetWeaponSelectionMenu(dialog, item);
        });
    }

    private async Task HandleChangeEquipment(SocketMessageComponent component, EquipDialog dialog)
    {
        var itemCode = component.Data.Values.First();
        var item = dialog.Character.Inventory.FirstOrDefault(i => i.GetItemCode() == itemCode) as Equipment;

        switch (item.EquipmentCategory)
        {
            case EquipmentCategory.Helmet:
                dialog.Character.Equipment.Helmet = item;
                break;
            case EquipmentCategory.Armor:
                dialog.Character.Equipment.Armor = item;
                break;
            case EquipmentCategory.Pants:
                dialog.Character.Equipment.Pants = item;
                break;
            case EquipmentCategory.Amulet:
                dialog.Character.Equipment.Amulet = item;
                break;
            case EquipmentCategory.Ring:
                dialog.Character.Equipment.Ring = item;
                break;
        }

        dialog.CurrentItem = item;

        await component.UpdateAsync(properties =>
        {
            properties.Embed = EmbedHelper.GetEquipAsEmbed(item);
            properties.Components = GetEquipmentSelectionMenu(dialog, item);
        });
    }

    protected override Task HandleButton(SocketMessageComponent component, string id, EquipDialog dialog) => id switch
    {
        "close" => HandleClose(component, dialog),
        "equip-weapon" => HandleEquipWeapon(component, dialog),
        "equip-helmet" => HandleEquipEquipment(component, dialog, EquipmentPosition.Helmet),
        "equip-armor" => HandleEquipEquipment(component, dialog, EquipmentPosition.Armor),
        "equip-pants" => HandleEquipEquipment(component, dialog, EquipmentPosition.Pants),
        "equip-amulet" => HandleEquipEquipment(component, dialog, EquipmentPosition.Amulet),
        "equip-ring" => HandleEquipEquipment(component, dialog, EquipmentPosition.Ring),
    };

    private async Task HandleEquipWeapon(SocketMessageComponent component, EquipDialog dialog)
    {
        EndDialog(dialog.UserId);

        var id = dialog.Character.ID;
        var equip = dialog.Character.Equipment;
        var result = await characterService.UpdateEquipmentAsync(id, equip);
        if (!result.WasSuccessful)
        {
            await component.UpdateAsync(properties =>
            {
                properties.Embed = null;
                properties.Components = null;
                properties.Content = $"Something went wrong, please try again!";
            });

            return;
        }

        await component.UpdateAsync(properties =>
        {
            properties.Embed = null;
            properties.Components = null;
            properties.Content = $"You equipped {dialog.CurrentItem.Name} as your new weapon!";
        });
    }

    private async Task HandleEquipEquipment(SocketMessageComponent component, EquipDialog dialog,
        EquipmentPosition position)
    {
        EndDialog(dialog.UserId);

        var id = dialog.Character.ID;
        var equip = dialog.Character.Equipment;
        var result = await characterService.UpdateEquipmentAsync(id, equip);

        if (!result.WasSuccessful)
        {
            await component.UpdateAsync(properties =>
            {
                properties.Embed = null;
                properties.Components = null;
                properties.Content = $"Something went wrong, please try again!!";
            });
        }

        await component.UpdateAsync(properties =>
        {
            properties.Embed = null;
            properties.Components = null;
            properties.Content = $"You equipped {dialog.CurrentItem.Name}!";
        });
    }

    private async Task HandleClose(SocketMessageComponent component, EquipDialog dialog)
    {
        EndDialog(dialog.UserId);
        await component.UpdateAsync(properties =>
        {
            properties.Embed = null;
            properties.Components = null;
            properties.Content = "You closed your inventory";
        });
    }

    private MessageComponent GetEquipmentSelectionMenu(EquipDialog dialog, Equipment equipment = null,
        EquipmentPosition position = default)
    {
        position = equipment?.Position ?? position;
        var equipments =
            dialog.Character.Inventory.Where(
                i => i is Equipment e && e.Position == position && !e.Equals(equipment));

        var menuBuilder = new SelectMenuBuilder()
            .WithPlaceholder($"Choose a new {position}")
            .WithCustomId(CommandName + $".change-{position.ToString().ToLower()}");

        foreach (Equipment w in equipments)
        {
            menuBuilder.AddOption($"[{w.Rarity}] {w.Name} (Lvl: {w.Level} | {w.Worth}$)]", w.GetItemCode());
        }

        var messageComponentBuilder = new ComponentBuilder();
        if (equipments.Any()) messageComponentBuilder.WithSelectMenu(menuBuilder);

        return messageComponentBuilder
            .WithButton("Equip", CommandName + $".equip-{position.ToString().ToLower()}",
                disabled: dialog.CurrentItem == null)
            .WithButton("Close", CommandName + ".close", ButtonStyle.Secondary)
            .Build();
    }

    private MessageComponent GetWeaponSelectionMenu(EquipDialog dialog, Weapon weapon)
    {
        var weapons =
            dialog.Character.Inventory.Where(i =>
                i is Weapon w && w.Level <= dialog.Character.Level.CurrentLevel && !w.Equals(weapon));

        var menuBuilder = new SelectMenuBuilder()
            .WithPlaceholder("Choose a new Weapon")
            .WithCustomId(CommandName + ".change-weapon");
        foreach (Weapon w in weapons)
        {
            menuBuilder.AddOption($"[{w.Rarity}] {w.Name} (Lvl: {w.Level} | {w.Worth}$)]", w.GetItemCode());
        }

        var messageComponentBuilder = new ComponentBuilder();
        if (weapons.Any()) messageComponentBuilder.WithSelectMenu(menuBuilder);

        return messageComponentBuilder
            .WithButton("Equip", CommandName + ".equip-weapon", disabled: dialog.CurrentItem == null)
            .WithButton("Close", CommandName + ".close", ButtonStyle.Secondary)
            .Build();
    }
}