using Discord;
using Discord.WebSocket;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Application.Models;
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
public class Sell : DialogCommandBase<SellDialog>
{
    public Sell(DiscordSocketClient client, ILogger logger, IActivityService activityService,
        ICharacterService characterService, IDungeonService dungeonService, IGuildService guildService,
        IShopService shopService) : base(client,
        logger, activityService, characterService, dungeonService, guildService, shopService)
    {
    }

    public override string CommandName => "sell";

    public override async Task InstallAsync(SocketGuild guild)
    {
        try
        {
            var command = new SlashCommandBuilder()
                .WithName(CommandName)
                .WithDescription("Sell items from your inventory")
                .Build();

            await guild.CreateApplicationCommandAsync(command);
        }
        catch (Exception e)
        {
            logger.Here().Error(e, "Failed to install command {Name}", CommandName);
        }
    }

    protected override async Task HandleDialogAsync(SocketSlashCommand command, GuildCommandContext context,
        SellDialog dialog)
    {
        dialog.GuildId = new GuildId(context.Guild.Id);
        SetDialogCharacter(dialog, context.Character);

        var menu = GetMenu(dialog);
        var embeds = GetDisplayEmbeds(dialog);

        await command.RespondAsync(embeds: embeds, component: menu, ephemeral: true);
    }

    private MessageComponent GetMenu(SellDialog dialog)
    {
        var pagedItems = dialog.SellableItems.Skip((dialog.CurrentPage - 1) * 10).Take(10)
            .OrderByDescending(i => i.ItemType).ThenByDescending(i => i.Level).ThenByDescending(i => i.Rarity);

        var selectionBuilder = new SelectMenuBuilder();
        selectionBuilder.WithCustomId(GetCommandId("select-item"));
        foreach (var item in pagedItems)
        {
            selectionBuilder.AddOption(item.ToString(), item.Id.Value);
        }

        var componentBuilder = new ComponentBuilder();
        if (selectionBuilder.Options.Any())
            componentBuilder.WithSelectMenu(selectionBuilder);
        componentBuilder.WithButton("Sell", GetCommandId("sell"), ButtonStyle.Primary,
            disabled: dialog.SelectedItem is null);
        componentBuilder.WithButton("Cancel", GetCommandId("cancel"), ButtonStyle.Secondary);
        componentBuilder.WithButton("<", GetCommandId("prev-page"), ButtonStyle.Secondary, row: 1,
            disabled: dialog.CurrentPage <= 1);
        componentBuilder.WithButton(">", GetCommandId("next-page"), ButtonStyle.Secondary, row: 1);

        return componentBuilder.Build();
    }

    private Embed[] GetDisplayEmbeds(SellDialog dialog)
    {
        if (dialog.SelectedItem is null)
        {
            return new[]
            {
                EmbedHelper.GetMoneyAsEmbed(dialog.CurrentMoney),
                new EmbedBuilder().WithDescription("Select an item").Build()
            };
        }

        var embed = EmbedHelper.GetItemAsEmbed(dialog.SelectedItem);

        return new[] {EmbedHelper.GetMoneyAsEmbed(dialog.CurrentMoney), embed};
    }

    [Handler("select-item")]
    public async Task HandleSelectItem(SocketMessageComponent component, SellDialog dialog)
    {
        var id = new ItemId(component.Data.Values.First());
        var item = dialog.SellableItems.First(i => i.Id == id);
        dialog.SelectedItem = item;

        var menu = GetMenu(dialog);
        var embeds = GetDisplayEmbeds(dialog);

        await component.UpdateAsync(properties =>
        {
            properties.Embeds = embeds;
            properties.Components = menu;
        });
    }

    [Handler("sell")]
    public async Task HandleSell(SocketMessageComponent component, SellDialog dialog)
    {
        var result = await characterService.SellItemAsync(dialog.GuildId, dialog.CharacterId, dialog.SelectedItem.Id,
            dialog.Context);
        if (!result.WasSuccessful)
        {
            EndDialog(dialog.UserId);
            await component.UpdateAsync(properties =>
            {
                properties.Embeds = null;
                properties.Embed = new EmbedBuilder().WithColor(Color.Orange).WithTitle("Something went wrong!")
                    .WithDescription(result.ErrorMessage).Build();
                properties.Components = null;
            });
        }

        var successEmbed = new EmbedBuilder().WithDescription($"Successfully sold {dialog.SelectedItem.Name}!").Build();
        var character = await characterService.GetCharacterAsync(dialog.CharacterId, dialog.Context);

        SetDialogCharacter(dialog, character.Value);
        dialog.SelectedItem = null;

        var moneyEmbed = EmbedHelper.GetMoneyAsEmbed(dialog.CurrentMoney);
        var menu = GetMenu(dialog);

        await component.UpdateAsync(properties =>
        {
            properties.Components = menu;
            properties.Embeds = new Embed[] {moneyEmbed, successEmbed};
        });
    }

    [Handler("prev-page")]
    public async Task HandlePrevPage(SocketMessageComponent component, SellDialog dialog)
    {
        if (dialog.CurrentPage > 1)
            dialog.CurrentPage--;

        var menu = GetMenu(dialog);
        var embeds = GetDisplayEmbeds(dialog);

        await component.UpdateAsync(properties =>
        {
            properties.Embeds = embeds;
            properties.Components = menu;
        });
    }

    [Handler("next-page")]
    public async Task HandleNextPage(SocketMessageComponent component, SellDialog dialog)
    {
        dialog.CurrentPage++;

        var menu = GetMenu(dialog);
        var embeds = GetDisplayEmbeds(dialog);

        await component.UpdateAsync(properties =>
        {
            properties.Embeds = embeds;
            properties.Components = menu;
        });
    }


    private static void SetDialogCharacter(SellDialog dialog, CharacterReadModel readModel)
    {
        dialog.CharacterId = new CharacterId(readModel.Id);
        dialog.SellableItems = readModel.Inventory.Where(i => !i.IsEquipped).ToList();
        dialog.CurrentMoney = readModel.Money.Value;
    }
}