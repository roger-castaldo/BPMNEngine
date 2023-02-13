using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using Org.Reddragonit.BpmEngine.Interfaces;
using System.Linq;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Scripts
{
    internal abstract class ACompiledScript : AScript
    {
        private const string _NAME_CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvzwxyz";
        private const int _NAME_LENGTH = 32;

        private static Random _rand;

        private readonly string _className;
        protected string _ClassName { get { return _className; } }
        private readonly string _functionName;
        protected string _FunctionName { get { return _functionName; } }

        private IEnumerable<string> _Imports
            => new string[] { "System", "Org.Reddragonit.BpmEngine", "Org.Reddragonit.BpmEngine.Interfaces", "System.Linq" }
            .Concat(SubNodes
                .Where(n => n.NodeType==XmlNodeType.Element && n.Name.ToLower()=="using")
                .Select(n => n.InnerText)
            );

        private IEnumerable<string> _Dlls
            => new string[] { Assembly.GetAssembly(this.GetType()).Location }
            .Concat(SubNodes
                .Where(n => n.NodeType==XmlNodeType.Element && n.Name.ToLower()=="dll")
                .Select(n => n.InnerText)
            );
        
        private Assembly _assembly;

        private static string _NextName()
        {
            StringBuilder result = new StringBuilder();
            while (result.Length < _NAME_LENGTH)
            {
                result.Append(_NAME_CHARS[_rand.Next(_NAME_CHARS.Length - 1)]);
            }
            return result.ToString();
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

        protected abstract EmitResult _Compile(string name, IEnumerable<MetadataReference> references, IEnumerable<string> imports, string code, ref MemoryStream ms);
        
        private bool _CompileAssembly(out string errors)
        {
            errors = null;
            lock (this)
            {
                if (_assembly == null)
                {
                    MemoryStream ms = new MemoryStream();
                    var references = AppDomain.CurrentDomain.GetAssemblies()
                        .Where(ass => GetAssemblyLocation(ass) != null)
                        .Select(ass => MetadataReference.CreateFromFile(GetAssemblyLocation(ass)))
                        .Concat(
                            _Dlls
                            .Select(d=> MetadataReference.CreateFromFile(d))
                        );
                    EmitResult res = _Compile(_NextName(), references, _Imports, _Code, ref ms);
                    if (!res.Success)
                    {
                        StringBuilder error = new StringBuilder();
                        foreach (Diagnostic diag in res.Diagnostics)
                            error.AppendLine(diag.ToString());
                        errors = string.Format("Unable to compile script Code.  Errors:{0}", error.ToString());
                        _assembly = null;
                    }
                    else
                        _assembly = Assembly.Load(ms.ToArray());
                }
            }
            return errors == null;
        }

        protected static string GetAssemblyLocation(Assembly ass)
        {
            try
            {
                return ass.Location=="" ? null : ass.Location;
            }catch(Exception)
            {
                return null;
            }
        }

        protected sealed override object _Invoke(IVariables variables)
        {
            Debug("Attempting to compile script to execute for script element {0}",new object[] { id });
            string errors;
            if (!_CompileAssembly(out errors))
                throw new Exception(errors);
            Debug("Creating new instance of compiled script class for script element {0}", new object[] { id });
            object o = _assembly.CreateInstance(_className);
            Debug("Accesing method from new instance of compiled script class for script element {0}", new object[] { id });
            MethodInfo mi = o.GetType().GetMethod(_functionName);
            object[] args = new object[] { variables };
            Debug("Executing method from new instance of compiled script class for script element {0}", new object[] { id });
            object ret = mi.Invoke(o, args);
            if (mi.ReturnType == typeof(void))
            {
                Debug("Collecting the returned value from new instance of compiled script class for script element {0}", new object[] { id });
                ret = args[0];
            }
            return ret;
        }

        protected sealed override object _Invoke(IReadonlyVariables variables)
        {
            Debug("Attempting to compile script to execute for script element {0}", new object[] { id });
            string errors;
            if (!_CompileAssembly(out errors))
                throw new Exception(errors);
            Debug("Creating new instance of compiled script class for script element {0}", new object[] { id });
            object o = _assembly.CreateInstance(_className);
            Debug("Accesing method from new instance of compiled script class for script element {0}", new object[] { id });
            MethodInfo mi = o.GetType().GetMethod(_functionName);
            object[] args = new object[] { variables };
            Debug("Executing method from new instance of compiled script class for script element {0}", new object[] { id });
            object ret = mi.Invoke(o, args);
            if (mi.ReturnType == typeof(void))
            {
                Debug("Collecting the returned value from new instance of compiled script class for script element {0}", new object[] { id });
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
