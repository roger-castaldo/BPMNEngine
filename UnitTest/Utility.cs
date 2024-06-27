using BPMNEngine;
using BPMNEngine.Interfaces;
using BPMNEngine.Interfaces.State;
using System;
using System.IO;
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
            var ret = new XmlDocument();
            ret.Load(src);
            return ret;
        }

        public static bool StepCompleted(IState state, string name)
            => StepAchievedStatus(state, name, StepStatuses.Succeeded);
        public static bool StepCompleted(XmlDocument doc, string name)
            => StepAchievedStatus(doc, name, StepStatuses.Succeeded);
        public static bool StepAborted(IState state, string name)
            => StepAchievedStatus(state, name, StepStatuses.Aborted);
        public static bool StepAborted(XmlDocument doc, string name)
            => StepAchievedStatus(doc, name, StepStatuses.Aborted);

        public static bool StepAchievedStatus(IState state, string name, StepStatuses status)
        {
            var doc = new XmlDocument();
            doc.LoadXml(state.AsXMLDocument);
            return StepAchievedStatus(doc, name, status);
        }

        public static bool StepAchievedStatus(XmlDocument doc, string name, StepStatuses status, string? CompletedBy = null)
            => doc.SelectSingleNode($"/ProcessState/ProcessPath/StateStep[@elementID='{name}'][@status='{status}']{(!string.IsNullOrEmpty(CompletedBy) ? $"[@completedByID='{CompletedBy}']" : "")}")!=null;

        private static readonly TimeSpan DEFAULT_PROCESS_WAIT = TimeSpan.FromMinutes(5);

        public static bool WaitForCompletion(IProcessInstance instance, TimeSpan? waitTime = null)
        {
            var result = instance.WaitForCompletion(waitTime??DEFAULT_PROCESS_WAIT);
            if (!result)
                Console.WriteLine(instance.CurrentState.AsXMLDocument);
            return result;
        }
    }
}
