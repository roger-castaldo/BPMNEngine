using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Interfaces
{
    public interface IStepElement : IElement
    {
        IElement Process { get; }
        IElement Lane { get; }
    }
}
