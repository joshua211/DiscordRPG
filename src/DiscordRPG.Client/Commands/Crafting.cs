namespace DiscordRPG.Client.Commands;

/*
[RequireGuild]
[RequireCharacter]
public class Crafting : DialogCommandBase<CraftingDialog>
{
    private readonly RecipeGenerator recipeGenerator;

    public Crafting(DiscordSocketClient client, ILogger logger, IActivityService activityService,
        ICharacterService characterService, IDungeonService dungeonService, IGuildService guildService,
        RecipeGenerator recipeGenerator) : base(client,
        logger, activityService, characterService, dungeonService, guildService)
    {
        this.recipeGenerator = recipeGenerator;
    }

    public override string CommandName => "craft";

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
        CraftingDialog dialog)
    {
        dialog.Character = context.Character;

        var menuBuilder = new SelectMenuBuilder()
            .WithPlaceholder("Select which type of item")
            .WithCustomId(GetCommandId("set-item-type"))
            .AddOption("Equipment", "equipment", "Craft a new piece of equipment")
            .AddOption("Item", "item", "Craft a new item");

        var component = new ComponentBuilder()
            .WithSelectMenu(menuBuilder)
            .WithButton("Cancel", GetCommandId("cancel"), ButtonStyle.Secondary)
            .Build();

        await command.RespondAsync("Choose which type of item you want to craft", ephemeral: true,
            component: component);
    }

    [Handler("select-equipment-category")]
    public async Task SetEquipCategory(SocketMessageComponent component, CraftingDialog dialog)
    {
        var category = component.Data.Values.FirstOrDefault();
        dialog.EquipmentCategory = Enum.Parse<EquipmentCategory>(category);

        await HandleSelectEquipment(component, dialog);
    }

    [Handler("set-item-type")]
    public async Task HandleSelectItemType(SocketMessageComponent component, CraftingDialog dialog)
    {
        var itemType = component.Data.Values.FirstOrDefault();
        switch (itemType)
        {
            case "equipment":
                await HandleSelectEquipmentCategory(component, dialog);
                break;
            case "item":
                await HandleSelectItem(component, dialog);
                break;
        }
    }

    [Handler("select-item-recipe")]
    public async Task HandleSelectItem(SocketMessageComponent component, CraftingDialog dialog)
    {
        var selectedRecipe = component.Data.Values.FirstOrDefault();
        if (selectedRecipe is not null)
            dialog.SelectedRecipe = recipeGenerator
                .GetAllItemRecipes(dialog.Character.Level.CurrentLevel)
                .FirstOrDefault(r => r.Name == selectedRecipe)!;

        var menuComponent = GetRecipeMenu(dialog);
        var embeds = GetDisplayEmbeds(dialog);

        await component.UpdateAsync(properties =>
        {
            properties.Embeds = embeds.Any() ? embeds : null;
            properties.Content = dialog.RecipeCount > 0
                ? "Choose what kind of item you want to craft."
                : "You dont have enough materials to craft anything!";
            properties.Components = menuComponent;
        });
    }

    [Handler("select-equipment-recipe")]
    public async Task HandleSelectEquipment(SocketMessageComponent component, CraftingDialog dialog)
    {
        var selectedRecipe = component.Data.Values.FirstOrDefault();
        dialog.IsEquipment = true;
        if (selectedRecipe is not null)
        {
            dialog.SelectedRecipe = recipeGenerator
                .GetAllEquipmentRecipes(dialog.Character.Level.CurrentLevel)
                .FirstOrDefault(r => r.Name == selectedRecipe)!;
        }

        var embeds = GetDisplayEmbeds(dialog);
        var menuComponent = GetRecipeMenu(dialog);

        await component.UpdateAsync(properties =>
        {
            properties.Embeds = embeds.Any() ? embeds : null;
            properties.Content = dialog.RecipeCount > 0
                ? "Choose what kind of equipment you want to craft."
                : "You dont have enough materials to craft anything!";
            properties.Components = menuComponent;
        });
    }


    private Embed[] GetDisplayEmbeds(CraftingDialog dialog)
    {
        if (dialog.SelectedRecipe is null)
            return Array.Empty<Embed>();

        var builder = new EmbedBuilder();
        builder.WithTitle(dialog.SelectedRecipe.Name);
        builder.WithDescription(dialog.SelectedRecipe.Description);
        builder.AddField("Rarity", dialog.SelectedRecipe.Rarity);
        foreach (var (ingredientName, amount) in dialog.SelectedRecipe.Ingredients)
        {
            builder.AddField($"[{dialog.SelectedRecipe.Rarity.ToString()}] {ingredientName}", amount, true);
        }

        return new[] {builder.Build()};
    }

    private MessageComponent GetRecipeMenu(CraftingDialog dialog)
    {
        var menuBuilder = new SelectMenuBuilder();
        if (dialog.IsEquipment)
        {
            menuBuilder.WithCustomId(GetCommandId("select-equipment-recipe"));
            foreach (var recipe in recipeGenerator
                         .GetAllEquipmentRecipes(dialog.Character.Level.CurrentLevel)
                         .Where(r => r.EquipmentCategory == dialog.EquipmentCategory &&
                                     r.IsCraftableWith(dialog.Character.Inventory))
                         .OrderByDescending(r => r.Level)
                         .Skip((dialog.CurrentPage - 1) * 10)
                         .Take(10))
            {
                menuBuilder.AddOption(recipe.Name, recipe.Name, recipe.Description);
            }
        }

        else
        {
            menuBuilder.WithCustomId(GetCommandId("select-item-recipe"));
            foreach (var recipe in recipeGenerator
                         .GetAllItemRecipes(dialog.Character.Level.CurrentLevel)
                         .Where(r => r.IsCraftableWith(dialog.Character.Inventory))
                         .OrderByDescending(r => r.Level)
                         .Skip((dialog.CurrentPage - 1) * 10)
                         .Take(10))
                menuBuilder.AddOption(recipe.Name, recipe.Name, recipe.Description);
        }

        var builder = new ComponentBuilder();
        if (menuBuilder.Options.Any())
        {
            dialog.RecipeCount = menuBuilder.Options.Count;
            builder.WithSelectMenu(menuBuilder);
        }

        builder.WithButton("<", GetCommandId("prev-page"), ButtonStyle.Secondary, disabled: dialog.CurrentPage <= 1);
        builder.WithButton(">", GetCommandId("next-page"), ButtonStyle.Secondary,
            disabled: menuBuilder.Options.Count < 10);
        builder.WithButton("Craft", GetCommandId("craft"), disabled: dialog.SelectedRecipe is null, row: 3);
        builder.WithButton("Cancel", GetCommandId("cancel"), ButtonStyle.Secondary);

        return builder.Build();
    }

    private async Task HandleSelectEquipmentCategory(SocketMessageComponent component, CraftingDialog dialog)
    {
        var menuBuilder = new SelectMenuBuilder();
        menuBuilder.WithCustomId(GetCommandId("select-equipment-category"));
        foreach (var cat in Enum.GetValues<EquipmentCategory>())
            menuBuilder.AddOption(cat.ToString(), cat.ToString());

        var menuComponent = new ComponentBuilder()
            .WithSelectMenu(menuBuilder)
            .WithButton("Cancel", GetCommandId("cancel"), ButtonStyle.Secondary)
            .Build();

        await component.UpdateAsync(properties =>
        {
            properties.Content = "Choose what kind of equipment you want to craft.";
            properties.Components = menuComponent;
        });
    }

    [Handler("craft")]
    public async Task HandleCraft(SocketMessageComponent component, CraftingDialog dialog)
    {
        EndDialog(dialog.UserId);
        var result = await characterService.CraftItemAsync(dialog.Character, dialog.SelectedRecipe);
        if (!result.WasSuccessful)
        {
            await component.UpdateAsync(properties =>
            {
                properties.Components = null;
                properties.Content = null;
                properties.Embed = new EmbedBuilder().WithTitle("Something went wrong!")
                    .WithDescription(result.ErrorMessage).Build();
            });
        }
        else
        {
            await component.UpdateAsync(properties =>
            {
                properties.Components = null;
                properties.Content = null;
                properties.Embed = new EmbedBuilder().WithTitle("Crafting success!")
                    .WithDescription($"You have successfully crafted {dialog.SelectedRecipe.Name}!").Build();
            });
        }
    }
}*/