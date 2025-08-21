using System.Text.Json.Nodes;
using Sor.API;
using Sor.API.SimpleNodes;

namespace Sor.JsonPlugin.Nodes;

internal class JsonInputValue(JsonValue value) : BaseStringInputValue
{
    public override string? AsString()
    {
        return value.ToString();
    }
}