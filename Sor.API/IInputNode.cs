using System.Collections;

namespace Sor.API;

public enum InputContext
{
    Parsing,
    Parameter,
    Variable,
    Result
}

public interface IInputNode
{
    IInputNode WithContext(InputContext context) => this;
    IInputValue? IsEqual(IInputNode? right);
    IInputValue? IsNotEqual(IInputNode? right) => (IInputValue?)InputHelper.Wrap(!IsEqual(right)?.AsBool());
}

public interface IInputObject : IInputNode
{
    IEnumerable<string> FieldsName { get; }
    IInputNode? GetField(string fieldName);

    IInputValue? IInputNode.IsEqual(IInputNode? right)
    {
        if (right == null)
            return (IInputValue?)InputHelper.Wrap(false);
        if (right is not IInputObject inputObject)
            return (IInputValue?)InputHelper.Wrap(false);
        if (!FieldsName.SequenceEqual(inputObject.FieldsName, StringComparer.OrdinalIgnoreCase))
            return (IInputValue?)InputHelper.Wrap(false);
        foreach (var fieldName in FieldsName)
        {
            var l = GetField(fieldName);
            var r = inputObject.GetField(fieldName);
            
            if (l != null || r != null)
                if (l?.IsEqual(r)?.AsBool() != true)
                    return (IInputValue?)InputHelper.Wrap(false);
        }
        return (IInputValue?)InputHelper.Wrap(true);
    }
}

public interface IInputArray : IInputNode, IEnumerable<IInputNode?>
{
    IInputNode? GetAt(Index index);
    List<IInputNode?> GetRange(Range range);

    IEnumerator<IInputNode?> IEnumerable<IInputNode?>.GetEnumerator() => GetRange(Range.All).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetRange(Range.All).GetEnumerator();
}

public interface IInputValue : IInputNode
{
    string? AsString();
    int? AsInt() => int.TryParse(AsString(), out var result) ? result : null;
    double? AsDouble() => double.TryParse(AsString(), out var result) ? result : null;
    bool? AsBool() => bool.TryParse(AsString(), out var result) ? result : null;

    IInputNode? Plus(IInputValue? rightValue);
    IInputNode? Minus(IInputValue? rightValue);
    IInputNode? Multiply(IInputValue? rightValue);
    IInputNode? Div(IInputValue? rightValue);
    IInputValue? GreaterThan(IInputValue? rightValue);
    IInputValue? LessThan(IInputValue? rightValue);
    IInputValue? GreaterOrEqualThan(IInputValue? rightValue);
    IInputValue? LessOrEqualThan(IInputValue? rightValue);
}