using Discord;
using Discord.WebSocket;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Client.Commands.Attributes;
using DiscordRPG.Client.Commands.Base;
using DiscordRPG.Client.Dialogs;
using DiscordRPG.Common.Extensions;
using Serilog;

namespace DiscordRPG.Client.Commands;

[RequireGuild]
[RequireCharacter]
public class Crafting : DialogCommandBase<CraftingDialog>
{
    public Crafting(DiscordSocketClient client, ILogger logger, IActivityService activityService,
        ICharacterService characterService, IDungeonService dungeonService, IGuildService guildService) : base(client,
        logger, activityService, characterService, dungeonService, guildService)
    {
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
            .WithCustomId(CommandName + ".item-type")
            .AddOption("Equipment", "equipment", "Craft a new piece of equipment")
            .AddOption("Item", "item", "Craft a new item");
        var component = new ComponentBuilder().WithSelectMenu(menuBuilder)
            .WithButton("Cancel", CommandName + ".cancel", ButtonStyle.Secondary).Build();

        await command.RespondAsync("Choose which type of item you want to craft", ephemeral: true,
            component: component);
    }

    protected override Task HandleSelection(SocketMessageComponent component, string id, CraftingDialog dialog) =>
        id switch
        {
            "item-type" => HandleSelectItemType(component, dialog),
        };

    private async Task HandleSelectItemType(SocketMessageComponent component, CraftingDialog dialog)
    {
        var itemType = component.Data.Values.FirstOrDefault();
        switch (itemType)
        {
            case "equipment":
                await HandleSelectEquipment(component, dialog);
                break;
            case "item":
                await HandleSelectItem(component, dialog);
                break;
        }
    }

    private async Task HandleSelectItem(SocketMessageComponent component, CraftingDialog dialog)
    {
        throw new NotImplementedException();
    }

    private async Task HandleSelectEquipment(SocketMessageComponent component, CraftingDialog dialog)
    {
        throw new NotImplementedException();
    }

    protected override Task HandleButton(SocketMessageComponent component, string id, CraftingDialog dialog)
    {
        throw new NotImplementedException();
    }
}