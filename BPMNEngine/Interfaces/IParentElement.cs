using System;
using System.Collections.Generic;
using System.Text;

namespace BPMNEngine.Interfaces
{
    internal interface IParentElement : IElement
    {
        IEnumerable<IElement> Children { get; }
    }
}
