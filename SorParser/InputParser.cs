using System.Reflection;
using Antlr4.Runtime;
using BasicComponents;
using Sor.API;
using Sor.Filter;

namespace Sor;

public class SorParser
{
    private readonly List<IInputReader> _inputReaders = [];
    private readonly List<IOutputWriter> _outputWriters = [];
    private readonly ParsingFunctions _parsingFunctions = new ParsingFunctions();
    
    public SorParser()
    {
        var filesPath = Directory.GetDirectories("./Plugins/")
            .Append("./Plugins/")
            .Distinct()
            .SelectMany(s => Directory.GetFiles(s, "*.dll"))
            .Select(Path.GetFullPath)
            .Distinct()
            .ToList();

        new DefaultFunctions().Initialize(_parsingFunctions);
        foreach (var filePath in filesPath)
        {
            var types = Assembly.LoadFile(filePath).GetExportedTypes()
                .Where(type => type is { IsClass: true, IsAbstract: false, IsGenericType: false })
                .ToList();
            
            _inputReaders.AddRange(types.Where(type => type.ImplementInterface(typeof(IInputReader)))
                .Select(type => (IInputReader?)Activator.CreateInstance(type))
                .WhereNotNull());
            _outputWriters.AddRange(types.Where(type => type.IsSubclassOfGenericInterface(typeof(IOutputWriterInternal<>)))
                .Select(type => (IOutputWriter?)GetType()
                    .GetMethod(nameof(BuildWriter), BindingFlags.Static | BindingFlags.NonPublic)!
                    .MakeGenericMethod(type, type.GetGenericParametersOfGenericInterface(typeof(IOutputWriterInternal<>)).Single())
                    .Invoke(null, []))
                .WhereNotNull());
            foreach (var functionsFactory in types
                         .Where(type => type.ImplementInterface(typeof(IFunctionsFactory)))
                         .Select(type => (IFunctionsFactory)Activator.CreateInstance(type)))
            {
                functionsFactory?.Initialize(_parsingFunctions);
            }
        }
    }

    private static IOutputWriter BuildWriter<T, TOutput>()
    {
        return (IOutputWriter)Activator.CreateInstance(typeof(T),
            (Func<BaseOutputWriter<TOutput>, IInputNode?, IOutputNode<TOutput>?>)OutputWriterExtensions.BuildInternal);
    }
    
    internal IInputReader? FindReader(string inputType)
    {
        return _inputReaders.FirstOrDefault(reader => string.Equals(reader.TypeKey, inputType, StringComparison.OrdinalIgnoreCase));
    }
    
    internal BaseOutputWriter<T>? FindWriter<T>(string outputType)
    {
        var writers = _outputWriters
            .Where(writer => string.Equals(writer.OutputKey, outputType, StringComparison.OrdinalIgnoreCase))
            .ToList();
        
        if (!writers.Any())
            return null;
        if (typeof(T) == typeof(string))
            return (BaseOutputWriter<T>)(object)new OutputWriterStringWrapper(
                OutputWriterExtensions.BuildInternal,
                writers.First());
        return writers.OfType<BaseOutputWriter<T>>().Single();
    }
    
    public IOutputNode<T>? Parse<T>(string pattern)
    {
        var lexer = new FilterGrammarLexer(new AntlrInputStream(pattern));
        var tokenStream = new CommonTokenStream(lexer);
        var parser = new FilterGrammarParser(tokenStream);

        parser.AddErrorListener(new ErrorHandler());
        return new ExpressionVisitor<T>(this, _parsingFunctions).VisitExpression(parser.expression());
    }

    class ErrorHandler : BaseErrorListener
    {
        public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg,
            RecognitionException e)
        {
            throw new InvalidParserException(line, charPositionInLine, msg, e);
        }
    }
}

public class InvalidParserException
    (int line, int charPositionInLine, string msg, RecognitionException recognitionException)
    : Exception($"Parsing error (line : {line}, char {charPositionInLine}) : {msg}", recognitionException);