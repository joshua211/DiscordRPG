namespace DiscordRPG.Application.Settings;

public class GuildDatabaseSettings : IGuildDatabaseSettings
{
    public string ConnectionString { get; set; }
    public string CollectionName { get; set; }
    public string DatabaseName { get; set; }
}

public interface IGuildDatabaseSettings
{
    string ConnectionString { get; set; }
    string CollectionName { get; set; }
    string DatabaseName { get; set; }
}