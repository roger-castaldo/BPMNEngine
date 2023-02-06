using System;
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

        private Dictionary<string, List<string>> _prefixMaps;
        private BusinessProcess _process;

        public XmlPrefixMap(BusinessProcess process)
        {
            _process = process;
            _prefixMaps = new Dictionary<string, List<string>>();
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
                        _prefixMaps.Add(prefix, new List<string>());
                    List<string> tmp = _prefixMaps[prefix];
                    tmp.Add(att.Name.Substring(att.Name.IndexOf(':') + 1));
                    _prefixMaps.Remove(prefix);
                    _prefixMaps.Add(prefix, tmp);
                }
            }
            return changed;
        }

        public List<string> Translate(string prefix)
        {
            _process.WriteLogLine((string)null, LogLevels.Debug, new System.Diagnostics.StackFrame(1, true), DateTime.Now, string.Format("Attempting to translate xml prefix {0}", new object[] { prefix }));
            List<string> ret = new List<string>();
            lock (_prefixMaps)
            {
                if (_prefixMaps.ContainsKey(prefix))
                {
                    _process.WriteLogLine((string)null, LogLevels.Debug, new System.Diagnostics.StackFrame(1, true), DateTime.Now, string.Format("Translation for XML Prefix {0} located", new object[] { prefix }));
                    ret.AddRange(_prefixMaps[prefix].ToArray());
                }
            }
            return ret;
        }

        internal bool isMatch(string prefix, string tag, string nodeName)
        {
            _process.WriteLogLine((string)null, LogLevels.Debug, new System.Diagnostics.StackFrame(1, true), DateTime.Now, string.Format("Checking if prefix {0} matches {1}:{2}",new object[] { nodeName,prefix,tag }));
            if (string.Format("{0}:{1}", prefix, tag).ToLower() == nodeName.ToLower())
                return true;
            foreach (string str in Translate(prefix))
            {
                _process.WriteLogLine((string)null, LogLevels.Debug, new System.Diagnostics.StackFrame(1, true), DateTime.Now, string.Format("Checking if prefix {0} matches {1}:{2}", new object[] { nodeName,str,tag}));
                if (string.Format("{0}:{1}", str, tag).ToLower() == nodeName.ToLower())
                    return true;
            }
            return false;
        }
    }
}
