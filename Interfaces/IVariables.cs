using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Interfaces
{
    internal interface IVariables : IReadonlyVariables
    {
        /// <summary>
        /// Called to get or set the value of a process variable
        /// </summary>
        /// <param name="name">The name of the process variable</param>
        /// <returns>The value of the process variable or null if not found</returns>
        new object this[string name] { get; set; } 
    }
}
