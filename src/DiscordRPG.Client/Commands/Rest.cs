namespace DiscordRPG.Client.Commands;

/*[RequireCharacter]
[RequireNoCurrentActivity]
[RequireChannelName(ServerHandler.Inn)]
[RequireGuild]
public class Rest : DialogCommandBase<RestDialog>
{
    public Rest(DiscordSocketClient client, ILogger logger, IActivityService activityService,
        ICharacterService characterService, IDungeonService dungeonService, IGuildService guildService) : base(client,
        logger, activityService, characterService, dungeonService, guildService)
    {
    }

    public override string CommandName => "rest";

    public override async Task InstallAsync(SocketGuild guild)
    {
        try
        {
            var optionBuilder = OptionHelper.GetActivityDurationBuilder("How long do you want to rest?");
            var command = new SlashCommandBuilder()
                .WithName(CommandName)
                .WithDescription("Rest and recover from your wounds")
                .AddOption(optionBuilder)
                .Build();

            await guild.CreateApplicationCommandAsync(command);
        }
        catch (Exception e)
        {
            logger.Here().Error(e, "Failed to install command {Name}", CommandName);
        }
    }

    protected override async Task HandleDialogAsync(SocketSlashCommand command, GuildCommandContext context,
        RestDialog dialog)
    {
        dialog.CharId = context.Character.ID;
        dialog.ServerId = context.Guild.ServerId;
        var value = (long) command.Data.Options.FirstOrDefault().Value;
        var duration = (ActivityDuration) (int) value;
        dialog.Duration = duration;

        var component = new ComponentBuilder()
            .WithButton("Rest", GetCommandId("rest"))
            .WithButton("Cancel", GetCommandId("cancel"), ButtonStyle.Secondary)
            .Build();

        var text =
            $"Do you want to rest at an inn and recover from your wounds?";

        await command.RespondAsync(text, component: component, ephemeral: true);
    }

    [Handler("rest")]
    public async Task HandleRest(SocketMessageComponent component, RestDialog dialog)
    {
        var result = await activityService.QueueActivityAsync(dialog.CharId, dialog.Duration,
            ActivityType.Rest, new ActivityData
            {
                UserId = dialog.UserId,
                ServerId = dialog.ServerId
            });

        await component.UpdateAsync(properties =>
        {
            properties.Components = null;
            if (result.WasSuccessful)
                properties.Content =
                    $"You are resting! Come back in {(int) dialog.Duration} minutes.";
            else
                properties.Content = $"Something went wrong, please try again in a few minutes!";
        });

        EndDialog(dialog.UserId);
    }
}*/