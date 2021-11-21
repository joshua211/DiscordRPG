namespace DiscordRPG.Application.Settings;

public class CharacterDatabaseSettings : ICharacterDatabaseSettings
{
    public string ConnectionString { get; set; }
    public string CollectionName { get; set; }
    public string DatabaseName { get; set; }
}

public interface ICharacterDatabaseSettings
{
    string ConnectionString { get; set; }
    string CollectionName { get; set; }
    string DatabaseName { get; set; }
}