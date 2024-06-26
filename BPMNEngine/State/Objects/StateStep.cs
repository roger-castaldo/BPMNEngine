using BPMNEngine.Interfaces.State;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace BPMNEngine.State.Objects
{
    internal record StateStep : IStateStep,IStateComponent
    {
        private static readonly Version ORIGINAL_VERSION = new("1.0");
        private const string _ORIGINAL_PATH_ENTRY_ELEMENT = "sPathEntry";
        private const string _STEP_STATUS = "status";
        private const string _ELEMENT_ID = "elementID";
        private const string _START_TIME = "startTime";
        private const string _END_TIME = "endTime";
        private const string _OUTGOING_ID = "outgoingID";
        private const string _INCOMING_ID = "incomingID";
        private const string _OUTGOING_ELEM = "outgoing";
        private const string _ORIGINAL_COMPLETED_BY = "CompletedByID";
        private const string _COMPLETED_BY = "completedByID";
        
        public string ElementID { get; private set; }
        public StepStatuses Status { get; private set; }
        public DateTime StartTime { get; private set; }
        public string IncomingID { get; private set; }
        public DateTime? EndTime { get; private set; }
        public string CompletedBy { get; private set; }
        public IImmutableList<string> OutgoingID { get; private set; }

        public StateStep() { }

        public StateStep(string elementID, StepStatuses status,DateTime start,string incomingID=null,DateTime? endTime=null,string completedBy=null,IImmutableList<string> outgoingID = null)
        {
            ElementID = elementID;
            Status = status;
            StartTime = start;
            IncomingID = incomingID;
            EndTime = endTime;
            CompletedBy = completedBy;
            OutgoingID = outgoingID;
        }

        public bool CanRead(XmlReader reader, Version version)
            => reader.NodeType==XmlNodeType.Element && (
                (reader.Name==_ORIGINAL_PATH_ENTRY_ELEMENT && version.Equals(ORIGINAL_VERSION))
                ||(reader.Name==GetType().Name && version>ORIGINAL_VERSION)
            );
        public bool CanRead(Utf8JsonReader reader, Version version)
            => reader.TokenType==JsonTokenType.StartObject;

        public XmlReader Read(XmlReader reader, Version version){
            ElementID = reader.GetAttribute(_ELEMENT_ID);
            Status = (StepStatuses)Enum.Parse(typeof(StepStatuses), reader.GetAttribute(_STEP_STATUS));
            StartTime = DateTime.ParseExact(reader.GetAttribute(_START_TIME), Constants.DATETIME_FORMAT, CultureInfo.InvariantCulture);
            IncomingID = reader.GetAttribute(_INCOMING_ID);
            EndTime = (reader.GetAttribute(_END_TIME)==null ? (DateTime?)null : DateTime.ParseExact(reader.GetAttribute(_END_TIME), Constants.DATETIME_FORMAT, CultureInfo.InvariantCulture));
            CompletedBy = (version>ORIGINAL_VERSION ? reader.GetAttribute(_COMPLETED_BY) : reader.GetAttribute(_ORIGINAL_COMPLETED_BY));
            IEnumerable<string> outgoing;
            if (reader.GetAttribute(_OUTGOING_ID)==null)
            {
                outgoing = new List<string>();
                reader.Read();
                while (reader.NodeType==XmlNodeType.Element && reader.Name==_OUTGOING_ELEM)
                {
                    reader.Read();
                    ((List<string>)outgoing).Add(reader.Value);
                    reader.Read();
                    if (reader.NodeType==XmlNodeType.EndElement && reader.Name==_OUTGOING_ELEM)
                        reader.Read();
                }
                if (reader.NodeType==XmlNodeType.EndElement && reader.Name==_ORIGINAL_PATH_ENTRY_ELEMENT)
                    reader.Read();
                OutgoingID = outgoing.ToImmutableArray();
            }
            else
            {
                OutgoingID = new string[] { reader.GetAttribute(_OUTGOING_ID) }.ToImmutableArray();
                reader.Read();
            }
            return reader;
        }
        public Utf8JsonReader Read(Utf8JsonReader reader, Version version)
        {
            reader.Read();
            while (reader.TokenType!=JsonTokenType.EndObject)
            {
                var propName = reader.GetString();
                reader.Read();
                switch (propName)
                {
                    case _ELEMENT_ID:
                        ElementID=reader.GetString();
                        break;
                    case _STEP_STATUS:
                        Status = Enum.Parse<StepStatuses>(reader.GetString());
                        break;
                    case _INCOMING_ID:
                        IncomingID=reader.GetString();
                        break;
                    case _START_TIME:
                        StartTime = DateTime.ParseExact(reader.GetString(), Constants.DATETIME_FORMAT, CultureInfo.InvariantCulture);
                        break;
                    case _END_TIME:
                        EndTime = DateTime.ParseExact(reader.GetString(), Constants.DATETIME_FORMAT, CultureInfo.InvariantCulture);
                        break;
                    case _COMPLETED_BY:
                        CompletedBy=reader.GetString();
                        break;
                    case _OUTGOING_ID:
                        var outgoing = new List<string>();
                        reader.Read();
                        while (reader.TokenType!=JsonTokenType.EndArray)
                        {
                            outgoing.Add(reader.GetString());
                            reader.Read();
                        }
                        OutgoingID = outgoing.ToImmutableList();
                        break;
                }
                reader.Read();
            }
            if (reader.TokenType==JsonTokenType.EndObject)
                reader.Read();
            return reader;
        }
        public void Write(XmlWriter writer){
            writer.WriteStartElement(GetType().Name);
            writer.WriteAttributeString(_ELEMENT_ID, ElementID);
            writer.WriteAttributeString(_STEP_STATUS, Status.ToString());
            writer.WriteAttributeString(_START_TIME, StartTime.ToString(Constants.DATETIME_FORMAT));
            if (IncomingID!=null)
                writer.WriteAttributeString(_INCOMING_ID, IncomingID);
            if (EndTime.HasValue)
                writer.WriteAttributeString(_END_TIME, EndTime.Value.ToString(Constants.DATETIME_FORMAT));
            if (!string.IsNullOrEmpty(CompletedBy))
                writer.WriteAttributeString(_COMPLETED_BY, CompletedBy);
            if (OutgoingID!=null)
            {
                if (OutgoingID.Count==1)
                    writer.WriteAttributeString(_OUTGOING_ID, OutgoingID[0]);
                else
                    OutgoingID.ForEach(outid =>
                    {
                        writer.WriteStartElement(_OUTGOING_ELEM);
                        writer.WriteValue(outid);
                        writer.WriteEndElement();
                    });
            }
            writer.WriteEndElement();
        }
        public void Write(Utf8JsonWriter writer){
            writer.WriteStartObject();
            writer.WritePropertyName(_ELEMENT_ID);
            writer.WriteStringValue(ElementID);
            writer.WritePropertyName(_STEP_STATUS);
            writer.WriteStringValue(Status.ToString());
            writer.WritePropertyName(_START_TIME);
            writer.WriteStringValue(StartTime.ToString(Constants.DATETIME_FORMAT));
            if (IncomingID!=null)
            {
                writer.WritePropertyName(_INCOMING_ID);
                writer.WriteStringValue(IncomingID);
            }
            if (EndTime.HasValue)
            {
                writer.WritePropertyName(_END_TIME);
                writer.WriteStringValue(EndTime.Value.ToString(Constants.DATETIME_FORMAT));
            }
            if (!string.IsNullOrEmpty(CompletedBy))
            {
                writer.WritePropertyName(_COMPLETED_BY);
                writer.WriteStringValue(CompletedBy);
            }
            if (OutgoingID!=null)
            {
                writer.WritePropertyName(_OUTGOING_ID);
                writer.WriteStartArray();
                OutgoingID.ForEach(outid =>
                {
                    writer.WriteStringValue(outid);
                });
                writer.WriteEndArray();
            }
            writer.WriteEndObject();
        }
    }
}
