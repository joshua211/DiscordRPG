using Discord;
using Discord.WebSocket;
using DiscordRPG.Application.Generators;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Client.Commands.Attributes;
using DiscordRPG.Client.Commands.Base;
using DiscordRPG.Client.Dialogs;
using DiscordRPG.Common.Extensions;
using DiscordRPG.Domain.Aggregates.Character.Enums;
using DiscordRPG.Domain.Aggregates.Character.ValueObjects;
using DiscordRPG.Domain.Aggregates.Guild;
using Serilog;

namespace DiscordRPG.Client.Commands;

[RequireGuild]
[RequireCharacter]
public class Forge : DialogCommandBase<ForgeDialog>
{
    private readonly ForgeCalculator forgeCalculator;

    public Forge(DiscordSocketClient client, ILogger logger, IActivityService activityService,
        ICharacterService characterService, IDungeonService dungeonService, IGuildService guildService,
        IShopService shopService, ForgeCalculator forgeCalculator) : base(client, logger, activityService,
        characterService, dungeonService,
        guildService, shopService)
    {
        this.forgeCalculator = forgeCalculator;
    }

    public override string CommandName => "forge";

    public override async Task InstallAsync(SocketGuild guild)
    {
        try
        {
            var command = new SlashCommandBuilder()
                .WithName(CommandName)
                .WithDescription("Forge a new item")
                .Build();

            await guild.CreateApplicationCommandAsync(command);
        }
        catch (Exception e)
        {
            logger.Here().Error(e, "Failed to install command {Name}", CommandName);
        }
    }

    protected override async Task HandleDialogAsync(SocketSlashCommand command, GuildCommandContext context,
        ForgeDialog dialog)
    {
        dialog.CharacterId = new CharacterId(context.Character.Id);
        dialog.GuildId = new GuildId(context.Guild.Id);
        dialog.Level = context.Character.Level.CurrentLevel;
        dialog.AvailableItems = context.Character.Inventory.Where(i => i.ItemType == ItemType.Item).ToList();

        var embeds = GetDisplayEmbeds(dialog);
        var menu = GetMenu(dialog);

        await command.RespondAsync(embeds: embeds, component: menu, ephemeral: true);
    }

    [Handler("select-category")]
    public async Task HandleSelectCategory(SocketMessageComponent component, ForgeDialog dialog)
    {
        var cat = (EquipmentCategory) int.Parse(component.Data.Values.First());
        dialog.Category = cat;

        var embeds = GetDisplayEmbeds(dialog);
        var menu = GetMenu(dialog);

        await component.UpdateAsync(properties =>
        {
            properties.Embeds = embeds;
            properties.Components = menu;
        });
    }

    [Handler("select-item")]
    public async Task HandleSelectItem(SocketMessageComponent component, ForgeDialog dialog)
    {
        var id = new ItemId(component.Data.Values.First());
        var item = dialog.AvailableItems.First(i => i.Id == id);
        dialog.SelectedItem = item;

        var embeds = GetDisplayEmbeds(dialog);
        var menu = GetMenu(dialog);

        await component.UpdateAsync(properties =>
        {
            properties.Embeds = embeds;
            properties.Components = menu;
        });
    }

    [Handler("increase")]
    public async Task HandleIncrease(SocketMessageComponent component, ForgeDialog dialog)
    {
        dialog.IncreaseIngredient(dialog.SelectedItem);

        var embeds = GetDisplayEmbeds(dialog);
        var menu = GetMenu(dialog);

        await component.UpdateAsync(properties =>
        {
            properties.Embeds = embeds;
            properties.Components = menu;
        });
    }

    [Handler("decrease")]
    public async Task HandleDecrease(SocketMessageComponent component, ForgeDialog dialog)
    {
        dialog.DecreaseIngredient(dialog.SelectedItem);

        var embeds = GetDisplayEmbeds(dialog);
        var menu = GetMenu(dialog);

        await component.UpdateAsync(properties =>
        {
            properties.Embeds = embeds;
            properties.Components = menu;
        });
    }

    [Handler("prev-page")]
    public async Task HandlePrevPage(SocketMessageComponent component, ForgeDialog dialog)
    {
        if (dialog.CurrentPage > 1)
            dialog.CurrentPage--;

        var embeds = GetDisplayEmbeds(dialog);
        var menu = GetMenu(dialog);

        await component.UpdateAsync(properties =>
        {
            properties.Embeds = embeds;
            properties.Components = menu;
        });
    }

    [Handler("next-page")]
    public async Task HandleNextPage(SocketMessageComponent component, ForgeDialog dialog)
    {
        dialog.CurrentPage++;

        var embeds = GetDisplayEmbeds(dialog);
        var menu = GetMenu(dialog);

        await component.UpdateAsync(properties =>
        {
            properties.Embeds = embeds;
            properties.Components = menu;
        });
    }

    [Handler("back")]
    public async Task HandleBack(SocketMessageComponent component, ForgeDialog dialog)
    {
        dialog.SelectedItem = null;
        dialog.Category = null;
        dialog.Ingredients.Clear();

        var embeds = GetDisplayEmbeds(dialog);
        var menu = GetMenu(dialog);

        await component.UpdateAsync(properties =>
        {
            properties.Embeds = embeds;
            properties.Components = menu;
        });
    }

    [Handler("forge")]
    public async Task HandleForge(SocketMessageComponent component, ForgeDialog dialog)
    {
        var ingredients = dialog.Ingredients.Select(i => (i.item.Id, i.amount)).ToList();
        var result = await characterService.ForgeItemAsync(dialog.GuildId, dialog.CharacterId, dialog.Category.Value,
            dialog.Level,
            ingredients, dialog.Context);

        EndDialog(dialog.UserId);
        if (!result.WasSuccessful)
        {
            await component.UpdateAsync(properties =>
            {
                properties.Embeds = null;
                properties.Components = null;
                properties.Embed = new EmbedBuilder().WithTitle("Something went wrong!")
                    .WithDescription(result.ErrorMessage).WithColor(Color.Red).Build();
            });

            return;
        }

        await component.UpdateAsync(properties =>
        {
            properties.Embeds = null;
            properties.Components = null;
            properties.Embed = new EmbedBuilder().WithTitle("Success!")
                .WithDescription("You have successfully forged a new item!").WithColor(Color.Gold).Build();
        });
    }

    private Embed[] GetDisplayEmbeds(ForgeDialog dialog)
    {
        if (dialog.Category == null)
            return new[] {new EmbedBuilder().WithDescription("Select what kind if item you want to forge").Build()};
        if (!dialog.Ingredients.Any())
            return new[] {new EmbedBuilder().WithDescription("Select up to 20 resources to forge a new item").Build()};

        var currentRecipeBuilder =
            new EmbedBuilder().WithTitle($"Current resources {dialog.Ingredients.Sum(i => i.amount)}/20");
        foreach (var ingredient in dialog.Ingredients)
        {
            currentRecipeBuilder.AddField(ingredient.item.Name, ingredient.amount);
        }

        currentRecipeBuilder.WithFooter($"You need at least 10 resources to forge a new {dialog.Category}");

        var (str, vit, agi, intel) = forgeCalculator.GetStatsFromIngredients(dialog.Ingredients,
            dialog.Category is EquipmentCategory.Amulet or EquipmentCategory.Ring
                ? 1
                : 2);
        var rarity = forgeCalculator.GetRarityFromIngredients(dialog.Ingredients);
        var itemPeekBuilder = new EmbedBuilder().WithTitle("Possible outcome");
        itemPeekBuilder.AddField("Level", dialog.Level);
        itemPeekBuilder.AddField("Rarity", rarity);
        itemPeekBuilder.AddField("Strength", str, true);
        itemPeekBuilder.AddField("Vitality", vit, true);
        itemPeekBuilder.AddField("Agility", agi, true);
        itemPeekBuilder.AddField("Intelligence", intel, true);

        return new[] {currentRecipeBuilder.Build(), itemPeekBuilder.Build()};
    }

    private MessageComponent GetMenu(ForgeDialog dialog)
    {
        if (dialog.Category == null)
        {
            var categorySelectionBuilder = new SelectMenuBuilder();
            categorySelectionBuilder.WithCustomId(GetCommandId("select-category"));
            categorySelectionBuilder.WithPlaceholder("Category");
            foreach (var cat in Enum.GetValues<EquipmentCategory>())
            {
                categorySelectionBuilder.AddOption(cat.ToString(), $"{(int) cat}");
            }

            return new ComponentBuilder()
                .WithSelectMenu(categorySelectionBuilder)
                .WithButton("Back", GetCommandId("back"), ButtonStyle.Secondary)
                .WithButton("Cancel", GetCommandId("cancel"))
                .Build();
        }

        var selectionBuilder = new SelectMenuBuilder();
        selectionBuilder.WithCustomId(GetCommandId("select-item"));
        selectionBuilder.WithPlaceholder(dialog.SelectedItem is null ? "Ingredient" : dialog.SelectedItem.Name);
        foreach (var item in dialog.AvailableItems)
        {
            selectionBuilder.AddOption($"{item.ToString()} x {item.Amount}", item.Id.Value);
        }

        var builder = new ComponentBuilder();
        if (selectionBuilder.Options.Any())
            builder.WithSelectMenu(selectionBuilder);
        builder.WithButton("Forge", GetCommandId("forge"), ButtonStyle.Primary,
            disabled: dialog.Ingredients.Sum(i => i.amount) < 10);
        builder.WithButton("+", GetCommandId("increase"));
        builder.WithButton("-", GetCommandId("decrease"));
        builder.WithButton("Back", GetCommandId("back"), ButtonStyle.Secondary);
        builder.WithButton("Cancel", GetCommandId("cancel"), ButtonStyle.Secondary);
        builder.WithButton("<", GetCommandId("prev-page"), ButtonStyle.Secondary);
        builder.WithButton(">", GetCommandId("next-page"), ButtonStyle.Secondary);

        return builder.Build();
    }
}