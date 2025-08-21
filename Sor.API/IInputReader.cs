namespace Sor.API;

public interface IInputReader
{
    string TypeKey { get; }
    IInputNode? GetNode(string input);
}