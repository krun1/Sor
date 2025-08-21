namespace Sor.API.SimpleNodes;

public class ResultArray(IReadOnlyList<IInputNode?> inner) : IInputArray
{
    public IInputNode? GetAt(Index index)
    {
        return inner[index];
    }

    public List<IInputNode?> GetRange(Range range)
    {
        return inner.Take(range).ToList();
    }

    public IInputValue? IsEqual(IInputNode? right)
    {
        if (right is not IInputArray inputArray)
            return (IInputValue?)InputHelper.Wrap(false);
        var inputNodes = inputArray.ToList();
            
        if (inputNodes.Count != inner.Count)
            return (IInputValue?)InputHelper.Wrap(false);
        foreach (var (l, r) in inner.Zip(inputNodes))
        {
            if (l != null || r != null)
                if (l?.IsEqual(r)?.AsBool() != true)
                    return (IInputValue?)InputHelper.Wrap(false);
        }
        return (IInputValue?)InputHelper.Wrap(true);
    }
}