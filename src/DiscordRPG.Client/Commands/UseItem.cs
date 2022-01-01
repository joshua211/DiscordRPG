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
public class UseItem : DialogCommandBase<UseItemDialog>
{
    public UseItem(DiscordSocketClient client, ILogger logger, IActivityService activityService,
        ICharacterService characterService, IDungeonService dungeonService, IGuildService guildService) : base(client,
        logger, activityService, characterService, dungeonService, guildService)
    {
    }

    public override string CommandName => "use-item";

    public override async Task InstallAsync(SocketGuild guild)
    {
        try
        {
            var command = new SlashCommandBuilder()
                .WithName(CommandName)
                .WithDescription("Use an item from your inventory")
                .Build();

            await guild.CreateApplicationCommandAsync(command);
        }
        catch (Exception e)
        {
            logger.Here().Error(e, "Failed to install command {Name}", CommandName);
        }
    }

    protected override async Task HandleDialogAsync(SocketSlashCommand command, GuildCommandContext context,
        UseItemDialog dialog)
    {
        dialog.Character = context.Character;
        var menu = GetMenu(dialog);
        var embeds = GetDisplayEmbeds(dialog);

        await command.RespondAsync("Choose which item you want to use.", embeds, component: menu, ephemeral: true);
    }

    private Embed[] GetDisplayEmbeds(UseItemDialog dialog)
    {
        if (dialog.SelectedItem is null)
            return null;

        return new[]
        {
            new EmbedBuilder()
                .WithTitle(dialog.SelectedItem.Name)
                .WithDescription(dialog.SelectedItem.Description)
                .AddField("Amount", dialog.SelectedItem.Amount)
                .Build()
        };
    }

    private MessageComponent GetMenu(UseItemDialog dialog)
    {
        var menuBuilder = new SelectMenuBuilder();
        menuBuilder.WithCustomId(CommandName + ".select-item");
        foreach (var item in dialog.Character.Inventory.Where(i => i.IsUsable))
        {
            menuBuilder.AddOption(item.Name, item.Name, item.Description);
        }

        var componentBuilder = new ComponentBuilder();
        if (menuBuilder.Options.Any())
            componentBuilder.WithSelectMenu(menuBuilder);
        componentBuilder.WithButton("Use Item", CommandName + ".use",
            disabled: dialog.SelectedItem is null);
        componentBuilder.WithButton("Cancel", CommandName + ".cancel", ButtonStyle.Secondary);

        return componentBuilder.Build();
    }

    protected override Task HandleSelection(SocketMessageComponent component, string id, UseItemDialog dialog) =>
        id switch
        {
            "select-item" => HandleSelectItem(component, dialog),
        };

    private async Task HandleSelectItem(SocketMessageComponent component, UseItemDialog dialog)
    {
        var itemName = component.Data.Values.First();
        dialog.SelectedItem = dialog.Character.Inventory.FirstOrDefault(i => i.Name == itemName);

        var menu = GetMenu(dialog);
        var embeds = GetDisplayEmbeds(dialog);

        await component.UpdateAsync(properties =>
        {
            properties.Components = menu;
            properties.Embeds = embeds;
        });
    }

    protected override Task HandleButton(SocketMessageComponent component, string id, UseItemDialog dialog) => id switch
    {
        "cancel" => HandleCancel(component, dialog),
        "use" => HandleUseItem(component, dialog)
    };

    private async Task HandleUseItem(SocketMessageComponent component, UseItemDialog dialog)
    {
        EndDialog(dialog.UserId);
        var result = await characterService.UseItemAsync(dialog.Character, dialog.SelectedItem);
        Embed embed;
        if (!result.WasSuccessful)
            embed = new EmbedBuilder().WithTitle("Something went wrong!").WithDescription(result.ErrorMessage).Build();
        else
            embed = new EmbedBuilder().WithTitle("Success!")
                .WithDescription($"You have successfully used {dialog.SelectedItem.Name}!").Build();

        await component.UpdateAsync(properties =>
        {
            properties.Components = null;
            properties.Content = null;
            properties.Embeds = null;
            properties.Embed = embed;
        });
    }

    private async Task HandleCancel(SocketMessageComponent component, UseItemDialog dialog)
    {
        EndDialog(dialog.UserId);
        await component.UpdateAsync(properties =>
        {
            properties.Components = null;
            properties.Content = "Maybe another time";
            properties.Embeds = null;
        });
    }
}