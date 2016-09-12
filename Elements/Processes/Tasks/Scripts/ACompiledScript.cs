using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Tasks.Scripts
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

        protected abstract Assembly _compileAssembly(Dictionary<string, string> providerOptions, CompilerParameters compilerParams, string[] imports, string code);
        
        protected sealed override void _Invoke(ref ProcessVariablesContainer variables)
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
                    _assembly = _compileAssembly(new Dictionary<string, string>(){
                            {"CompilerVersion", (_GetAttributeValue("compilerVersion") == null ? "v2.0" : _GetAttributeValue("compilerVersion"))}
                        },
                        compilerParams,
                        _Imports,
                        _Code
                    );
                }
            }
            object o = _assembly.CreateInstance(_className);
            MethodInfo mi = o.GetType().GetMethod(_functionName);
            object[] args = new object[] { variables };
            mi.Invoke(o, args);
            variables = (ProcessVariablesContainer)args[0];
        }
    }
}
