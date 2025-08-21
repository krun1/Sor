namespace Sor.API;

public class ParsingFunctions
{
    private readonly Dictionary<string, Func<IInputNode?>> _func0 = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, Func<IInputNode?, IInputNode?>> _func1 = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, Func<IInputNode?, IInputNode?, IInputNode?>> _func2 = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, Func<IInputNode?, IInputNode?, IInputNode?, IInputNode?>> _func3 = new(StringComparer.OrdinalIgnoreCase);

    private IInputNode? Invoke(string name)
    {
        if (_func0.TryGetValue(name, out var func))
        {
            return func();
        }
        throw new InvalidOperationException($"Function {name} with 0 parameter not found.");
    }
    
    private IInputNode? Invoke(string name, IInputNode? param1)
    {
        if (_func1.TryGetValue(name, out var func))
        {
            return func(param1);
        }
        throw new InvalidOperationException($"Function {name} with 1 parameter not found.");
    }
    
    private IInputNode? Invoke(string name, IInputNode? param1, IInputNode? param2)
    {
        if (_func2.TryGetValue(name, out var func))
            return func(param1, param2);
        throw new InvalidOperationException($"Function {name} with 2 parameters not found.");
    }
    
    private IInputNode? Invoke(string name, IInputNode? param1, IInputNode? param2, IInputNode? param3)
    {
        if (_func3.TryGetValue(name, out var func))
            return func(param1, param2, param3);
        throw new InvalidOperationException($"Function {name} with 3 parameters not found.");
    }
    
    public IInputNode? Invoke(string name, List<IInputNode?> parameters)
    {
        return parameters.Count switch
        {
            0 => Invoke(name),
            1 => Invoke(name, parameters[0]),
            2 => Invoke(name, parameters[0], parameters[1]),
            3 => Invoke(name, parameters[0], parameters[1], parameters[2]),
            _ => throw new ArgumentException($"Invalid number of parameters: {parameters.Count}")
        };
    }
    
    public void Add(string name, Func<IInputNode?> func) => _func0.Add(name, func);
    public void Add<T1>(string name, Func<T1?, IInputNode?> func) 
        where T1 : IInputNode
        => _func1.Add(name, node => func((T1?)node));
    
    public void Add<T1, T2>(string name, Func<T1?, T2?, IInputNode?> func) 
        where T1 : IInputNode
        where T2 : IInputNode
        => _func2.Add(name, (n1, n2) => func((T1?)n1, (T2?)n2));
    public void Add<T1, T2, T3>(string name, Func<T1?, T2?, T3?, IInputNode?> func)
        where T1 : IInputNode
        where T2 : IInputNode
        where T3 : IInputNode
        => _func3.Add(name, (n, n2, n3) => func((T1?)n, (T2?)n2, (T3?)n3));
}

public static class FunctionsExtensions
{
    public static void Add<T1, TResult>(this ParsingFunctions self, string name, Func<T1?, TResult> func)
        where T1 : IInputNode
    {
        self.Add<T1>(name, p1 => InputHelper.Wrap(func(p1)));
    }
    
    public static void Add<T1, T2, TResult>(this ParsingFunctions self, string name, Func<T1?, T2?, TResult> func)
        where T1 : IInputNode
        where T2 : IInputNode
    {
        self.Add<T1, T2>(name, (p1, p2) => InputHelper.Wrap(func(p1, p2)));
    }
}