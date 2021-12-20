using System.Text.Json;

namespace DiscordRPG.Common.Extensions;

public static class ObjectExtensions
{
    public static T DeepCopy<T>(this object obj)
    {
        var json = JsonSerializer.Serialize(obj);
        return JsonSerializer.Deserialize<T>(json);
    }
}