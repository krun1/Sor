using System.Text.Json.Nodes;
using Sor.API;

namespace Sor.JsonPlugin.Nodes;

internal class JsonInputArray : IInputArray
{
    private readonly JsonArray _array;

    public JsonInputArray(JsonArray array)
    {
        _array = array;
    }

    public IInputNode? GetAt(Index index)
    {
        return _array[index].ToInputNode();
    }

    public List<IInputNode?> GetRange(Range range)
    {
        return _array.Take(range).Select(node => node?.ToInputNode()).ToList();
    }
    
    public IInputValue? IsEqual(IInputNode? right)
    {
        if (right is not IInputArray inputArray)
            return (IInputValue?)InputHelper.Wrap(false);
        var inputNodes = inputArray.ToList();
            
        if (inputNodes.Count != _array.Count)
            return (IInputValue?)InputHelper.Wrap(false);
        foreach (var (l, r) in _array.Zip(inputNodes))
        {
            if (l != null || r != null)
                if (l?.ToInputNode()?.IsEqual(r)?.AsBool() != true)
                    return (IInputValue?)InputHelper.Wrap(false);
        }
        return (IInputValue?)InputHelper.Wrap(true);
    }
}