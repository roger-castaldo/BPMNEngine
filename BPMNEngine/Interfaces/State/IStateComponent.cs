using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BPMNEngine.Interfaces.State
{
    internal interface IStateComponent
    {
        bool CanRead(XmlReader reader,Version version);
        bool CanRead(Utf8JsonReader reader,Version version);
        XmlReader Read(XmlReader reader,Version version);
        Utf8JsonReader Read(Utf8JsonReader reader,Version version);
        void Write(XmlWriter writer);
        void Write(Utf8JsonWriter writer);
    }
}
