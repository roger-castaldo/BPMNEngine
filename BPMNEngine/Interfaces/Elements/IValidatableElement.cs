namespace BPMNEngine.Interfaces.Elements
{
    internal interface IValidatableElement : IElement
    {
        (bool isValid,IEnumerable<string> errors) IsValid(ILogger? logger);
    }
}
