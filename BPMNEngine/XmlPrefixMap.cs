using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace BPMNEngine
{
    internal class XmlPrefixMap(BusinessProcess process)
    {
        private static readonly Regex regBPMNRef = new(".+www\\.omg\\.org/spec/BPMN/.+/MODEL", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ECMAScript, TimeSpan.FromMilliseconds(500));
        private static readonly Regex regBPMNDIRef = new(".+www\\.omg\\.org/spec/BPMN/.+/DI", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ECMAScript, TimeSpan.FromMilliseconds(500));
        private static readonly Regex regDIRef = new(".+www\\.omg\\.org/spec/DD/.+/DI", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ECMAScript, TimeSpan.FromMilliseconds(500));
        private static readonly Regex regDCRef = new(".+www\\.omg\\.org/spec/DD/.+/DC", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ECMAScript, TimeSpan.FromMilliseconds(500));
        private static readonly Regex regXSIRef = new(".+www\\.w3\\.org/.+/XMLSchema-instance", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ECMAScript, TimeSpan.FromMilliseconds(500));
        private static readonly Regex regEXTSRef = new(".+raw\\.githubusercontent\\.com/roger-castaldo/BPMNEngine/.+/Extensions", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ECMAScript, TimeSpan.FromMilliseconds(500));

        private readonly struct SPrefixMappingPair
        {
            public string Prefix { get; init; }
            public string Value { get; init; }
        }

        private readonly ReaderWriterLockSlim locker = new();
        private readonly List<SPrefixMappingPair> mappings = [];

        public bool Load(XmlElement element)
        {
            bool changed = false;
            element.Attributes.Cast<XmlAttribute>().Where(att => att.Name.StartsWith("xmlns:")).ForEach(att =>
            {
                string prefix = null;
                if (regBPMNRef.IsMatch(att.Value))
                    prefix = "bpmn";
                else if (regBPMNDIRef.IsMatch(att.Value))
                    prefix = "bpmndi";
                else if (regDIRef.IsMatch(att.Value))
                    prefix = "di";
                else if (regDCRef.IsMatch(att.Value))
                    prefix = "dc";
                else if (regXSIRef.IsMatch(att.Value))
                    prefix = "xsi";
                else if (regEXTSRef.IsMatch(att.Value))
                    prefix = "exts";
                if (prefix != null)
                {
                    changed = true;
                    process.WriteLogLine((string)null, LogLevel.Debug, new System.Diagnostics.StackFrame(1, true), DateTime.Now,
                        $"Mapping prefix {prefix} to {att.Name[(att.Name.IndexOf(':') + 1)..]}");
                    locker.EnterWriteLock();
                    var val = att.Name[(att.Name.IndexOf(':')+1)..];
                    if (!mappings.Exists(m => m.Prefix.Equals(prefix, StringComparison.InvariantCultureIgnoreCase)
                        && m.Value.Equals(val, StringComparison.InvariantCultureIgnoreCase)))
                        mappings.Add(new()
                        {
                            Prefix=prefix,
                            Value=val
                        });
                    locker.ExitWriteLock();
                }
            });
            return changed;
        }

        public IEnumerable<string> Translate(string prefix)
        {
            process.WriteLogLine((string)null, LogLevel.Debug, new System.Diagnostics.StackFrame(1, true), DateTime.Now, $"Attempting to translate xml prefix {prefix}");
            locker.EnterReadLock();
            var result = mappings.Where(m => m.Prefix.Equals(prefix, StringComparison.InvariantCultureIgnoreCase)).Select(m => m.Value).ToImmutableArray();
            locker.ExitReadLock();
            return result;
        }

        internal bool IsMatch(string prefix, string tag, string nodeName)
        {
            process.WriteLogLine((string)null, LogLevel.Debug, new System.Diagnostics.StackFrame(1, true), DateTime.Now, $"Checking if prefix {nodeName} matches {prefix}:{tag}");
            return string.Equals($"{prefix}:{tag}", nodeName, StringComparison.InvariantCultureIgnoreCase)
                ||Translate(prefix).Any(t => string.Equals($"{prefix}:{t}", nodeName, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
