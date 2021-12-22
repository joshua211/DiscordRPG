using MongoDB.Bson;

namespace DiscordRPG.DiagnosticConsole.Models;

public class LogEntry
{
    public BsonObjectId Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string Level { get; set; }
    public string MessageTemplate { get; set; }
    public string RenderedMessage { get; set; }
    public string Exception { get; set; }
    public object Properties { get; set; }
    public string UtcTimestamp { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj is not LogEntry ent)
            return false;

        return ent.Id == Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public bool ContainsString(string str)
    {
        var normalized = str.ToLower().Trim();
        return Level.ToLower().Contains(normalized)
               || RenderedMessage.ToLower().Split(' ').Any(s => s.Contains(normalized))
               || (!string.IsNullOrEmpty(Exception) && Exception.ToLower().Split(' ').Any(s => s.Contains(normalized)));
    }
}