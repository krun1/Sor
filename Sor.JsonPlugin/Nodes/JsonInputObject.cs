using System.Text.Json.Nodes;
using Sor.API;

namespace Sor.JsonPlugin.Nodes;

internal class JsonInputObject(JsonObject node) : IInputObject
{
    public IEnumerable<string> FieldsName => node.Select(pair => pair.Key);

    public IInputNode? GetField(string fieldName)
    {
        return node[fieldName].ToInputNode();
    }
}