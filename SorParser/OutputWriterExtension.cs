using Sor.API;
using Sor.Wrappers;

namespace Sor;


internal static class OutputWriterExtensions
{
    public static IOutputNode<TOutput>? BuildInternal<TOutput>(BaseOutputWriter<TOutput> self, IInputNode? input)
    {
        if (input == null)
            return null;
        var result = input switch
        {
            RangeInputArray range => self.BuildArray(range.GetResult()),
            IInputArray inputArray => self.BuildArray(inputArray),
            IInputObject inputObject => self.BuildObject(inputObject),
            IInputValue inputValue => self.BuildValue(inputValue),
            _ => throw new ArgumentOutOfRangeException(nameof(input))
        };
        return new DefaultOutputNode<TOutput>(result);
    }
}