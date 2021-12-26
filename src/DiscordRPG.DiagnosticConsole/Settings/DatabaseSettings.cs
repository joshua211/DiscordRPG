namespace DiscordRPG.DiagnosticConsole.Settings;

public class DatabaseSettings : IDatabaseSettings
{
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
    public string LogCollectionName { get; set; }
    public string EventCollectionName { get; set; }
    public string CharacterCollectionName { get; set; }
    public string LiveDatabaseName { get; set; }
}

public interface IDatabaseSettings
{
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
    public string LogCollectionName { get; set; }
    public string EventCollectionName { get; set; }
    public string CharacterCollectionName { get; set; }
    public string LiveDatabaseName { get; set; }
}