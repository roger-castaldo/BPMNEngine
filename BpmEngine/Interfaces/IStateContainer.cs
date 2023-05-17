using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace BpmEngine.Interfaces
{
    internal interface IStateContainer : IDisposable
    {
        void Load(XmlReader reader);
        void Load(Utf8JsonReader reader);
        IReadOnlyStateContainer Clone();
    }
}
