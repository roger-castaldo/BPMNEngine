using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Interfaces
{
    internal interface IParentElement : IElement
    {
        IElement[] Children { get; }
    }
}
