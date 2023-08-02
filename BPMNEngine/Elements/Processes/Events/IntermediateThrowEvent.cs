using BPMNEngine.Attributes;
using BPMNEngine.Elements.Processes.Events.Definitions;

namespace BPMNEngine.Elements.Processes.Events
{
    [XMLTag("bpmn", "intermediateThrowEvent")]
    internal class IntermediateThrowEvent : AEvent
    {
        public string Message
        {
            get
            {
                if (SubType.HasValue)
                {
                    switch (SubType.Value)
                    {
                        case EventSubTypes.Signal:
                            return Children
                                .OfType<SignalEventDefinition>()
                                .Select(child => child.SignalTypes.First())
                                .FirstOrDefault();
                        case EventSubTypes.Error:
                            return Children
                                .OfType<ErrorEventDefinition>()
                                .Select(child => child.ErrorTypes.First())
                                .FirstOrDefault();
                        case EventSubTypes.Message:
                            return Children
                                .OfType<MessageEventDefinition>()
                                .Select(child => child.MessageTypes.First())
                                .FirstOrDefault();
                    }
                }
                return null;
            }
        }

        public IntermediateThrowEvent(XmlElement elem, XmlPrefixMap map,AElement parent)
            : base(elem, map,parent) { }

        public override bool IsValid(out IEnumerable<string> err)
        {
            var res = base.IsValid(out err);
            if (!Incoming.Any())
            {
                err = (err??Array.Empty<string>()).Concat(new string[] { "Intermediate Throw Events must have an incoming path." });
                res=false;
            }else if (Incoming.Count()!= 1)
            {
                err = (err??Array.Empty<string>()).Concat(new string[] { "Intermediate Throw Events must have only 1 incoming path." });
                res=false;
            }
            if (SubType.HasValue)
            {
                switch (SubType.Value)
                {
                    case EventSubTypes.Signal:
                        if (!Children.Any(child=>child is SignalEventDefinition definition && definition.SignalTypes.Any()))
                        {
                            err = (err??Array.Empty<string>()).Concat(new string[] { "Intermediate Throw Signal Events must specify a singal type." });
                            return false;
                        }else if (Children.Any(child => child is SignalEventDefinition definition && definition.SignalTypes.Any() && definition.SignalTypes.Count()!=1))
                        {
                            err = (err??Array.Empty<string>()).Concat(new string[] { "Intermediate Throw Signal Events can only specify a single singal." });
                            return false;
                        }
                        break;
                    case EventSubTypes.Error:
                        if (!Children.Any(child => child is ErrorEventDefinition definition && definition.ErrorTypes.Any()))
                        {
                            err = (err??Array.Empty<string>()).Concat(new string[] { "Intermediate Throw Error Events must specify an error." });
                            return false;
                        }
                        else if (Children.Any(child => child is ErrorEventDefinition definition && definition.ErrorTypes.Any() && definition.ErrorTypes.Count()!=1))
                        {
                            err = (err??Array.Empty<string>()).Concat(new string[] { "Intermediate Throw Error Events can only specify a single error." });
                            return false;
                        }
                        break;
                    case EventSubTypes.Message:
                        if (!Children.Any(child => child is MessageEventDefinition definition && definition.MessageTypes.Any()))
                        {
                            err = (err??Array.Empty<string>()).Concat(new string[] { "Intermediate Throw Message Events must specify a message." });
                            return false;
                        }
                        else if (Children.Any(child => child is MessageEventDefinition definition && definition.MessageTypes.Any() && definition.MessageTypes.Count()!=1))
                        {
                            err = (err??Array.Empty<string>()).Concat(new string[] { "Intermediate Throw Message Events can only specify a single message." });
                            return false;
                        }
                        break;
                }
            }
            return res;
        }
    }
}
