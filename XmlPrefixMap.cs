using System;
using System.Collections.Generic;
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

        private Dictionary<string, List<string>> _prefixMaps;

        public XmlPrefixMap()
        {
            _prefixMaps = new Dictionary<string, List<string>>();
        }

        public void Load(XmlElement element)
        {
            foreach (XmlAttribute att in element.Attributes)
            {
                if (att.Name.StartsWith("xmlns:"))
                {
                    string prefix=null;
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
                    if (prefix != null)
                    {
                        if (!_prefixMaps.ContainsKey(prefix))
                            _prefixMaps.Add(prefix, new List<string>());
                        List<string> tmp = _prefixMaps[prefix];
                        tmp.Add(att.Name.Substring(att.Name.IndexOf(':') + 1));
                        _prefixMaps.Remove(prefix);
                        _prefixMaps.Add(prefix, tmp);
                    }
                }
            }
        }

        internal List<string> Translate(string prefix)
        {
            List<string> ret = new List<string>();
            lock (_prefixMaps)
            {
                if (_prefixMaps.ContainsKey(prefix))
                    ret.AddRange(_prefixMaps[prefix].ToArray());
            }
            return ret;
        }
    }
}
