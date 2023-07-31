using System.Text.Json;

namespace BPMNEngine.Interfaces.State
{
    internal interface IReadOnlyStateContainer
    {
        void Append(XmlWriter writer);
        void Append(Utf8JsonWriter writer);
    }
}
