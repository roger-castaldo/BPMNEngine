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

        public static bool AreHashtablesEqual(Hashtable left,Hashtable right)
        {
            if (
                (left==null && right!=null)||
                (left!=null&&right==null)
               )
                return false;
            else if (left==null&&right==null)
                return true;
            else if (left.Count!=right.Count)
                return false;
            foreach (object key in left.Keys)
            {
                if (!right.ContainsKey(key))
                    return false;
                else if (
                    (left[key]==null && right[key]!=null)||
                    (left[key]!=null && right[key]==null)
                    )
                    return false;
                else if (left[key]!=null&&right[key]!=null)
                {
                    if (!left[key].Equals(right[key]))
                        return false;
                }
            }
            foreach (object key in right.Keys)
            {
                if (!left.ContainsKey(key))
                    return false;
            }
            return true;
        }

        public static bool StepCompleted(XmlDocument state,string name)
        {
            return state.SelectSingleNode(string.Format("/ProcessState/ProcessPath/sPathEntry[@elementID='{0}'][@status='Succeeded']", name))!=null;
        }
    }
}
