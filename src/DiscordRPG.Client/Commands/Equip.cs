using Discord;
using Discord.WebSocket;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Client.Commands.Attributes;
using DiscordRPG.Client.Commands.Base;
using DiscordRPG.Client.Commands.Helpers;
using DiscordRPG.Client.Dialogs;
using DiscordRPG.Common.Extensions;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Character;
using DiscordRPG.Domain.Entities.Character.Enums;
using Humanizer;
using Serilog;

namespace DiscordRPG.Client.Commands;

[RequireCharacter]
[RequireGuild]
public class Equip : DialogCommandBase<EquipDialog>
{
    public Equip(DiscordSocketClient client, ILogger logger, IActivityService activityService,
        ICharacterService characterService, IDungeonService dungeonService, IGuildService guildService,
        IShopService shopService) : base(client,
        logger, activityService, characterService, dungeonService, guildService, shopService)
    {
    }

    public override string CommandName => "equip";

    public override async Task InstallAsync(SocketGuild guild)
    {
        try
        {
            var command = new SlashCommandBuilder()
                .WithName(CommandName)
                .WithDescription("Display or change your current equipment");

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
        dialog.GuildId = new GuildId(context.Guild.Id);

        var embed = GetDisplayEmbeds(dialog);
        var menu = GetMenu(dialog);


        await command.RespondAsync(embeds: embed, component: menu, ephemeral: true);
    }

    private MessageComponent GetMenu(EquipDialog dialog)
    {
        if (dialog.Position is null)
        {
            var positionSelect = new SelectMenuBuilder()
                .WithPlaceholder("Choose equipment type")
                .WithCustomId(GetCommandId("select-position"));
            foreach (var pos in Enum.GetValues<EquipmentPosition>())
            {
                positionSelect.AddOption(pos.ToString(), pos.ToString());
            }

            return new ComponentBuilder()
                .WithButton("Share", GetCommandId("share"))
                .WithButton("Close", GetCommandId("cancel"), ButtonStyle.Secondary)
                .WithSelectMenu(positionSelect)
                .Build();
        }

        var position = dialog.Position;
        var items = dialog.Character.Inventory.Where(i =>
            i.Position == position && i.ItemType is ItemType.Equipment or ItemType.Weapon);

        var menuBuilder = new SelectMenuBuilder()
            .WithPlaceholder($"Choose a new {dialog.Position}")
            .WithCustomId(GetCommandId("select-item"));
        var displayItems = items.Skip((dialog.CurrentPage - 1) * 10)
            .Take(10);

        foreach (var i in displayItems)
        {
            menuBuilder.AddOption(i.ToString(), i.Id.Value);
        }

        var messageComponentBuilder = new ComponentBuilder();
        if (displayItems.Any()) messageComponentBuilder.WithSelectMenu(menuBuilder);

        if (dialog.CurrentItem?.Id.Value != dialog.Character.Inventory
                .FirstOrDefault(i => i.Position == dialog.Position && i.IsEquipped)?.Id.Value)
        {
            messageComponentBuilder.WithButton("Equip", GetCommandId("equip"), ButtonStyle.Success,
                disabled: dialog.CurrentItem == null);
        }
        else
        {
            messageComponentBuilder.WithButton("Unequip", GetCommandId("unequip"), ButtonStyle.Success,
                disabled: dialog.CurrentItem == null);
        }

        return messageComponentBuilder
            .WithButton("Share", GetCommandId("share"), disabled: dialog.CurrentItem == null)
            .WithButton("Back", GetCommandId("back"), ButtonStyle.Secondary)
            .WithButton("Cancel", GetCommandId("cancel"), ButtonStyle.Danger)
            .WithButton("<", GetCommandId("prev-page"), ButtonStyle.Secondary, disabled: dialog.CurrentPage <= 0,
                row: 1)
            .WithButton(">", GetCommandId("next-page"), ButtonStyle.Secondary, row: 1)
            .Build();
    }

    private Embed[] GetDisplayEmbeds(EquipDialog dialog)
    {
        if (dialog.Position is null)
        {
            var embed = new EmbedBuilder()
                .WithTitle($"{dialog.Character.Name}'s equipment")
                .WithColor(Color.Green)
                .AddField("Weapon", dialog.Character.Weapon?.ToString() ?? "Nothing equipped")
                .AddField("Helmet", dialog.Character.Helmet?.ToString() ?? "Nothing equipped")
                .AddField("Armor", dialog.Character.TorsoArmor?.ToString() ?? "Nothing equipped")
                .AddField("Pants", dialog.Character.Pants?.ToString() ?? "Nothing equipped")
                .AddField("Amulet", dialog.Character.Amulet?.ToString() ?? "Nothing equipped")
                .AddField("Ring", dialog.Character.Ring?.ToString() ?? "Nothing equipped")
                .AddField("\u200B", "\u200B")
                .AddField("Total Damage",
                    $"{dialog.Character.TotalDamage.Value} {dialog.Character.TotalDamage.DamageType}", true)
                .AddField("Total Armor", dialog.Character.Armor, true)
                .AddField("Total Magic Armor", dialog.Character.MagicArmor, true)
                .Build();

            dialog.ShareableEmbed = embed;
            var effects = dialog.Character.GetCurrentStatusEffects();
            if (effects.Any())
            {
                var statusEmbedBuilder = new EmbedBuilder();
                statusEmbedBuilder.WithTitle("Status effects");
                foreach (var effect in effects)
                {
                    statusEmbedBuilder.AddField(effect.Name,
                        $"{effect.StatusEffectType.Humanize()} ({effect.Modifier * 100}%)",
                        true);
                }

                return new[] {embed, statusEmbedBuilder.Build()};
            }

            return new[] {embed};
        }

        var comparisonItem = dialog.Character.Inventory.Where(i => i.IsEquipped)
            .FirstOrDefault(i => i.Position == dialog.CurrentItem.Position);
        var itemEmbed = EmbedHelper.GetItemAsEmbed(dialog.CurrentItem, comparison: comparisonItem);
        dialog.ShareableEmbed = itemEmbed;

        return new[] {itemEmbed};
    }

    [Handler("select-position")]
    public async Task HandleSelectPosition(SocketMessageComponent component, EquipDialog dialog)
    {
        var position = Enum.Parse<EquipmentPosition>(component.Data.Values.FirstOrDefault());
        var equipped = dialog.Character.Inventory.FirstOrDefault(i => i.IsEquipped && i.Position == position);
        dialog.CurrentItem = equipped;
        dialog.Position = position;

        var menu = GetMenu(dialog);
        var embed = GetDisplayEmbeds(dialog);

        await component.UpdateAsync(properties =>
        {
            properties.Embeds = embed;
            properties.Components = menu;
        });
    }

    [Handler("select-item")]
    public async Task HandleSelect(SocketMessageComponent component, EquipDialog dialog)
    {
        var id = component.Data.Values.FirstOrDefault();
        var item = dialog.Character.Inventory.FirstOrDefault(i => i.Id.Value == id);

        dialog.CurrentItem = item;

        var menu = GetMenu(dialog);
        var embed = GetDisplayEmbeds(dialog);

        await component.UpdateAsync(properties =>
        {
            properties.Embeds = embed;
            properties.Components = menu;
        });
    }

    [Handler("back")]
    public async Task HandleBack(SocketMessageComponent component, EquipDialog dialog)
    {
        dialog.Position = null;
        dialog.CurrentItem = null;
        var menu = GetMenu(dialog);
        var embed = GetDisplayEmbeds(dialog);

        await component.UpdateAsync(properties =>
        {
            properties.Embeds = embed;
            properties.Components = menu;
        });
    }

    [Handler("unequip")]
    public async Task HandleUnequip(SocketMessageComponent component, EquipDialog dialog)
    {
        var result = await characterService.UnequipItemAsync(dialog.GuildId, new CharacterId(dialog.Character.Id),
            dialog.CurrentItem.Id, dialog.Context);
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

        var updatedChar =
            await characterService.GetCharacterAsync(new CharacterId(dialog.Character.Id), dialog.Context);
        dialog.Character = updatedChar.Value;

        dialog.Position = null;
        dialog.CurrentItem = null;

        var menu = GetMenu(dialog);
        var embed = GetDisplayEmbeds(dialog);

        await component.UpdateAsync(properties =>
        {
            properties.Embeds = embed;
            properties.Components = menu;
        });
    }

    [Handler("equip")]
    public async Task HandleEquip(SocketMessageComponent component, EquipDialog dialog)
    {
        var result = await characterService.EquipItemAsync(dialog.GuildId, new CharacterId(dialog.Character.Id),
            dialog.CurrentItem.Id, dialog.Context);
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

        var updatedChar =
            await characterService.GetCharacterAsync(new CharacterId(dialog.Character.Id), dialog.Context);
        dialog.Character = updatedChar.Value;

        dialog.Position = null;
        dialog.CurrentItem = null;

        var menu = GetMenu(dialog);
        var embed = GetDisplayEmbeds(dialog);

        await component.UpdateAsync(properties =>
        {
            properties.Embeds = embed;
            properties.Components = menu;
        });
    }

    [Handler("prev-page")]
    public async Task HandlePrevPage(SocketMessageComponent component, EquipDialog dialog)
    {
        if (dialog.CurrentPage >= 0)
            dialog.CurrentPage--;

        var menu = GetMenu(dialog);
        var embed = GetDisplayEmbeds(dialog);

        await component.UpdateAsync(properties =>
        {
            properties.Embeds = embed;
            properties.Components = menu;
        });
    }

    [Handler("next-page")]
    public async Task HandleNextPage(SocketMessageComponent component, EquipDialog dialog)
    {
        dialog.CurrentPage++;
        var menu = GetMenu(dialog);
        var embed = GetDisplayEmbeds(dialog);

        await component.UpdateAsync(properties =>
        {
            properties.Embeds = embed;
            properties.Components = menu;
        });
    }
}