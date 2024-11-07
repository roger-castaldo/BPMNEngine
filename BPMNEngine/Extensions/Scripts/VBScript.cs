using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Elements;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.VisualBasic;
using System.Text;

namespace BPMNEngine.Extensions.Scripts
{
    [XMLTagAttribute("exts", "VBScript")]
    internal record VBScript : ACompiledScript
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

        public VBScript(XmlElement xmlElement, IBaseElement parent, ILogger? logger) 
            : base(xmlElement, parent, logger) {}

        protected override EmitResult Compile(string name, IEnumerable<MetadataReference> references, IEnumerable<string> imports, string code, out byte[] compiled, ILogger? logger)
        {
            logger?.LogInformation("Generating VB Code for script compilation for script element");
            var sbUsing = new StringBuilder();
            imports.ForEach(str => sbUsing.AppendFormat("Imports {0}\n", str));
            string ccode = string.Format(
                (IsCondition, IsTimerEvent) switch
                {
                    (true, _) => _CODE_BASE_CONDITION_TEMPLATE,
                    (_, true) => _CODE_BASE_TIMER_EVENT_TEMPLATE,
                    _ => _CODE_BASE_SCRIPT_TEMPLATE
                },
                sbUsing.ToString(),
                ClassName,
                FunctionName,
                code
            );
            VisualBasicCompilation comp = VisualBasicCompilation.Create(name, [SyntaxFactory.SyntaxTree(VisualBasicSyntaxTree.ParseText(ccode).GetRoot())],
                references, new VisualBasicCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            using MemoryStream ms = new();
            var result = comp.Emit(ms);
            compiled=ms.ToArray();
            return result;
        }
    }
}
