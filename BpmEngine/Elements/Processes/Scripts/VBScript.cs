using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CSharp;
using Microsoft.VisualBasic;
using Org.Reddragonit.BpmEngine.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Scripts
{
    [XMLTag("exts", "VBScript")]
    internal class VBScript : ACompiledScript
    {
        private const string _CODE_BASE_SCRIPT_TEMPLATE = @"{0}

Public Class {1}
    Public Sub New()
    End Sub

    Public Sub {2}(ByRef variables As IVariables)
        {3}
    End Sub
End Class";

        private const string _CODE_BASE_CONDITION_TEMPLATE = @"{0}

Public Class {1}
    Public Sub New()
    End Sub

    Public Function {2}(variables As IReadonlyVariables) AS Boolean
        {3}
    End Function
End Class";

        private const string _CODE_BASE_TIMER_EVENT_TEMPLATE = @"{0}

Public Class {1}
    Public Sub New()
    End Sub

    Public Function {2}(variables As IReadonlyVariables) AS DateTime
        {3}
    End Function
End Class";

        public VBScript(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent)
        { }

        protected override EmitResult _Compile(string name, IEnumerable<MetadataReference> references, IEnumerable<string> imports, string code, ref MemoryStream ms)
        {
            Info("Generating VB Code for script compilation for script element {0}", new object[] { id });
            StringBuilder sbUsing = new StringBuilder();
            imports.ForEach(str => sbUsing.AppendFormat("Imports {0}\n", str));
            string ccode = string.Format((_IsCondition ? _CODE_BASE_CONDITION_TEMPLATE : (_IsTimerEvent ? _CODE_BASE_TIMER_EVENT_TEMPLATE : _CODE_BASE_SCRIPT_TEMPLATE)), new object[]{
                sbUsing.ToString(),
                _ClassName,
                _FunctionName,
                code
            });
            List<SyntaxTree> tress = new List<SyntaxTree>();
            tress.Add(SyntaxFactory.SyntaxTree(VisualBasicSyntaxTree.ParseText(ccode).GetRoot()));
            VisualBasicCompilation comp = VisualBasicCompilation.Create(name, tress,references, new VisualBasicCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            return comp.Emit(ms);
        }
    }
}
