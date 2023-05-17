using System;
using System.Collections.Generic;
using System.Text;

namespace BpmEngine.Interfaces
{
    internal interface IParentElement : IElement
    {
        IEnumerable<IElement> Children { get; }
    }
}
