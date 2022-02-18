using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements
{
    internal abstract class AParentElement : AElement,IParentElement
    {

        private List<IElement> _children=null;
        public IElement[] Children
        {
            get {   
                return _children.ToArray();
            }
        }

        public void LoadChildren(ref XmlPrefixMap map, ref ElementTypeCache cache, ref ConcurrentQueue<System.Threading.Tasks.Task> loadTasks)
        {
            XmlPrefixMap smap = map;
            ElementTypeCache scache = cache;
            ConcurrentQueue<System.Threading.Tasks.Task> sloadTasks = loadTasks;
            loadTasks.Enqueue(System.Threading.Tasks.Task.Run(() =>
            {
                if (SubNodes != null)
                {
                    foreach (XmlNode n in SubNodes)
                    {
                        if (n.NodeType == XmlNodeType.Element)
                        {
                            IElement subElem = Utility.ConstructElementType((XmlElement)n, ref smap, ref scache, this);
                            if (subElem != null)
                            {
                                if (_children == null)
                                    _children = new List<IElement>();
                                _children.Add(subElem);
                                if (subElem is AParentElement)
                                    ((AParentElement)subElem).LoadChildren(ref smap, ref scache, ref sloadTasks);
                            }
                        }
                    }
                }
            }));
        }

        public AParentElement(XmlElement elem,XmlPrefixMap map, AElement parent)
            : base(elem,map,parent)
        {
            _children=new List<IElement>();
        }
    }
}
