using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Scripts
{
    internal abstract class ACompiledScript : AScript
    {
        private const string _NAME_CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvzwxyz";
        private const int _NAME_LENGTH = 32;

        private static Random _rand;

        private string _className;
        protected string _ClassName { get { return _className; } }
        private string _functionName;
        protected string _FunctionName { get { return _functionName; } }

        protected bool _IsCondition
        {
            get
            {
                XmlNode n = Element.ParentNode;
                while (n != null)
                {
                    if (n.Name == "conditionSet")
                        return true;
                    n = n.ParentNode;
                }
                return false;
            }
        }

        private string _Code
        {
            get
            {
                string code = "";
                if (SubNodes.Length > 1)
                {
                    foreach (XmlNode n in SubNodes)
                    {
                        if (n.NodeType == XmlNodeType.Element)
                        {
                            if (n.Name == "code")
                            {
                                code = n.InnerText;
                                break;
                            }
                        }
                    }
                }
                else
                    code = this.Element.InnerText;
                return code;
            }
        }

        private string[] _Imports
        {
            get
            {
                List<string> ret = new List<string>(new string[] { "Org.Reddragonit.BpmEngine" });
                if (SubNodes.Length > 1)
                {
                    foreach (XmlNode n in SubNodes)
                    {
                        if (n.NodeType == XmlNodeType.Element)
                        {
                            if (n.Name == "using")
                                ret.Add(n.InnerText);
                        }
                    }
                }
                return ret.ToArray();
            }
        }

        private string[] _Dlls
        {
            get
            {
                List<string> ret = new List<string>(new string[]{Assembly.GetAssembly(this.GetType()).Location});
                if (SubNodes.Length > 1)
                {
                    foreach (XmlNode n in SubNodes)
                    {
                        if (n.NodeType == XmlNodeType.Element)
                        {
                            if (n.Name == "dll")
                                ret.Add(n.InnerText);
                        }
                    }
                }
                return ret.ToArray();
            }
        }

        private Assembly _assembly;

        private static string _NextName()
        {
            string ret = "";
            while (ret.Length < _NAME_LENGTH)
            {
                ret += _NAME_CHARS[_rand.Next(_NAME_CHARS.Length - 1)];
            }
            return ret;
        }

        static ACompiledScript()
        {
            _rand = new Random(DateTime.Now.Millisecond);
        }

        public ACompiledScript(XmlElement elem, XmlPrefixMap map)
            : base(elem, map)
        {
            lock (_rand)
            {
                _className = _NextName();
                _functionName = _NextName();
            }
        }

        protected abstract string _GenerateCode(string[] imports,string code);
        protected abstract CodeDomProvider _CodeProvider { get; }
        
        protected sealed override object _Invoke(ProcessVariablesContainer variables)
        {
            lock (this)
            {
                if (_assembly == null)
                {
                    CompilerParameters compilerParams = new CompilerParameters
                        {
                            GenerateInMemory = true,
                            GenerateExecutable = false,
                            TreatWarningsAsErrors = false
                        };
                    compilerParams.ReferencedAssemblies.AddRange(_Dlls);
                    CompilerResults results = _CodeProvider.CompileAssemblyFromSource(compilerParams, new string[]{_GenerateCode(_Imports,_Code)});
                    if (results.Errors.Count > 0)
                    {
                        StringBuilder error = new StringBuilder();
                        foreach (CompilerError ce in results.Errors)
                            error.AppendLine(ce.ErrorText);
                        throw new Exception(string.Format("Unable to compile script Code.  Errors:{0}", error.ToString()));
                    }
                    _assembly = results.CompiledAssembly;
                }
            }
            object o = _assembly.CreateInstance(_className);
            MethodInfo mi = o.GetType().GetMethod(_functionName);
            object[] args = new object[] { variables };
            object ret = mi.Invoke(o, args);
            if (mi.ReturnType==typeof(void))
                ret = args[0];
            return ret;
        }
    }
}
