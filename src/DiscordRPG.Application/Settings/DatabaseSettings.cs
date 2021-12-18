namespace DiscordRPG.Application.Settings;

public class DatabaseSettings : IDatabaseSettings
{
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
    public string ActivityCollectionName { get; set; }
    public string CharacterCollectionName { get; set; }
    public string GuildCollectionName { get; set; }
    public string DungeonCollectionName { get; set; }
    public string DiagnosticDatabaseName { get; set; }
    public string DiagnosticEventCollectionName { get; set; }
    public string DiagnosticLogCollectionName { get; set; }
}

public interface IDatabaseSettings
{
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
    public string ActivityCollectionName { get; set; }
    public string CharacterCollectionName { get; set; }
    public string GuildCollectionName { get; set; }
    public string DungeonCollectionName { get; set; }
    public string DiagnosticDatabaseName { get; set; }
    public string DiagnosticEventCollectionName { get; set; }
    public string DiagnosticLogCollectionName { get; set; }
}