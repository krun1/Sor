using System.Text.Json.Nodes;
using Sor;

namespace SorParserTest;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void TestLoadDll()
    {
        var ip = new SorParser();
    }
    
    [Test]
    public void TestErrorReadJson()
    {
        var parser = new SorParser();

        var json = """{    \"titi\":\"1\",    \"toto\":\"1\",    \"tata\":\"1\" }""";
        var str = """FROM jsonString " """ + json + """ " ... TO json""";
        Assert.Throws<InvalidParserException>(() => parser.Parse<JsonNode>(str)) ;
    }
    
    [Test]
    public void TestReadJson()
    {
        var parser = new SorParser();

        var json = """{    \"titi\":\"1\",    \"toto\":\"1\",    \"tata\":\"1\" }""";
        var str = """FROM jsonString " """ + json + """ " . TO json""";
        var outputNode = parser.Parse<JsonNode>(str);

        var jsonNode = JsonNode.Parse(json.Replace("\\\"", "\""))!;

        Assert.That(outputNode, Is.Not.Null);
        Assert.That(outputNode.GetResult().ToJsonString(), Is.EqualTo(jsonNode.ToJsonString()));
    }
    
    [Test]
    public void TestReadJsonField()
    {
        var parser = new SorParser();

        var json = """{    \"titi\":1,    \"toto\":2,    \"tata\":3 }""";
        var str = """FROM jsonString " """ + json + """ " .toto TO json""";
        var outputNode = parser.Parse<string>(str);

        Assert.That(outputNode, Is.Not.Null);
        Assert.That(outputNode.GetResult(), Is.EqualTo("2"));
    }

    [Test]
    public void TestReadJsonArray()
    {
        var parser = new SorParser();

        var json = """{    \"titi\":1,    \"toto\":2,    \"tata\": [ 1, 2, 3 ] }""";
        var str = """FROM jsonString " """ + json + """ " .tata TO json""";
        var outputNode = parser.Parse<JsonNode>(str);

        Assert.That(outputNode, Is.Not.Null);
        Assert.That(outputNode.GetResult().ToJsonString(), 
            Is.EqualTo(new JsonArray{ "1", "2", "3" }.ToJsonString()));
    }
    
    [Test]
    public void TestReadJsonArrayRange()
    {
        var parser = new SorParser();

        var json = """{    \"titi\":1,    \"toto\":2,    \"tata\": [ 1, 2, 3 ] }""";
        var strTake = """FROM jsonString " """ + json + """ " .tata[..2] TO json""";
        var strSkip = """FROM jsonString " """ + json + """ " .tata[1..] TO json""";
        var outputNode1 = parser.Parse<JsonNode>(strTake);
        var outputNode2 = parser.Parse<JsonNode>(strSkip);

        Assert.Multiple(() =>
        {
            Assert.That(outputNode1, Is.Not.Null);
            Assert.That(outputNode2, Is.Not.Null);
        });
        Assert.Multiple(() =>
        {
            Assert.That(outputNode1.GetResult().ToJsonString(), Is.EqualTo(new JsonArray { "1", "2" }.ToJsonString()));
            Assert.That(outputNode2.GetResult().ToJsonString(), Is.EqualTo(new JsonArray { "2", "3" }.ToJsonString()));
        });
    }

    [Test]
    public void TestMultiline()
    {
        var parser = new SorParser();

        var json =
            """
            {
                \"titi\":1,
                \"toto\":2,
                \"tata\": [ 1, 2, 3 ]
            }
            """;
        var strTake = """
                      FROM jsonString
                      " 
                      """ + json + 
                      """
                      " .tata[]
                      
                      TO json
                      """;
        var outputNode1 = parser.Parse<JsonNode>(strTake);

        Assert.That(outputNode1, Is.Not.Null);
        Assert.That(outputNode1.GetResult().ToJsonString(), Is.EqualTo(new JsonArray { "1", "2", "3" }.ToJsonString()));
    }

    [Test]
    public void TestJsonFile()
    {
        var parser = new SorParser();

        var name = """
                      FROM jsonFile
                      "test1.json"
                      .result[].status
                      TO json
                      """;
        var outputNode = parser.Parse<JsonNode>(name);
        Assert.That(outputNode?.GetResult(), Is.Not.Null);
        Assert.That(outputNode.GetResult().ToJsonString(),
            Is.EqualTo(new JsonArray{"active", "active"}.ToJsonString()));
    }
    
    [Test]
    public void TestCreateVariable()
    {
        var parser = new SorParser();

        var name = """
                   FROM jsonFile
                   "test1.json"
                   $v1 = .result[].name.first;
                   .result[].status
                   TO json
                   """;
        var outputNode = parser.Parse<JsonNode>(name);
        Assert.That(outputNode?.GetResult(), Is.Not.Null);
        Assert.That(outputNode.GetResult().ToJsonString(),
            Is.EqualTo(new JsonArray{"active", "active"}.ToJsonString()));
    }
    
    [Test]
    public void TestUseVariable()
    {
        var parser = new SorParser();

        var name = """
                   FROM jsonFile "test1.json"
                   $v1 = .result[].status ;
                   $v1
                   TO json
                   """;
        var outputNode = parser.Parse<JsonNode>(name);
        Assert.That(outputNode?.GetResult(), Is.Not.Null);
        Assert.That(outputNode.GetResult().ToJsonString(),
            Is.EqualTo(new JsonArray{"active", "active"}.ToJsonString()));
    }

    [Test]
    public void TestUseVariable2()
    {
        var parser = new SorParser();

        var name = """
                   FROM jsonFile "test1.json"
                   $v1 = .result[].name ;
                   $v1.first
                   TO json
                   """;
        var outputNode = parser.Parse<JsonNode>(name);
        Assert.That(outputNode?.GetResult(), Is.Not.Null);
        Assert.That(outputNode.GetResult().ToJsonString(),
            Is.EqualTo(new JsonArray{"Felton", "Candido"}.ToJsonString()));
    }
    
    [Test]
    public void TestFunction()
    {
        var parser = new SorParser();

        var name = """
                   FROM jsonFile "test1.json"
                   sum(.result[].creditCard.cvv)
                   TO json
                   """;
        var outputNode = parser.Parse<JsonNode>(name);
        Assert.That(outputNode?.GetResult(), Is.Not.Null);
        Assert.That(outputNode.GetResult().ToJsonString(),
            Is.EqualTo( JsonValue.Create((968 + 950).ToString()).ToJsonString()));
    }
}