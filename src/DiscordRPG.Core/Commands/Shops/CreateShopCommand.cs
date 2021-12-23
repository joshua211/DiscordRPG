namespace DiscordRPG.Core.Commands.Shops;

public class CreateShopCommand : Command
{
    public CreateShopCommand(Identity guildId)
    {
        GuildId = guildId;
    }

    public Identity GuildId { get; private set; }
}