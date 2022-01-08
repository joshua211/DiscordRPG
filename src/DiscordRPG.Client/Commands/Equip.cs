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

            dialog.ShareableEmbed = embed;
            var component = new ComponentBuilder().WithButton("Share", GetCommandId("share"))
                .WithButton("Close", GetCommandId("cancel"), ButtonStyle.Secondary).Build();
            await command.RespondAsync(embed: embed, component: component, ephemeral: true);
        }
        else
        {
            var value = (long) category;
            dialog.Position = (EquipmentPosition) (int) value;
            await HandleSelectEquipment(command, dialog);
        }
    }

    private async Task HandleSelectEquipment(SocketSlashCommand command, EquipDialog dialog)
    {
        var embed = GetDisplayEmbed(dialog);
        var menu = GetMenu(dialog);

        await command.RespondAsync(embed: embed, component: menu, ephemeral: true);
    }

    private Embed GetDisplayEmbed(EquipDialog dialog)
    {
        var embed = dialog.CurrentItem is null
            ? new EmbedBuilder().WithDescription("Choose your new equipment").Build()
            : EmbedHelper.GetItemAsEmbed(dialog.CurrentItem,
                comparison: dialog.Character.Equipment.CurrentEquipment[dialog.Position]);

        dialog.ShareableEmbed = embed;
        return embed;
    }

    [Handler("select")]
    public async Task HandleSelect(SocketMessageComponent component, EquipDialog dialog)
    {
        var itemCode = component.Data.Values.FirstOrDefault();
        var item = dialog.Character.Inventory.FirstOrDefault(i => i.GetItemCode() == itemCode);

        dialog.CurrentItem = item;

        var menu = GetMenu(dialog);
        var embed = GetDisplayEmbed(dialog);

        await component.UpdateAsync(properties =>
        {
            properties.Embed = embed;
            properties.Components = menu;
        });
    }

    [Handler("equip")]
    public async Task HandleEquip(SocketMessageComponent component, EquipDialog dialog)
    {
        EndDialog(dialog.UserId);

        var id = dialog.Character.ID;
        var equip = dialog.Character.Equipment;
        equip.CurrentEquipment[dialog.Position] = (Equipment) dialog.CurrentItem;

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
            properties.Content = $"You've equipped {dialog.CurrentItem.Name}!";
        });
    }

    private MessageComponent GetMenu(EquipDialog dialog)
    {
        var current = dialog.Character.Equipment.CurrentEquipment[dialog.Position];
        var items = dialog.Character.Inventory.Where(i =>
            i is Equipment e && e.Position == dialog.Position && !e.Equals(current));

        var menuBuilder = new SelectMenuBuilder()
            .WithPlaceholder($"Choose a new {dialog.Position}")
            .WithCustomId(GetCommandId("select"));
        var displayItems = items.Skip((dialog.CurrentPage - 1) * 10)
            .Take(10);

        foreach (Equipment w in displayItems)
        {
            menuBuilder.AddOption($"[{w.Rarity}] {w.Name} (Lvl: {w.Level} | {w.Worth}$)]", w.GetItemCode());
        }

        var messageComponentBuilder = new ComponentBuilder();
        if (displayItems.Any()) messageComponentBuilder.WithSelectMenu(menuBuilder);

        return messageComponentBuilder
            .WithButton("Equip", GetCommandId("equip"),
                disabled: dialog.CurrentItem == null)
            .WithButton("<", GetCommandId("prev-page"), ButtonStyle.Secondary, disabled: dialog.CurrentPage <= 0)
            .WithButton(">", GetCommandId("next-page"), ButtonStyle.Secondary)
            .WithButton("Share", GetCommandId("share"), disabled: dialog.CurrentItem == null)
            .WithButton("Cancel", GetCommandId("cancel"), ButtonStyle.Secondary)
            .Build();
    }

    [Handler("prev-page")]
    public async Task HandlePrevPage(SocketMessageComponent component, EquipDialog dialog)
    {
        if (dialog.CurrentPage >= 0)
            dialog.CurrentPage--;

        var menu = GetMenu(dialog);
        var embed = GetDisplayEmbed(dialog);

        await component.UpdateAsync(properties =>
        {
            properties.Embed = embed;
            properties.Components = menu;
        });
    }

    [Handler("next-page")]
    public async Task HandleNextPage(SocketMessageComponent component, EquipDialog dialog)
    {
        dialog.CurrentPage++;
        var menu = GetMenu(dialog);
        var embed = GetDisplayEmbed(dialog);

        await component.UpdateAsync(properties =>
        {
            properties.Embed = embed;
            properties.Components = menu;
        });
    }
}