using Discord;
using Discord.WebSocket;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Application.Models;
using DiscordRPG.Client.Commands.Attributes;
using DiscordRPG.Client.Commands.Base;
using DiscordRPG.Client.Dialogs;
using DiscordRPG.Common.Extensions;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Character;
using DiscordRPG.Domain.Entities.Character.ValueObjects;
using Serilog;

namespace DiscordRPG.Client.Commands;

[RequireGuild]
[RequireCharacter]
public class Title : DialogCommandBase<TitleDialog>
{
    public Title(DiscordSocketClient client, ILogger logger, IActivityService activityService,
        ICharacterService characterService, IDungeonService dungeonService, IGuildService guildService,
        IShopService shopService) : base(client, logger, activityService, characterService, dungeonService,
        guildService, shopService)
    {
    }

    public override string CommandName => "title";

    public override async Task InstallAsync(SocketGuild guild)
    {
        try
        {
            var command = new SlashCommandBuilder()
                .WithName(CommandName)
                .WithDescription("Select a new title")
                .Build();

            await guild.CreateApplicationCommandAsync(command);
        }
        catch (Exception e)
        {
            logger.Here().Error(e, "Failed to install command {Name}", CommandName);
        }
    }

    protected override async Task HandleDialogAsync(SocketSlashCommand command, GuildCommandContext context,
        TitleDialog dialog)
    {
        dialog.GuildId = new GuildId(context.Guild.Id);
        SetDialog(dialog, context.Character);

        var menu = GetMenu(dialog);
        var embed = GetDisplayEmbed(dialog);

        await command.RespondAsync(embed: embed, component: menu, ephemeral: true);
    }

    [Handler("select-title")]
    public async Task HandleSelectTitle(SocketMessageComponent component, TitleDialog dialog)
    {
        var titleId = new TitleId(component.Data.Values.FirstOrDefault());
        var title = dialog.AllTitles.FirstOrDefault(t => t.Id == titleId);
        dialog.SelectedTitle = title;

        var menu = GetMenu(dialog);
        var embed = GetDisplayEmbed(dialog);

        await component.UpdateAsync(properties =>
        {
            properties.Components = menu;
            properties.Embed = embed;
        });
    }

    [Handler("equip")]
    public async Task HandleEquip(SocketMessageComponent component, TitleDialog dialog)
    {
        var result = await characterService.EquipTitleAsync(dialog.GuildId, dialog.CharacterId, dialog.SelectedTitle.Id,
            dialog.Context);
        if (!result.WasSuccessful)
        {
            await component.UpdateAsync(properties =>
            {
                properties.Components = null;
                properties.Embed = new EmbedBuilder().WithTitle("Something went wrong!")
                    .WithDescription(result.ErrorMessage).WithColor(Color.Red).Build();
            });
            return;
        }

        dialog.CurrentTitle = dialog.SelectedTitle;
        var menu = GetMenu(dialog);
        var embed = GetDisplayEmbed(dialog);

        await component.UpdateAsync(properties =>
        {
            properties.Components = menu;
            properties.Embed = embed;
        });
    }

    [Handler("unequip")]
    public async Task HandleUnequip(SocketMessageComponent component, TitleDialog dialog)
    {
        var result = await characterService.UnequipTitleAsync(dialog.GuildId, dialog.CharacterId,
            dialog.SelectedTitle.Id,
            dialog.Context);
        if (!result.WasSuccessful)
        {
            await component.UpdateAsync(properties =>
            {
                properties.Components = null;
                properties.Embed = new EmbedBuilder().WithTitle("Something went wrong!")
                    .WithDescription(result.ErrorMessage).WithColor(Color.Red).Build();
            });
            return;
        }

        dialog.CurrentTitle = null;
        dialog.SelectedTitle = null;
        var menu = GetMenu(dialog);
        var embed = GetDisplayEmbed(dialog);

        await component.UpdateAsync(properties =>
        {
            properties.Components = menu;
            properties.Embed = embed;
        });
    }

    [Handler("prev-page")]
    public async Task HandlePrevPage(SocketMessageComponent component, TitleDialog dialog)
    {
        if (dialog.CurrentPage > 1)
            dialog.CurrentPage--;

        var menu = GetMenu(dialog);
        var embed = GetDisplayEmbed(dialog);

        await component.UpdateAsync(properties =>
        {
            properties.Components = menu;
            properties.Embed = embed;
        });
    }

    [Handler("next-page")]
    public async Task HandleNextPage(SocketMessageComponent component, TitleDialog dialog)
    {
        dialog.CurrentPage++;

        var menu = GetMenu(dialog);
        var embed = GetDisplayEmbed(dialog);

        await component.UpdateAsync(properties =>
        {
            properties.Components = menu;
            properties.Embed = embed;
        });
    }

    private MessageComponent GetMenu(TitleDialog dialog)
    {
        var selectionBuilder = new SelectMenuBuilder();
        selectionBuilder.WithCustomId(GetCommandId("select-title"));
        selectionBuilder.WithPlaceholder("Select a title");
        var pagedTitles = dialog.AllTitles.Skip((dialog.CurrentPage - 1) * 10).Take(10).OrderBy(t => t.IsEquipped)
            .ThenByDescending(t => t.Rarity);
        foreach (var title in pagedTitles)
        {
            selectionBuilder.AddOption(title.ToString(), title.Id.Value);
        }

        var builder = new ComponentBuilder();
        if (selectionBuilder.Options.Any())
            builder.WithSelectMenu(selectionBuilder);
        if (dialog.SelectedTitle == dialog.CurrentTitle)
            builder.WithButton("Unequip", GetCommandId("unequip"), ButtonStyle.Primary,
                disabled: dialog.SelectedTitle == null);
        else
            builder.WithButton("Equip", GetCommandId("equip"), ButtonStyle.Primary,
                disabled: dialog.SelectedTitle == null);
        builder.WithButton("Cancel", GetCommandId("cancel"), ButtonStyle.Secondary);
        builder.WithButton("<", GetCommandId("prev-page"), ButtonStyle.Secondary);
        builder.WithButton(">", GetCommandId("next-page"), ButtonStyle.Secondary);

        return builder.Build();
    }

    private Embed GetDisplayEmbed(TitleDialog dialog)
    {
        if (dialog.SelectedTitle is null)
            return new EmbedBuilder().WithDescription("Select one of your titles").Build();

        var b = new EmbedBuilder().WithTitle(dialog.SelectedTitle.Name);
        b.AddField("Rarity", dialog.SelectedTitle.Rarity.ToString());
        foreach (var effect in dialog.SelectedTitle.Effects)
        {
            b.AddField(effect.Name, $"{effect.StatusEffectType} {effect.Modifier * 100}%");
        }

        return b.Build();
    }

    private void SetDialog(TitleDialog dialog, CharacterReadModel character)
    {
        dialog.CharacterId = new CharacterId(character.Id);
        dialog.AllTitles = character.Titles;
        dialog.CurrentTitle = character.Title;
        dialog.SelectedTitle = dialog.CurrentTitle;
    }
}