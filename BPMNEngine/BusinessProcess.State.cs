using BPMNEngine.Interfaces.State;
using System.Collections.Immutable;
using System.Text.Json;

namespace BPMNEngine
{
    public sealed partial class BusinessProcess
    {
        #region Xml State
        private static IState LoadState(XmlDocument doc,int? stepIndex=null)
        {
            return ProcessState.LoadState(doc,stepIndex:stepIndex);
        }

        /// <summary>
        /// A Utility call used to extract the variable values from a Business Process State Document.
        /// </summary>
        /// <param name="doc">The State XML Document file to extract the values from</param>
        /// <returns>The variables extracted from the Process State Document</returns>
        public static IImmutableDictionary<string, object> ExtractProcessVariablesFromStateDocument(XmlDocument doc)
            => LoadState(doc).Variables;

        /// <summary>
        /// A Utility call used to extract the variable values from a Business Process State Document at a given step index.
        /// </summary>
        /// <param name="doc">The State XML Document file to extract the values from</param>
        /// <param name="stepIndex">The step index to extract the values at</param>
        /// <returns>The variables extracted from the Process State Document</returns>
        public static IImmutableDictionary<string, object> ExtractProcessVariablesFromStateDocument(XmlDocument doc, int stepIndex)
            => LoadState(doc,stepIndex:stepIndex).Variables;

        /// <summary>
        /// A Utility call used to extract the steps from a Business Process State Document
        /// </summary>
        /// <param name="doc">The State XML Document file to extract the values from</param>
        /// <returns>The steps from the Process State Document</returns>
        public static IImmutableList<IStateStep> ExtractProcessSteps(XmlDocument doc)
            => LoadState(doc).Steps;

        /// <summary>
        /// A Utility call used to extract the log from a Business Process State Document
        /// </summary>
        /// <param name="doc">The State XML Document file to extract the values from</param>
        /// <returns>The log from the Process State Document</returns>
        public static string ExtractProcessLog(XmlDocument doc)
            => LoadState(doc).Log;

        #endregion

        #region Json State
        private static IState LoadState(Utf8JsonReader reader, int? stepIndex = null)
        {
            return ProcessState.LoadState(reader, stepIndex: stepIndex);
        }

        /// <summary>
        /// A Utility call used to extract the variable values from a Business Process State Document.
        /// </summary>
        /// <param name="reader">The State Json Document file to extract the values from</param>
        /// <returns>The variables extracted from the Process State Document</returns>
        public static IImmutableDictionary<string, object> ExtractProcessVariablesFromStateDocument(Utf8JsonReader reader)
            => LoadState(reader).Variables;

        /// <summary>
        /// A Utility call used to extract the variable values from a Business Process State Document at a given step index.
        /// </summary>
        /// <param name="reader">The State Json Document file to extract the values from</param>
        /// <param name="stepIndex">The step index to extract the values at</param>
        /// <returns>The variables extracted from the Process State Document</returns>
        public static IImmutableDictionary<string, object> ExtractProcessVariablesFromStateDocument(Utf8JsonReader reader, int stepIndex)
            => LoadState(reader, stepIndex: stepIndex).Variables;

        /// <summary>
        /// A Utility call used to extract the steps from a Business Process State Document
        /// </summary>
        /// <param name="reader">The State Json Document file to extract the values from</param>
        /// <returns>The steps from the Process State Document</returns>
        public static IImmutableList<IStateStep> ExtractProcessSteps(Utf8JsonReader reader)
            => LoadState(reader).Steps;

        /// <summary>
        /// A Utility call used to extract the log from a Business Process State Document
        /// </summary>
        /// <param name="reader">The State Json Document file to extract the values from</param>
        /// <returns>The log from the Process State Document</returns>
        public static string ExtractProcessLog(Utf8JsonReader reader)
            => LoadState(reader).Log;
        #endregion
    }
}
