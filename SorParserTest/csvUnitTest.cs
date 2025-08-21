using System.Text.Json.Nodes;
using Sor;

namespace SorParserTest;

public class csvUnitTest
{
    [Test]
    public void TestReadCsv()
    {
        var parser = new SorParser();

        var name = """
                   FROM csvFile
                   "test2.csv"
                   .[].key
                   TO json
                   """;
        var outputNode = parser.Parse<JsonNode>(name);
        Assert.That(outputNode?.GetResult(), Is.Not.Null);
        Assert.That(outputNode.GetResult().ToJsonString(),
            Is.EqualTo(new JsonArray{"a", "b", "c"}.ToJsonString()));
    }
    
    [Test]
    public void TestReadCsv2()
    {
        var parser = new SorParser();

        var name = """
                   FROM csvFile
                   "test2.csv"
                   .[].value
                   TO json
                   """;
        var outputNode = parser.Parse<JsonNode>(name);
        Assert.That(outputNode?.GetResult(), Is.Not.Null);
        Assert.That(outputNode.GetResult().ToJsonString(),
            Is.EqualTo(new JsonArray{"1", "2", "3"}.ToJsonString()));
    }

    [Test]
    [TestCase("""
              FROM csvFile
              "test2.csv"
              Filter(.[].value, :. == 2)
              TO json
              """)]
    [TestCase("""
              FROM csvFile
              "test2.csv"
              $f = Filter(.[], :.value == 2);
              $f[].value
              TO json
              """)]
    [TestCase("""
              FROM csvFile
              "test2.csv"
              $f = Filter(., :.value == 2);
              $f[].value
              TO json
              """)]
    public void TestFilterCsv(string filter)
    {
        var parser = new SorParser();
        var outputNode = parser.Parse<JsonNode>(filter);
        Assert.That(outputNode?.GetResult(), Is.Not.Null);
        Assert.That(outputNode.GetResult().ToJsonString(),
            Is.EqualTo(new JsonArray{"2"}.ToJsonString()));
    }
}