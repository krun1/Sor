using System.Collections;
using Sor.API;
using Sor.API.SimpleNodes;

namespace Sor.Wrappers;

internal class RangeInputArray(IReadOnlyList<IInputNode?> inner) : IInputArray, IInputObject, IInputValue
{
    public IInputArray GetResult() => new ResultArray(inner);
    
    public IEnumerable<string> FieldsName => inner.OfType<IInputObject>()
        .SelectMany(node => node.FieldsName)
        .Distinct();

    public IInputNode? GetField(string fieldName)
    {
        return new RangeInputArray(inner.Select(node => (node as IInputObject)?.GetField(fieldName)).ToArray());
    }

    public IInputNode? GetAt(Index index)
    {
        return new RangeInputArray(inner.Select(node => (node as IInputArray)?.GetAt(index)).ToArray());
    }

    public List<IInputNode?> GetRange(Range range)
    {
        return inner.Select(node =>
        {
            var nodes = (node as IInputArray)?.GetRange(range);

            return (IInputNode?)(nodes == null ? null : new RangeInputArray(nodes));
        }).ToList();
    }

    public string? AsString()
        => inner
               .Select(node => (node as IInputValue)?.AsString())
               .Aggregate((s, v) => s + ", " + v)
           ?? string.Empty;

    public IInputNode? Plus(IInputValue? rightValue)
    {
        return new RangeInputArray(inner.Select(node => (node as IInputValue)?.Plus(rightValue)).ToArray());
    }

    public IInputNode? Minus(IInputValue? rightValue)
    {
        return new RangeInputArray(inner.Select(node => (node as IInputValue)?.Minus(rightValue)).ToArray());
    }

    public IInputNode? Multiply(IInputValue? rightValue)
    {
        return new RangeInputArray(inner.Select(node => (node as IInputValue)?.Multiply(rightValue)).ToArray());
    }

    public IInputNode? Div(IInputValue? rightValue)
    {
        return new RangeInputArray(inner.Select(node => (node as IInputValue)?.Div(rightValue)).ToArray());
    }

    public IInputValue? GreaterThan(IInputValue? rightValue)
    {
        return new RangeInputArray(inner.Select(node => (node as IInputValue)?.GreaterThan(rightValue)).ToArray());
    }

    public IInputValue? LessThan(IInputValue? rightValue)
    {
        return new RangeInputArray(inner.Select(node => (node as IInputValue)?.LessThan(rightValue)).ToArray());
    }

    public IInputValue? GreaterOrEqualThan(IInputValue? rightValue)
    {
        return new RangeInputArray(inner.Select(node => (node as IInputValue)?.GreaterOrEqualThan(rightValue)).ToArray());
    }

    public IInputValue? LessOrEqualThan(IInputValue? rightValue)
    {
        return new RangeInputArray(inner.Select(node => (node as IInputValue)?.LessOrEqualThan(rightValue)).ToArray());
    }

    public IInputNode WithContext(InputContext context)
    {
        if (context == InputContext.Parameter)
            return new ResultArray(inner.Select(node => node?.WithContext(context)).ToArray());
        return this;
    }

    public IInputValue? IsEqual(IInputNode? right)
    {
        return new RangeInputArray(inner.Select(node => node?.IsEqual(right)).ToArray());
    }

    public IInputValue? IsNotEqual(IInputNode? right)
    {
        return new RangeInputArray(inner.Select(node => node?.IsNotEqual(right)).ToArray());
    }
}