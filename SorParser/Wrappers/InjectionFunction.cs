using Sor.API;
using Sor.Filter;

namespace Sor.Wrappers;

internal class InjectionFunction<T>(FilterGrammarParser.OperationContext operation, ExpressionVisitor<T> parent)
    : IInputFunction
{
    public IInputNode? Execute(IInputNode? root)
    {
        return new FilterVisitor<T>(root, parent).Visit(operation);
    }

    public override string ToString()
    {
        return operation.GetText();
    }

    public IInputValue? IsEqual(IInputNode? right)
    {
        throw new NotImplementedException();
    }
}