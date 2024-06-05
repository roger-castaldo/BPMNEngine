using System.Text.Json;

namespace BPMNEngine.Interfaces.State
{
    internal interface IStateContainer : IDisposable
    {
        XmlReader Load(XmlReader reader,Version version);
        Utf8JsonReader Load(Utf8JsonReader reader,Version version);
        IReadOnlyStateContainer Clone();
    }
}
