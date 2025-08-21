namespace Sor.API.SimpleNodes;

public abstract class BaseStringInputValue : IInputValue
{
    public abstract string? AsString();

    public IInputNode? Plus(IInputValue? rightValue)
    {
        if (AsString() is null || rightValue is null)
            return null;
        return InputHelper.Wrap(new System.Data.DataTable()
            .Compute($"{AsString()} + {rightValue.AsString()}", string.Empty));
    }

    public IInputNode? Minus(IInputValue? rightValue)
    {
        if (AsString() is null || rightValue is null)
            return null;
        return InputHelper.Wrap(new System.Data.DataTable()
            .Compute($"{AsString()} - {rightValue.AsString()}", string.Empty));
    }

    public IInputNode? Multiply(IInputValue? rightValue)
    {
        if (AsString() is null || rightValue is null)
            return null;
        return InputHelper.Wrap(new System.Data.DataTable()
            .Compute($"{AsString()} * {rightValue.AsString()}", string.Empty));
    }

    public IInputNode? Div(IInputValue? rightValue)
    {
        if (AsString() is null || rightValue is null)
            return null;
        return InputHelper.Wrap(new System.Data.DataTable()
            .Compute($"{AsString()} / {rightValue.AsString()}", string.Empty));

    }

    public IInputValue? GreaterThan(IInputValue? rightValue)
    {
        if (AsString() is null || rightValue is null)
            return null;
        return (IInputValue?)InputHelper.Wrap(new System.Data.DataTable()
            .Compute($"{AsString()} > {rightValue.AsString()}", string.Empty));
    }

    public IInputValue? LessThan(IInputValue? rightValue)
    {
        if (AsString() is null || rightValue is null)
            return null;
        return (IInputValue?)InputHelper.Wrap(new System.Data.DataTable()
            .Compute($"{AsString()} < {rightValue.AsString()}", string.Empty));
    }

    public IInputValue? GreaterOrEqualThan(IInputValue? rightValue)
    {
        if (AsString() is null || rightValue is null)
            return null;
        return (IInputValue?)InputHelper.Wrap(new System.Data.DataTable()
            .Compute($"{AsString()} >= {rightValue.AsString()}", string.Empty));
    }

    public IInputValue? LessOrEqualThan(IInputValue? rightValue)
    {
        if (AsString() is null || rightValue is null)
            return null;
        return (IInputValue?)InputHelper.Wrap(new System.Data.DataTable()
            .Compute($"{AsString()} <= {rightValue.AsString()}", string.Empty));
    }

    public IInputValue? IsEqual(IInputNode? right)
    {
        return (IInputValue?)InputHelper.Wrap(AsString() == (right as IInputValue)?.AsString());
    }
}