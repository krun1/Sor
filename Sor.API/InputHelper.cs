using System.Collections;
using System.Numerics;

namespace Sor.API;

public static class InputHelper
{
    public static IInputNode? Wrap(object? value)
    {
        return value switch
        {
            null => null,
            string or int or double or float => new DefaultInputNumber<object>(value),
            bool b => new DefaultInputNumber<bool>(b),
            IList list => new DefaultInputArray(list),
            IEnumerable enumerable => new DefaultInputArray(enumerable.Cast<object>().ToList()),
            _ => throw new NotImplementedException()
        };
    }

    private class DefaultInputNumber<T>(T? value) : IInputValue
    {
        public string? AsString() => value?.ToString();
        public IInputNode? Plus(IInputValue? rightValue)
        {
            if (value is null || rightValue is null)
                return null;
            return Wrap(new System.Data.DataTable()
                .Compute($"{AsString()} + {rightValue.AsString()}", string.Empty));
        }

        public IInputNode? Minus(IInputValue? rightValue)
        {
            if (value is null || rightValue is null)
                return null;
            return Wrap(new System.Data.DataTable()
                .Compute($"{AsString()} - {rightValue.AsString()}", string.Empty));
        }

        public IInputNode? Multiply(IInputValue? rightValue)
        {
            if (value is null || rightValue is null)
                return null;
            return Wrap(new System.Data.DataTable()
                .Compute($"{AsString()} * {rightValue.AsString()}", string.Empty));
        }

        public IInputNode? Div(IInputValue? rightValue)
        {
            if (value is null || rightValue is null)
                return null;
            return Wrap(new System.Data.DataTable()
                .Compute($"{AsString()} / {rightValue.AsString()}", string.Empty));

        }

        public IInputValue? GreaterThan(IInputValue? rightValue)
        {
            if (value is null || rightValue is null)
                return null;
            return (IInputValue?)Wrap(new System.Data.DataTable()
                .Compute($"{AsString()} > {rightValue.AsString()}", string.Empty));
        }

        public IInputValue? LessThan(IInputValue? rightValue)
        {
            if (value is null || rightValue is null)
                return null;
            return (IInputValue?)Wrap(new System.Data.DataTable()
                .Compute($"{AsString()} < {rightValue.AsString()}", string.Empty));
        }

        public IInputValue? GreaterOrEqualThan(IInputValue? rightValue)
        {
            if (value is null || rightValue is null)
                return null;
            return (IInputValue?)Wrap(new System.Data.DataTable()
                .Compute($"{AsString()} >= {rightValue.AsString()}", string.Empty));
        }

        public IInputValue? LessOrEqualThan(IInputValue? rightValue)
        {
            if (value is null || rightValue is null)
                return null;
            return (IInputValue?)Wrap(new System.Data.DataTable()
                .Compute($"{AsString()} <= {rightValue.AsString()}", string.Empty));
        }

        public IInputValue? IsEqual(IInputNode? right)
        {
            return (IInputValue?)Wrap(AsString() == (right as IInputValue)?.AsString());
        }
    }
    
    private class DefaultInputArray(IList? value) : IInputArray
    {
        public IInputNode? GetAt(Index index)
        {
            if (value == null)
                return null;
            var r = value[index];

            return Wrap(r);
        }

        public List<IInputNode?> GetRange(Range range)
        {
            return value == null
                ? []
                : value.Cast<object>().Take(range).Select(Wrap).ToList();
        }

        public IInputValue? IsEqual(IInputNode? right)
        {
            if (right is not IInputArray rightArray)
                return (IInputValue?)Wrap(false);
            var inputNodes = rightArray.ToList();
            
            if (value.Count != inputNodes.Count)
                return (IInputValue?)Wrap(false);
            for (int i = 0; i < value.Count; i++)
            {
                if (value[i]?.ToString() != inputNodes[i]?.ToString())
                    return (IInputValue?)Wrap(false);
            }
            return (IInputValue?)Wrap(true);
        }
    }
}