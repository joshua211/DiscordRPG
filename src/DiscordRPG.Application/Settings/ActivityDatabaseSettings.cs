namespace DiscordRPG.Application.Settings;

public class ActivityDatabaseSettings : IActivityDatabaseSettings
{
    public string ConnectionString { get; set; }
    public string CollectionName { get; set; }
    public string DatabaseName { get; set; }
}

public interface IActivityDatabaseSettings
{
    string ConnectionString { get; set; }
    string CollectionName { get; set; }
    string DatabaseName { get; set; }
}