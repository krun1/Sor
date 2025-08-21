using System.Text.Json.Nodes;
using Sor.API;

namespace Sor.JsonPlugin;

public class JsonWriter : BaseOutputWriter<JsonNode>
{
    public JsonWriter(Func<BaseOutputWriter<JsonNode>, IInputNode?, IOutputNode<JsonNode>?> builder) : base(builder)
    {
    }

    public override JsonNode BuildArray(IInputArray inputArray)
    {
        var array = new JsonArray();

        foreach (var node in inputArray)
            array.Add(Build(node)?.GetResult());
        return array;
    }
    
    public override JsonNode BuildObject(IInputObject inputObject)
    {
        var objectNode = new JsonObject();

        foreach (var field in inputObject.FieldsName)
        {
            var outputNode = Build(inputObject.GetField(field));

            objectNode.Add(field, outputNode?.GetResult());
        }
        return objectNode;
    }
    
    public override JsonNode BuildValue(IInputValue inputValue)
    {
        return inputValue.AsString();
    }

    public override string OutputKey => "json";
}