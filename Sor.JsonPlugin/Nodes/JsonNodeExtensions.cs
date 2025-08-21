using System.Text.Json.Nodes;
using Sor.API;

namespace Sor.JsonPlugin.Nodes;

internal static class JsonNodeExtensions
{
    public static IInputNode? ToInputNode(this JsonNode? jsonNode)
    {
        return jsonNode switch
        {
            null => null,
            JsonArray jsonArray => new JsonInputArray(jsonArray),
            JsonObject jsonObject => new JsonInputObject(jsonObject),
            JsonValue jsonValue => new JsonInputValue(jsonValue),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}