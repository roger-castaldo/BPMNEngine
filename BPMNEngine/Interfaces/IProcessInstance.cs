using Microsoft.Extensions.Logging;
using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace BPMNEngine.Interfaces
{
    /// <summary>
    /// This interface defines a running instance of a Business Process and will have its own Unique ID.  It contains its own state 
    /// defining the state of this instance.
    /// </summary>
    public interface IProcessInstance : IDisposable
    {
        /// <summary>
        /// The XML Document that was supplied to the constructor containing the BPMN 2.0 definition
        /// </summary>
        XmlDocument Document { get; }
        /// <summary>
        /// The Process State of this instance
        /// </summary>
        IState CurrentState { get; }
        /// <summary>
        /// The log level to use inside the state document for logging
        /// </summary>
        LogLevel StateLogLevel { get; }
        /// <summary>
        /// This is used to access the values of the process runtime and definition constants
        /// </summary>
        /// <param name="name">The name of the variable</param>
        /// <returns>The value of the variable</returns>
        object this[string name] { get; }
        /// <summary>
        /// Called to obtain the names of all process runtime and definition constants
        /// </summary>
        IEnumerable<string> Keys { get; }
        /// <summary>
        /// Called to Resume a suspended process.  Will fail if the process is not currently suspended.
        /// </summary>
        void Resume();
        /// <summary>
        /// Called to render a PNG image of the process at its current state
        /// </summary>
        /// <param name="outputVariables">Set true to include outputting variables into the image</param>
        /// <param name="type">The image format to encode the diagram in</param>
        /// <returns>A Bitmap containing a rendered image of the process at its current state</returns>
        byte[] Diagram(bool outputVariables, ImageFormat type);
        /// <summary>
        /// Called to render an animated version of the process (output in GIF format).  This will animate each step of the process using the current state.
        /// </summary>
        /// <param name="outputVariables">Set true to output the variables into the diagram</param>
        /// <returns>a binary array of data containing the animated GIF</returns>
        byte[] Animate(bool outputVariables);
        /// <summary>
        /// Called to suspend this instance
        /// </summary>
        void Suspend();
        
        #region ProcessLock
        /// <summary>
        /// Used to lock a Thread into waiting for the process to complete
        /// </summary>
        /// <returns>the result of calling WaitOne on the Process Complete manual reset event</returns>
        bool WaitForCompletion();

        /// <summary>
        /// Used to lock a Thread into waiting for the process to complete including a timeout
        /// </summary>
        /// <param name="millisecondsTimeout">The timeout for the process to complete</param>
        /// <returns>the result of calling WaitOne(millisecondsTimeout) on the Process Complete manual reset event</returns>
        bool WaitForCompletion(int millisecondsTimeout);

        /// <summary>
        /// Used to lock a Thread into waiting for the process to complete including a timeout
        /// </summary>
        /// <param name="timeout">The timeout for the process to complete</param>
        /// <returns>the result of calling WaitOne(timeout) on the Process Complete manual reset event</returns>
        bool WaitForCompletion(TimeSpan timeout);

        /// <summary>
        /// Used to lock a Thread into waiting for the process to complete including a timeout and exit context
        /// </summary>
        /// <param name="millisecondsTimeout">The timeout for the process to complete</param>
        /// <param name="exitContext">The exitContext variable</param>
        /// <returns>the result of calling WaitOne(millisecondsTimeout,exitContext) on the Process Complete manual reset event</returns>
        bool WaitForCompletion(int millisecondsTimeout, bool exitContext);

        /// <summary>
        /// Used to lock a Thread into waiting for the process to complete including a timeout and exit context
        /// </summary>
        /// <param name="timeout">The timeout for the process to complete</param>
        /// <param name="exitContext">The exitContext variable</param>
        /// <returns>the result of calling WaitOne(timeout,exitContext) on the Process Complete manual reset event</returns>
        bool WaitForCompletion(TimeSpan timeout, bool exitContext);

        #endregion
        /// <summary>
        /// Used to get an Active User Task by supplying the task ID
        /// </summary>
        /// <param name="taskID">The id of the task to load</param>
        /// <returns>The User task specified from the business process. If it cannot be found or the Task is not waiting it will return null.</returns>
        IUserTask GetUserTask(string taskID);
        /// <summary>
        /// Used to get an Active Manual Task by supplying the task ID
        /// </summary>
        /// <param name="taskID">The id of the task to load</param>
        /// <returns>The User task specified from the business process. If it cannot be found or the Task is not waiting it will return null.</returns>
        IManualTask GetManualTask(string taskID);
        /// <summary>
        /// Used to get the current variable values for this process instance
        /// </summary>
        Dictionary<string,object> CurrentVariables { get; }
    }
}
