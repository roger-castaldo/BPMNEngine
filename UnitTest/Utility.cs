using BpmEngine.Interfaces;
using System;
using System.Collections;
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
            catch (Exception) { }
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

        public static bool StepCompleted(IState state,string name)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(state.AsXMLDocument);
            return doc.SelectSingleNode(string.Format("/ProcessState/ProcessPath/sPathEntry[@elementID='{0}'][@status='Succeeded']", name))!=null;
        }

        public static bool StepAborted(IState state, string name)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(state.AsXMLDocument);
            return doc.SelectSingleNode(string.Format("/ProcessState/ProcessPath/sPathEntry[@elementID='{0}'][@status='Aborted']", name))!=null;
        }

        private static readonly TimeSpan DEFAULT_PROCESS_WAIT = TimeSpan.FromMinutes(5);

        public static bool WaitForCompletion(IProcessInstance instance,TimeSpan? waitTime=null)
        {
            var result = instance.WaitForCompletion(waitTime??DEFAULT_PROCESS_WAIT);
            if (!result)
                Console.WriteLine(instance.CurrentState.AsXMLDocument);
            return result;
        }
    }
}
