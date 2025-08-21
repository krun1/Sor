namespace Sor.API;

public interface IInputFunction : IInputNode
{
    IInputNode? Execute(IInputNode? root);
}