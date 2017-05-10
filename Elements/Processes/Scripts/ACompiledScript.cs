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
                            if (n.Name.ToLower() == "using")
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
                            if (n.Name.ToLower() == "dll")
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

        public ACompiledScript(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent)
        {
            lock (_rand)
            {
                _className = _NextName();
                _functionName = _NextName();
            }
        }

        protected abstract string _GenerateCode(string[] imports,string code);
        protected abstract CodeDomProvider _CodeProvider { get; }
        
        private bool _CompileAssembly(out string errors)
        {
            errors = null;
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
                    CompilerResults results = _CodeProvider.CompileAssemblyFromSource(compilerParams, new string[] { _GenerateCode(_Imports, _Code) });
                    if (results.Errors.Count > 0)
                    {
                        StringBuilder error = new StringBuilder();
                        foreach (CompilerError ce in results.Errors)
                            error.AppendLine(ce.ErrorText);
                        errors = string.Format("Unable to compile script Code.  Errors:{0}", error.ToString());
                        _assembly = null;
                    }else
                        _assembly = results.CompiledAssembly;
                }
            }
            return errors == null;
        }

        protected sealed override object _Invoke(ProcessVariablesContainer variables)
        {
            Log.Debug("Attempting to compile script to execute for script element {0}",new object[] { id });
            string errors;
            if (!_CompileAssembly(out errors))
                throw new Exception(errors);
            Log.Debug("Creating new instance of compiled script class for script element {0}", new object[] { id });
            object o = _assembly.CreateInstance(_className);
            Log.Debug("Accesing method from new instance of compiled script class for script element {0}", new object[] { id });
            MethodInfo mi = o.GetType().GetMethod(_functionName);
            object[] args = new object[] { variables };
            Log.Debug("Executing method from new instance of compiled script class for script element {0}", new object[] { id });
            object ret = mi.Invoke(o, args);
            if (mi.ReturnType == typeof(void))
            {
                Log.Debug("Collecting the returned value from new instance of compiled script class for script element {0}", new object[] { id });
                ret = args[0];
            }
            return ret;
        }

        protected override bool _IsValid(out string[] err)
        {
            _assembly = null;
            string error;
            if (!_CompileAssembly(out error))
            {
                err = new string[] { error };
                return false;
            }
            else
                err = null;
            return true;
        }
    }
}
