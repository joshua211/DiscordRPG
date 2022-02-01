namespace DiscordRPG.DiagnosticConsole.Settings;

public class DatabaseSettings : IDatabaseSettings
{
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
    public string LogCollectionName { get; set; }
}

public interface IDatabaseSettings
{
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
    public string LogCollectionName { get; set; }
}