using BasicComponents;
using Sor.API;
using Sor.Filter;

namespace Sor;

internal sealed class ExpressionVisitor<T>(SorParser sorParser, ParsingFunctions functions)
    : FilterGrammarBaseVisitor<IOutputNode<T?>?>
{
    public ParsingFunctions Functions { get; } = functions;
    public Dictionary<string, IInputNode?> Variables { get; } = new();
    
    public override IOutputNode<T?>? VisitExpression(FilterGrammarParser.ExpressionContext expression)
    {
        var writerId = expression.writerDef().writerId().GetText();
        var outputWriter = sorParser.FindWriter<T>(writerId);

        if (outputWriter == null)
            throw new InvalidOperationException($"Cannot find writer for : {writerId} and type : {typeof(T).Name}");
        IInputNode? contentResult = null;
        
        expression.content().Zip(expression.readerDef()).SmartForeach(
            each: tuple =>
            {
                var (contentContext, readerDefContext) = tuple;

                if (VisitContent(contentContext, readerDefContext) != null)
                {
                    throw new InvalidOperationException(
                        $"Only last operation should return a value. " +
                        $"Use a variable to keep the result of previous operation \"{contentContext.GetText()}\"");
                }
            },
            last: tuple =>
            {
                var (contentContext, readerDefContext) = tuple;

                contentResult = VisitContent(contentContext, readerDefContext);
            });
        return contentResult == null ? null : outputWriter.Build(contentResult);
    }

    private IInputNode? VisitContent(FilterGrammarParser.ContentContext content, FilterGrammarParser.ReaderDefContext readerDef)
    {
        if (content.variable() == null && content.content() != null)
            throw new InvalidOperationException($"Context : {content.GetText()} is not the last operation. It must be saved in a variable to be used");
        var readerId = readerDef.readerId().GetText();
        var url = readerDef.url().STRING().GetString();
        var inputReader = sorParser.FindReader(readerId);

        if (inputReader == null)
            throw new InvalidOperationException($"Cannot find reader for : {readerId}");
        var inputNode = inputReader.GetNode(url);
        IInputNode? contentResult;

        do
        {
            var filterVisitor = new FilterVisitor<T>(inputNode, this);
            contentResult = filterVisitor.Visit(content.operation());

            if (content.variable() != null)
            {
                Variables.Add(content.variable().ID().GetText(), contentResult?.WithContext(InputContext.Variable));
                contentResult = null;
            }
            content = content.content();
        } while (content != null);
        return contentResult;
    }
}