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

[RequireGuild]
[RequireCharacter]
public class Alchemy : DialogCommandBase<AlchemyDialog>
{
    public Alchemy(DiscordSocketClient client, ILogger logger, IActivityService activityService,
        ICharacterService characterService, IDungeonService dungeonService, IGuildService guildService,
        IShopService shopService) : base(client,
        logger, activityService, characterService, dungeonService, guildService, shopService)
    {
    }

    public override string CommandName => "alchemy";

    public override async Task InstallAsync(SocketGuild guild)
    {
        try
        {
            var command = new SlashCommandBuilder()
                .WithName(CommandName)
                .WithDescription("Craft a new item")
                .Build();

            await guild.CreateApplicationCommandAsync(command);
        }
        catch (Exception e)
        {
            logger.Here().Error(e, "Failed to install command {Name}", CommandName);
        }
    }

    protected override async Task HandleDialogAsync(SocketSlashCommand command, GuildCommandContext context,
        AlchemyDialog dialog)
    {
        dialog.AvailableRecipes = context.Character.GetAvailableRecipes().ToList();
        dialog.GuildId = new GuildId(context.Guild.Id);
        dialog.CharacterId = new CharacterId(context.Character.Id);

        var menu = GetMenu(dialog);
        var embed = GetDisplayEmbed(dialog);

        await command.RespondAsync(embed: embed, component: menu, ephemeral: true);
    }

    private Embed GetDisplayEmbed(AlchemyDialog dialog)
    {
        if (dialog.RecipeCategory is null)
            return new EmbedBuilder().WithDescription("Choose a recipe").Build();

        if (dialog.SelectedRecipe is null)
        {
            if (!dialog.AvailableRecipes.Any())
            {
                return new EmbedBuilder().WithTitle("No available recipes")
                    .WithDescription(
                        "Check your currently known recipes with ``/recipes`` to see what items you are missing!")
                    .WithColor(Color.Orange)
                    .Build();
            }

            return new EmbedBuilder().WithDescription("Choose a recipe").Build();
        }

        return EmbedHelper.RecipeAsEmbed(dialog.SelectedRecipe);
    }

    private MessageComponent GetMenu(AlchemyDialog dialog)
    {
        var componentBuilder = new ComponentBuilder();
        if (dialog.RecipeCategory is null)
        {
            var catSelectionBuilder = new SelectMenuBuilder();
            catSelectionBuilder.WithCustomId(GetCommandId("select-category"));
            catSelectionBuilder.WithPlaceholder("Choose a category");
            catSelectionBuilder.AddOption(RecipeCategory.HealthPotion.ToString(),
                RecipeCategory.HealthPotion.Humanize());

            componentBuilder.WithSelectMenu(catSelectionBuilder);
            componentBuilder.WithButton("Close", GetCommandId("cancel"), ButtonStyle.Secondary);
            return componentBuilder.Build();
        }

        var recipeSelectionBuilder = new SelectMenuBuilder();
        recipeSelectionBuilder.WithCustomId(GetCommandId("select-recipe"));
        recipeSelectionBuilder.WithPlaceholder("Choose a recipe");
        var pagedRecipes = dialog.AvailableRecipes.Skip((dialog.CurrentPage - 1) * 10)
            .Take(10);
        foreach (var recipe in pagedRecipes)
        {
            recipeSelectionBuilder.AddOption(recipe.Name, recipe.Id.ToString(), recipe.Description);
        }

        if (recipeSelectionBuilder.Options.Any())
            componentBuilder.WithSelectMenu(recipeSelectionBuilder);
        componentBuilder.WithButton("Craft", GetCommandId("craft"), ButtonStyle.Success,
            disabled: dialog.SelectedRecipe is null);
        componentBuilder.WithButton("Back", GetCommandId("back"), ButtonStyle.Secondary);
        componentBuilder.WithButton("Close", GetCommandId("cancel"), ButtonStyle.Secondary);
        componentBuilder.WithButton("<", GetCommandId("prev-page"), ButtonStyle.Secondary,
            disabled: dialog.CurrentPage <= 1,
            row: 1);
        componentBuilder.WithButton(">", GetCommandId("next-page"), ButtonStyle.Secondary, row: 1);

        return componentBuilder.Build();
    }

    [Handler("select-category")]
    public async Task HandleSelectCategory(SocketMessageComponent component, AlchemyDialog dialog)
    {
        var category = Enum.Parse<RecipeCategory>(component.Data.Values.FirstOrDefault());
        dialog.RecipeCategory = category;

        var menu = GetMenu(dialog);
        var embed = GetDisplayEmbed(dialog);

        await component.UpdateAsync(properties =>
        {
            properties.Components = menu;
            properties.Embed = embed;
        });
    }

    [Handler("select-recipe")]
    public async Task HandleSelectRecipe(SocketMessageComponent component, AlchemyDialog dialog)
    {
        var id = component.Data.Values.FirstOrDefault();
        var recipe = dialog.AvailableRecipes.FirstOrDefault(r => r.Id.Value == id);
        dialog.SelectedRecipe = recipe;

        var menu = GetMenu(dialog);
        var embed = GetDisplayEmbed(dialog);

        await component.UpdateAsync(properties =>
        {
            properties.Components = menu;
            properties.Embed = embed;
        });
    }

    [Handler("back")]
    public async Task HandleBack(SocketMessageComponent component, AlchemyDialog dialog)
    {
        dialog.RecipeCategory = null;
        dialog.SelectedRecipe = null;

        var menu = GetMenu(dialog);
        var embed = GetDisplayEmbed(dialog);

        await component.UpdateAsync(properties =>
        {
            properties.Components = menu;
            properties.Embed = embed;
        });
    }

    [Handler("craft")]
    public async Task HandleCraft(SocketMessageComponent component, AlchemyDialog dialog)
    {
        var result = await characterService.CraftItemAsync(dialog.GuildId, dialog.CharacterId, dialog.SelectedRecipe.Id,
            dialog.Context);
        if (result.WasSuccessful)
        {
            var embed = new EmbedBuilder().WithTitle("Alchemy success!")
                .WithDescription($"You have crafted {dialog.SelectedRecipe.Name}!").Build();
            var menu = new ComponentBuilder().WithButton("Back", GetCommandId("back"), ButtonStyle.Secondary)
                .WithButton("Close", GetCommandId("cancel"), ButtonStyle.Secondary).Build();

            var charResult = await characterService.GetCharacterAsync(dialog.CharacterId, dialog.Context);
            dialog.AvailableRecipes = charResult.Value.GetAvailableRecipes().ToList();

            await component.UpdateAsync(properties =>
            {
                properties.Components = menu;
                properties.Embed = embed;
            });
        }
        else
        {
            EndDialog(dialog.UserId);
            var embed = new EmbedBuilder().WithTitle("Something went wrong!").WithDescription("Please try again")
                .WithColor(Color.Red).Build();
            await component.UpdateAsync(properties => { properties.Embed = embed; });
        }
    }

    [Handler("next-page")]
    public async Task HandleNextPage(SocketMessageComponent component, AlchemyDialog dialog)
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

    [Handler("prev-page")]
    public async Task HandlePrevPage(SocketMessageComponent component, AlchemyDialog dialog)
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
}