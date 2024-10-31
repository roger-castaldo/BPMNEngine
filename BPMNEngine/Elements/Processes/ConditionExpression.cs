using BPMNEngine.Attributes;
using BPMNEngine.Interfaces;
using BPMNEngine.Interfaces.Elements;

namespace BPMNEngine.Elements.Processes
{
    [XMLTagAttribute("bpmn", "conditionExpression")]
    [ValidParent(typeof(SequenceFlow))]
    internal record ConditionExpression : AElement
    {
        public string? Value { get; private init; }

        public ConditionExpression(XmlElement elem, IBaseElement? parent, IElementFactory elementFactory) 
            : base(elem, parent, elementFactory)
        {
            Value = elem.ChildNodes.Cast<XmlNode>()
                .Select(x => x is XmlCDataSection section ? section.InnerText : x.InnerText)
                .FirstOrDefault();
        }
    }
}
