using Microsoft.CSharp;
using Microsoft.VisualBasic;
using Org.Reddragonit.BpmEngine.Attributes;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Tasks.Scripts
{
    [XMLTag("VBScript")]
    internal class VBScript : ACompiledScript
    {
        private const string _CODE_BASE_TEMPLATE = @"{0}

Public Class {1}
    Public Sub New()
    End Sub

    Public Sub {2}(ByRef variables As ProcessVariablesContainer)
        {3}
    End Sub
End Class";

        public VBScript(XmlElement elem, XmlPrefixMap map)
            : base(elem, map)
        { }

        protected override Assembly _compileAssembly(Dictionary<string, string> providerOptions, CompilerParameters compilerParams, string[] imports, string code)
        {
            StringBuilder sbUsing = new StringBuilder("Imports Org.Reddragonit.BpmEngine\n");
            foreach (string str in imports)
                sbUsing.AppendFormat("Imports {0}\n", str);

            VBCodeProvider provider = new VBCodeProvider(providerOptions);
            CompilerResults results = provider.CompileAssemblyFromSource(compilerParams, new string[]{string.Format(_CODE_BASE_TEMPLATE,new object[]{
                sbUsing.ToString(),
                _ClassName,
                _FunctionName,
                code
            })});
            if (results.Errors.Count > 0)
            {
                sbUsing = new StringBuilder();
                foreach (CompilerError ce in results.Errors)
                    sbUsing.AppendLine(ce.ErrorText);
                throw new Exception(string.Format("Unable to compile script Code.  Errors:{0}", sbUsing.ToString()));
            }

            return results.CompiledAssembly;
        }
    }
}
