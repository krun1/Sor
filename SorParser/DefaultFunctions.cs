using Sor.API;
using Sor.API.SimpleNodes;

namespace Sor;

public class DefaultFunctions : IFunctionsFactory
{
    public void Initialize(ParsingFunctions parsingFunctions)
    {
        parsingFunctions.Add("sum", (IInputArray? array)
            => array?.Sum(node => (node as IInputValue)?.AsInt() ?? throw new NullReferenceException()));
        parsingFunctions.Add("min", (IInputArray? array)
            => array?.Min(node => (node as IInputValue)?.AsInt() ?? throw new NullReferenceException()));
        parsingFunctions.Add("max", (IInputArray? array)
            => array?.Max(node => (node as IInputValue)?.AsInt() ?? throw new NullReferenceException()));
        
        parsingFunctions.Add("map", (IInputArray? l, IInputArray? r) => Map(l, r));
        parsingFunctions.Add("filter", (IInputArray? array, IInputFunction? filter) => Filter(array, filter));
    }

    private static IInputNode? Map(IInputArray? l, IInputArray? r)
    {
        if (l is null || r is null) throw new NullReferenceException();
        var count = Math.Max(l.Count(), r.Count());
        var result = new IInputNode[count];

        for (int i = 0; i < count; i++)
        {
            var le = l.ElementAtOrDefault(i);
            var re = r.ElementAtOrDefault(i);
            var dico = new Dictionary<string, IInputNode?>();

            if (le != null)
            {
                if (le is IInputObject leObj)
                {
                    foreach (var field in leObj.FieldsName)
                        dico.Add(field, leObj.GetField(field));
                }
                else
                {
                    dico.Add("_1", le);
                }
            }

            if (re != null)
            {
                if (re is IInputObject reObj)
                {
                    foreach (var field in reObj.FieldsName)
                        dico.Add(field, reObj.GetField(field));
                }
                else
                {
                    dico.Add("_2", re);
                }
            }

            result[i] = new ResultObject(dico);
        }

        return new ResultArray(result.ToList());
    }

    private static IInputNode? Filter(IInputArray? array, IInputFunction? filter)
    {
        if (array is null || filter is null)
            throw new NullReferenceException();
        var nodes = new List<IInputNode?>();
        
        foreach (var inputNode in array)
        {
            var result = filter.Execute(inputNode);

            switch (result)
            {
                case null:
                    break;
                case IInputValue inputValue when inputValue.AsBool() is { } boolValue:
                {
                    if (boolValue)
                        nodes.Add(inputNode);
                    break;
                }
                default:
                    throw new InvalidOperationException($"Filter {filter} in method 'Filter' must return a boolean value.");
            }
        }
        return new ResultArray(nodes);
    }
}