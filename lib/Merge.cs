using System.Text.Json;

namespace Attriba;

public class Merge(Dictionary<string, string> source, Dictionary<string, string> overrider)
{
    public JsonDocument AsJsonDocument()
    {
        var result = AsResultDictionary();
        return Json.DocumentFrom(result);
    }

    public Dictionary<string, string> AsResultDictionary()
    {
        var result = source.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        foreach (var kvp in overrider)
        {
            result[kvp.Key] = kvp.Value;
        }

        return result;
    }

    public static Merge Of(JsonDocument source, Dictionary<string, string>? overrider)
    {
        var sourceDict = source.ToDictionary();
        return new Merge(sourceDict, overrider ?? []);
    }
}