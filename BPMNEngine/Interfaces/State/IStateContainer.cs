using System.Text.Json;

namespace BPMNEngine.Interfaces.State
{
    internal interface IStateContainer : IDisposable
    {
        void Load(XmlReader reader);
        void Load(Utf8JsonReader reader);
        IReadOnlyStateContainer Clone();
    }
}
