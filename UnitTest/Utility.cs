using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace UnitTest
{
    internal static class Utility
    {
        public static Stream LoadResource(string path)
        {
            Stream ret = null;
            try
            {
                ret = typeof(Utility).Assembly.GetManifestResourceStream("UnitTest.diagrams."+path.Replace("/", "."));
            }
            catch (Exception ex) { }
            return ret;
        }

        public static XmlDocument LoadResourceDocument(string path)
        {
            Stream src = LoadResource(path);
            if (src==null)
                return null;
            XmlDocument ret = new XmlDocument();
            ret.Load(src);
            return ret;
        }
    }
}
