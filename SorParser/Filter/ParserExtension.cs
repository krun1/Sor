using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace Sor.Filter;

public static class ParserExtension
{
    public static string GetString(this ITerminalNode node)
    {
        return node.GetText().Trim('"').Replace("\\\"", "\"");
    }

    public static int GetInt(this ITerminalNode node) => int.Parse(node.GetText());
}