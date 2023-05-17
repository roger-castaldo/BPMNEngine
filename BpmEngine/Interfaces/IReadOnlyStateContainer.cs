using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;

namespace BpmEngine.Interfaces
{
    internal interface IReadOnlyStateContainer
    {
        void Append(XmlWriter writer);
        void Append(Utf8JsonWriter writer);
    }
}
