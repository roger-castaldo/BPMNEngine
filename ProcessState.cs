using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine
{
    public sealed class ProcessState
    {
        private ProcessPath _path;
        internal ProcessPath Path { get { return _path; } }

        private List<sVariableEntry> _variables;

        internal object this[string elementID, string variableName]
        {
            get
            {
                object ret = null;
                int stepIndex =-1;
                if (elementID == null)
                    stepIndex = _path.LastStep;
                else
                {
                    lock (_path)
                    {
                        stepIndex = _path.CurrentStepIndex(elementID);
                    }
                }
                lock (_variables)
                {
                    foreach (sVariableEntry sve in _variables)
                    {
                        if (sve.Name == variableName && sve.PathStepIndex <= stepIndex)
                            ret = sve.Value;
                    }
                }
                return ret;
            }
            set
            {
                lock (_variables)
                {
                    _variables.Add(new sVariableEntry(variableName, _path.CurrentStepIndex(elementID), value));
                    _variables.Sort();
                }
            }
        }

        internal string[] this[string elementID]
        {
            get
            {
                List<string> ret = new List<string>();
                int stepIndex = -1;
                if (elementID == null)
                    stepIndex = _path.LastStep;
                else
                {
                    lock (_path)
                    {
                        stepIndex = _path.CurrentStepIndex(elementID);
                    }
                }
                lock (_variables)
                {
                    foreach (sVariableEntry sve in _variables)
                    {
                        if (sve.PathStepIndex <= stepIndex)
                        {
                            if (sve.Value == null)
                                ret.Remove(sve.Name);
                            else if (!ret.Contains(sve.Name))
                                ret.Add(sve.Name);
                        }
                    }
                }
                return ret.ToArray();
            }
        }

        internal ProcessState(ProcessStepComplete complete, ProcessStepError error)
        {
            _path = new ProcessPath(complete,error);
            _variables = new List<sVariableEntry>();
        }

        internal bool Load(XmlDocument doc)
        {
            try
            {
                foreach (XmlNode n in doc.ChildNodes)
                {
                    if (n.NodeType == XmlNodeType.Element)
                    {
                        switch (n.Name)
                        {
                            case "ProcessState":
                                foreach (XmlNode nd in n.ChildNodes)
                                {
                                    if (nd.NodeType == XmlNodeType.Element)
                                    {
                                        switch (nd.Name)
                                        {
                                            case "ProcessPath":
                                                _path.Load((XmlElement)nd);
                                                break;
                                            case "ProcessVariables":
                                                foreach (XmlNode pnd in nd.ChildNodes)
                                                {
                                                    if (pnd.NodeType == XmlNodeType.Element)
                                                        _variables.Add(new sVariableEntry((XmlElement)pnd));
                                                }
                                                _variables.Sort();
                                                break;
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public XmlDocument Document
        {
            get
            {
                XmlDocument ret = new XmlDocument();
                try
                {
                    MemoryStream ms = new MemoryStream();
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Indent = true;
                    settings.Encoding = Encoding.UTF8;
                    XmlWriter writer = XmlWriter.Create(ms, settings);
                    writer.WriteStartDocument();
                    writer.WriteStartElement("ProcessState");
                    _path.Append(writer);
                    writer.WriteStartElement("ProcessVariables");
                    foreach (sVariableEntry sve in _variables)
                        sve.Append(writer);
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    writer.Flush();
                    System.Diagnostics.Debug.WriteLine(UTF8Encoding.UTF8.GetString(ms.ToArray()));
                    ms.Position = 0;
                    ret.Load(ms);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
                return ret;
            }
        }
    }
}
