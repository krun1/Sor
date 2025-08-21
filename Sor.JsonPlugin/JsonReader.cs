using System.Text.Json.Nodes;
using Sor.API;
using Sor.JsonPlugin.Nodes;

namespace Sor.JsonPlugin;

public class JsonReader : IInputReader
{
    public IInputNode? GetNode(string input)
    {
        return JsonNode.Parse(input).ToInputNode();
    }

    public string TypeKey => "jsonString";
}

public class JsonFileReader : IInputReader
{
    public IInputNode? GetNode(string input)
    {
        var text = File.ReadAllText(input);

        return JsonNode.Parse(text).ToInputNode();
    }

    public string TypeKey => "jsonFile";
}