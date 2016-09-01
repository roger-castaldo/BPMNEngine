using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Wrappers
{
    internal class WrappedXMLWriter
    {
        private const string BPMN_URI = "http://www.omg.org/spec/BPMN/20100524/MODEL";
        private const string BPMNDI_URI = "http://www.omg.org/spec/BPMN/20100524/DI";
        private const string DI_URI = "http://www.omg.org/spec/DD/20100524/DI";
        private const string DC_URI = "http://www.omg.org/spec/DD/20100524/DC";
        private const string XSI_URI = "http://www.w3.org/2001/XMLSchema-instance";

        private XmlWriter _writer;

        public WrappedXMLWriter(XmlWriter writer)
        {
            _writer = writer;
        }

        public void WriteStartDocument()
        {
            _writer.WriteStartDocument();
        }

        public void WriteStartElement(string name)
        {
            this.WriteStartElement(null, name);
        }

        public void WriteStartElement(string prefix, string name)
        {
            if (prefix != null)
                _writer.WriteStartElement(prefix, name, _GetNS(prefix));
            else
                _writer.WriteStartElement(name);
        }

        private string _GetNS(string prefix)
        {
            string ns = null;
            switch (prefix)
            {
                case "bpmn":
                    ns = BPMN_URI;
                    break;
                case "bpmndi":
                    ns = BPMNDI_URI;
                    break;
                case "di":
                    ns = DI_URI;
                    break;
                case "dc":
                    ns = DC_URI;
                    break;
                case "xsi":
                    ns = XSI_URI;
                    break;
            }
            return ns;
        }

        public void WriteAttributeString(string name, string value)
        {
            this.WriteAttributeString(null,name, value);
        }

        public void WriteAttributeString(string prefix,string name,string value)
        {
            if (prefix != null)
                _writer.WriteAttributeString(prefix, name, _GetNS(prefix), value);
            else
                _writer.WriteAttributeString(name, value);
        }

        

        public void WriteEndElement()
        {
            _writer.WriteEndElement();
        }

        public void Flush()
        {
            _writer.Flush();
        }

        public void WriteValue(object value)
        {
            _writer.WriteValue(value);
        }

        internal void WriteElement(XmlNode node)
        {
            node.WriteTo(_writer);
        }
    }
}
