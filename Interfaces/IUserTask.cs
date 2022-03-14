using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Interfaces
{
    public interface IUserTask : IManualTask
    {
        string UserID { get; set; }
    }
}
