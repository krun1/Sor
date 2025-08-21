namespace Sor.API.SimpleNodes;

public class ResultObject(IReadOnlyDictionary<string, IInputNode?> value) : IInputObject
{
    public IEnumerable<string> FieldsName => value.Keys;
    public IInputNode? GetField(string fieldName) => value.GetValueOrDefault(fieldName);
}