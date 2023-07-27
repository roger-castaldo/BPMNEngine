using System;
using System.Collections.Generic;
using System.Text;

namespace BPMNEngine.Interfaces.Elements
{
    internal interface IParentElement : IElement
    {
        IEnumerable<IElement> Children { get; }
    }
}
