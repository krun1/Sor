using System.Reflection;
using BasicComponents;
using Sor.API;

namespace Sor;

public class OutputWriterStringWrapper(
    Func<BaseOutputWriter<string>,IInputNode?,IOutputNode<string>?> builder,
    IOutputWriter inner)
    : BaseOutputWriter<string>(builder)
{
    public override IOutputNode<string> Build(IInputNode? input)
    {
        return (IOutputNode<string>)GetType().GetMethod(nameof(BuildGeneric),
                BindingFlags.Instance | BindingFlags.NonPublic,
                [typeof(IInputNode)])!
            .MakeGenericMethod(inner.GetType()
                .GetGenericParametersOfGenericInterface(typeof(IOutputWriterInternal<>))
                .Single())
            .Invoke(this, [input])!;
    }

    private IOutputNode<string>? BuildGeneric<T>(IInputNode input)
    {
        var outputWriter = (BaseOutputWriter<T>)inner;
        var result = outputWriter.Build(input).GetResult();

        return result == null ? null : new DefaultOutputNode<string>(result.ToString() ?? string.Empty);
    }

    public override string OutputKey => throw new NotImplementedException();
}