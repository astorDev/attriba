using System.Text.Json;

namespace Attriba;

public static class Json
{
    public static JsonDocument DocumentFrom(this Dictionary<string, string>? dictionary)
    {
        dictionary ??= [];
        var json = JsonSerializer.Serialize(dictionary);
        return JsonDocument.Parse(json);
    }

    public static Dictionary<string, string> ToDictionary(this JsonDocument document)
    {
        return document.Deserialize<Dictionary<string, string>>() ?? [];
    }
}