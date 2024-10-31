namespace BPMNEngine.Interfaces.Elements
{
    public interface IValidatableElement : IBaseElement
    {
        (bool isValid,IEnumerable<string> errors) IsValid(ILogger? logger);
    }
}
