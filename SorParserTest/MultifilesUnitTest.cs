using System.Text.Json.Nodes;
using Sor;

namespace SorParserTest;

public class MultiFilesUnitTest
{
    [Test]
    public void MapCsvAndJsonTest()
    {
        var parser = new SorParser();
        var name = """
                   FROM csvFile
                   "test2.csv"
                   $key = .[].key
                   FROM jsonFile
                   "test1.json"
                   $status = .result[].status;
                   map($key, $status)
                   TO json
                   """;
        var outputNode = parser.Parse<JsonNode>(name);
        Assert.That(outputNode?.GetResult(), Is.Not.Null);
        Assert.That(outputNode.GetResult().ToJsonString(),
            Is.EqualTo(new JsonArray
            {
                new JsonObject
                {
                    {"_1", "a"},
                    {"_2", "active"}
                },
                new JsonObject
                {
                    {"_1", "b"},
                    {"_2", "active"}
                },
                new JsonObject
                {
                    {"_1", "c"},
                }
            }.ToJsonString()));
    }
}