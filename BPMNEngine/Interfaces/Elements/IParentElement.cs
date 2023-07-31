namespace BPMNEngine.Interfaces.Elements
{
    internal interface IParentElement : IElement
    {
        IEnumerable<IElement> Children { get; }
    }
}
