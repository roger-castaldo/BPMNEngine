using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Elements;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine
{
    internal static class Utility
    {
        //Called to locate all child classes of a given parent type
        public static List<Type> LocateTypeInstances(Type parent)
        {
            List<Type> ret = new List<Type>();
            foreach (Assembly ass in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    if (ass.GetName().Name != "mscorlib" && !ass.GetName().Name.StartsWith("System.") && ass.GetName().Name != "System" && !ass.GetName().Name.StartsWith("Microsoft"))
                    {
                        foreach (Type t in ass.GetTypes())
                        {
                            if (t.IsSubclassOf(parent) || (parent.IsInterface && new List<Type>(t.GetInterfaces()).Contains(parent)))
                                ret.Add(t);
                        }
                    }
                }
                catch (Exception e)
                {
                    if (e.Message != "The invoked member is not supported in a dynamic assembly."
                        && e.Message != "Unable to load one or more of the requested types. Retrieve the LoaderExceptions property for more information.")
                    {
                        throw e;
                    }
                }
            }
            return ret;
        }

        public static Type LocateElementType(string tagName,XmlPrefixMap map)
        {
            Type ret = null;
            foreach (Type t in LocateTypeInstances(typeof(IElement)))
            {
                if (t.GetCustomAttributes(typeof(XMLTag), false).Length > 0)
                {
                    foreach (XMLTag xt in t.GetCustomAttributes(typeof(XMLTag), false))
                    {
                        if (xt.Matches(map,tagName))
                        {
                            ret = t;
                            break;
                        }
                    }
                    if (ret != null)
                        break;
                }
            }
            return ret;
        }

        //called to open a stream of a given embedded resource, again searches through all assemblies
        public static Stream LocateEmbededResource(string name)
        {
            Stream ret = typeof(Utility).Assembly.GetManifestResourceStream(name);
            if (ret == null)
            {
                foreach (Assembly ass in AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        if (ass.GetName().Name != "mscorlib" && !ass.GetName().Name.StartsWith("System.") && ass.GetName().Name != "System" && !ass.GetName().Name.StartsWith("Microsoft"))
                        {
                            ret = ass.GetManifestResourceStream(name);
                            if (ret != null)
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        if (e.Message != "The invoked member is not supported in a dynamic assembly.")
                        {
                            throw e;
                        }
                    }
                }
            }
            return ret;
        }

        internal static IElement ConstructElementType(XmlElement element, XmlPrefixMap map,AElement parent)
        {
            Type t = Utility.LocateElementType(element.Name, map);
            if (t != null)
                return (IElement)t.GetConstructor(new Type[] { typeof(XmlElement), typeof(XmlPrefixMap),typeof(AElement) }).Invoke(new object[] { element, map,parent });
            return null;
        }

        public static string FindXPath(XmlNode node)
        {
            StringBuilder builder = new StringBuilder();
            while (node != null)
            {
                switch (node.NodeType)
                {
                    case XmlNodeType.Attribute:
                        builder.Insert(0, "/@" + node.Name);
                        node = ((XmlAttribute)node).OwnerElement;
                        break;
                    case XmlNodeType.Element:
                        int index = FindElementIndex((XmlElement)node);
                        builder.Insert(0, "/" + node.Name + "[" + index + "]");
                        node = node.ParentNode;
                        break;
                    case XmlNodeType.Document:
                        return builder.ToString();
                    default:
                        throw new ArgumentException("Only elements and attributes are supported");
                }
            }
            throw new ArgumentException("Node was not in a document");
        }

        public static int FindElementIndex(XmlElement element)
        {
            XmlNode parentNode = element.ParentNode;
            if (parentNode is XmlDocument)
            {
                return 1;
            }
            XmlElement parent = (XmlElement)parentNode;
            int index = 1;
            foreach (XmlNode candidate in parent.ChildNodes)
            {
                if (candidate is XmlElement && candidate.Name == element.Name)
                {
                    if (candidate == element)
                    {
                        return index;
                    }
                    index++;
                }
            }
            throw new ArgumentException("Couldn't find element within parent");
        }
    }
}
