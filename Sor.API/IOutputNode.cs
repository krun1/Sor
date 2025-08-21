namespace Sor.API;

public interface IOutputWriter
{
    string OutputKey { get; }
}

public interface IOutputWriterInternal<TOutput> : IOutputWriter { }

public abstract class BaseOutputWriter<TOutput>(Func<BaseOutputWriter<TOutput>, IInputNode?, IOutputNode<TOutput>?> builder)
    : IOutputWriterInternal<TOutput>
{
    public virtual IOutputNode<TOutput>? Build(IInputNode? input) => builder(this, input); 
    
    public virtual TOutput BuildArray(IInputArray inputArray) => throw new NotImplementedException();
    public virtual TOutput BuildObject(IInputObject inputObject) => throw new NotImplementedException();
    public virtual TOutput BuildValue(IInputValue inputValue) => throw new NotImplementedException();
    public abstract string OutputKey { get; }
}

public interface IOutputNode<out T>
{
    T GetResult();
}

public class DefaultOutputNode<T>(T value) : IOutputNode<T>
{
    public T GetResult() => value;
}

