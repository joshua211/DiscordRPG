using Discord;
using Discord.WebSocket;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Client.Commands.Attributes;
using DiscordRPG.Client.Commands.Base;
using DiscordRPG.Client.Dialogs;
using DiscordRPG.Common.Extensions;
using DiscordRPG.Domain.Entities.Character.ValueObjects;
using Serilog;

namespace DiscordRPG.Client.Commands;

[RequireGuild]
[RequireCharacter]
public class ShowRecipes : DialogCommandBase<RecipesDialog>
{
    public ShowRecipes(DiscordSocketClient client, ILogger logger, IActivityService activityService,
        ICharacterService characterService, IDungeonService dungeonService, IGuildService guildService,
        IShopService shopService) : base(client, logger, activityService, characterService, dungeonService,
        guildService, shopService)
    {
    }

    public override string CommandName => "recipes";

    public override async Task InstallAsync(SocketGuild guild)
    {
        try
        {
            var command = new SlashCommandBuilder()
                .WithName(CommandName)
                .WithDescription("Show your known recipes")
                .Build();

            await guild.CreateApplicationCommandAsync(command);
        }
        catch (Exception e)
        {
            logger.Here().Error(e, "Failed to install command {Name}", CommandName);
        }
    }

    protected override async Task HandleDialogAsync(SocketSlashCommand command, GuildCommandContext context,
        RecipesDialog dialog)
    {
        dialog.AllRecipes = context.Character.KnownRecipes;
        dialog.PlayerInventory = context.Character.Inventory;

        var menu = GetMenu(dialog);
        var embed = GetDisplayEmbed(dialog);

        await command.RespondAsync(embed: embed, component: menu, ephemeral: true);
    }

    [Handler("select-recipe")]
    public async Task HandleSelectRecipe(SocketMessageComponent component, RecipesDialog dialog)
    {
        var id = new RecipeId(component.Data.Values.First());
        var recipe = dialog.AllRecipes.First(r => r.Id == id);
        dialog.SelectedRecipe = recipe;

        var menu = GetMenu(dialog);
        var embed = GetDisplayEmbed(dialog);

        await component.UpdateAsync(properties =>
        {
            properties.Embed = embed;
            properties.Components = menu;
        });
    }

    [Handler("prev-page")]
    public async Task HandlePrevPage(SocketMessageComponent component, RecipesDialog dialog)
    {
        if (dialog.CurrentPage > 1)
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
    public async Task HandleNext(SocketMessageComponent component, RecipesDialog dialog)
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

    private MessageComponent GetMenu(RecipesDialog dialog)
    {
        var selectionBuilder = new SelectMenuBuilder();
        selectionBuilder.WithCustomId(GetCommandId("select-recipe"));
        selectionBuilder.WithPlaceholder("Select a recipe");

        var pagedRecipes = dialog.AllRecipes.Skip((dialog.CurrentPage - 1) * 10).Take(10)
            .OrderByDescending(r => r.Level).ThenByDescending(r => r.Rarity);
        foreach (var r in pagedRecipes)
        {
            selectionBuilder.AddOption(r.Name, r.Id.Value);
        }

        var menuBuilder = new ComponentBuilder();
        if (selectionBuilder.Options.Any())
            menuBuilder.WithSelectMenu(selectionBuilder);
        menuBuilder.WithButton("<", GetCommandId("prev-page"), ButtonStyle.Secondary);
        menuBuilder.WithButton(">", GetCommandId("next-page"), ButtonStyle.Secondary);
        menuBuilder.WithButton("Close", GetCommandId("cancel"), ButtonStyle.Secondary);

        return menuBuilder.Build();
    }

    private Embed GetDisplayEmbed(RecipesDialog dialog)
    {
        if (dialog.SelectedRecipe is null)
            return new EmbedBuilder().WithDescription("Select one of your recipes").Build();

        var b = new EmbedBuilder().WithTitle(dialog.SelectedRecipe.Name)
            .WithColor(Color.Teal)
            .WithDescription(dialog.SelectedRecipe.Description).AddField("Level", dialog.SelectedRecipe.Level, true)
            .AddField("Rarity", dialog.SelectedRecipe.Rarity, true);
        foreach (var ingredient in dialog.SelectedRecipe.Ingredients)
        {
            b.AddField($"[{ingredient.Rarity}] {ingredient.Name} Lvl. {ingredient.Level}", ingredient.Amount);
        }

        if (dialog.SelectedRecipe.IsCraftableWith(dialog.PlayerInventory))
            b.WithFooter("*Craftable*");
        else
            b.WithDescription("*Not enough materials!*");

        return b.Build();
    }
}