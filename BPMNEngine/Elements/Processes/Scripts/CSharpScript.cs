using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using BPMNEngine.Attributes;
using System.IO;
using System.Text;

namespace BPMNEngine.Elements.Processes.Scripts
{
    [XMLTag("exts", "cSharpScript")]
    internal class CSharpScript : ACompiledScript
    {
        private const string _CODE_BASE_SCRIPT_TEMPLATE = @"{0}

public class {1} {{
    public void {2}(ref IVariables variables){{
        {3}
    }}
}}";

        private const string _CODE_BASE_CONDITION_TEMPLATE = @"{0}

public class {1} {{
    public bool {2}(IReadonlyVariables variables){{
        {3}
    }}
}}";

        private const string _CODE_BASE_TIMER_EVENT_TEMPLATE = @"{0}

public class {1} {{
    public DateTime {2}(IReadonlyVariables variables){{
        {3}
    }}
}}";

        public CSharpScript(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent)
        { }

        protected override EmitResult Compile(string name, IEnumerable<MetadataReference> references, IEnumerable<string> imports, string code, out byte[] compiled)
        {
            Info("Generating C# Code for script compilation for script element {0}",new object[] { ID });
            var sbUsing = new StringBuilder();
            imports.ForEach(str=>sbUsing.AppendFormat("using {0};\n", str));
            string ccode = string.Format(IsCondition ? _CODE_BASE_CONDITION_TEMPLATE : (IsTimerEvent ? _CODE_BASE_TIMER_EVENT_TEMPLATE : _CODE_BASE_SCRIPT_TEMPLATE), new object[]{
                sbUsing.ToString(),
                ClassName,
                FunctionName,
                code
            });
            CSharpCompilation comp = CSharpCompilation.Create(name, new[] { SyntaxFactory.SyntaxTree(CSharpSyntaxTree.ParseText(ccode).GetRoot()) },
                references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            using MemoryStream ms = new();
            var result = comp.Emit(ms);
            compiled=ms.ToArray();
            return result;
        }
    }
}
