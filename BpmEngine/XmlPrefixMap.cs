using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Org.Reddragonit.BpmEngine
{
    internal class XmlPrefixMap
    {
        private static readonly Regex _regBPMNRef = new Regex(".+www\\.omg\\.org/spec/BPMN/.+/MODEL", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ECMAScript);
        private static readonly Regex _regBPMNDIRef = new Regex(".+www\\.omg\\.org/spec/BPMN/.+/DI", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ECMAScript);
        private static readonly Regex _regDIRef = new Regex(".+www\\.omg\\.org/spec/DD/.+/DI", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ECMAScript);
        private static readonly Regex _regDCRef = new Regex(".+www\\.omg\\.org/spec/DD/.+/DC", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ECMAScript);
        private static readonly Regex _regXSIRef = new Regex(".+www\\.w3\\.org/.+/XMLSchema-instance", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ECMAScript);
        private static readonly Regex _regEXTSRef = new Regex(".+raw\\.githubusercontent\\.com/roger-castaldo/BPMEngine/.+/Extensions", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ECMAScript);

        private readonly ConcurrentDictionary<string, IEnumerable<string>> _prefixMaps;
        private readonly BusinessProcess _process;

        public XmlPrefixMap(BusinessProcess process)
        {
            _process = process;
            _prefixMaps = new ConcurrentDictionary<string, IEnumerable<string>>();
        }

        public bool Load(XmlElement element)
        {
            bool changed = false;
            foreach (XmlAttribute att in element.Attributes.Cast<XmlAttribute>().Where(att => att.Name.StartsWith("xmlns:")))
            {
                string prefix = null;
                if (_regBPMNRef.IsMatch(att.Value))
                    prefix = "bpmn";
                else if (_regBPMNDIRef.IsMatch(att.Value))
                    prefix = "bpmndi";
                else if (_regDIRef.IsMatch(att.Value))
                    prefix = "di";
                else if (_regDCRef.IsMatch(att.Value))
                    prefix = "dc";
                else if (_regXSIRef.IsMatch(att.Value))
                    prefix = "xsi";
                else if (_regEXTSRef.IsMatch(att.Value))
                    prefix = "exts";
                if (prefix != null)
                {
                    changed = true;
                    _process.WriteLogLine((string)null, LogLevels.Debug, new System.Diagnostics.StackFrame(1, true), DateTime.Now, string.Format("Mapping prefix {0} to {1}", new object[] { prefix, att.Name.Substring(att.Name.IndexOf(':') + 1) }));
                    if (!_prefixMaps.ContainsKey(prefix))
                        _prefixMaps.TryAdd(prefix, Array.Empty<string>());
                    IEnumerable<string> current;
                    _prefixMaps.TryGetValue(prefix, out current);
                    _prefixMaps.TryUpdate(prefix, current.Append(att.Name.Substring(att.Name.IndexOf(':') + 1)), current);
                }
            }
            return changed;
        }

        public IEnumerable<string> Translate(string prefix)
        {
            _process.WriteLogLine((string)null, LogLevels.Debug, new System.Diagnostics.StackFrame(1, true), DateTime.Now, string.Format("Attempting to translate xml prefix {0}", new object[] { prefix }));
            IEnumerable<string> ret;
            _prefixMaps.TryGetValue(prefix, out ret);
            return ret;
        }

        internal bool isMatch(string prefix, string tag, string nodeName)
        {
            _process.WriteLogLine((string)null, LogLevels.Debug, new System.Diagnostics.StackFrame(1, true), DateTime.Now, string.Format("Checking if prefix {0} matches {1}:{2}", new object[] { nodeName, prefix, tag }));
            return string.Equals($"{prefix}:{tag}", nodeName, StringComparison.InvariantCultureIgnoreCase)
                ||Translate(prefix).Any(str => string.Equals($"{prefix}:{str}", nodeName, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
