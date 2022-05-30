#if !NET461
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
#else
using System.CodeDom.Compiler;
#endif
using Microsoft.CSharp;
using Org.Reddragonit.BpmEngine.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Scripts
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

#if !NET461
        protected override EmitResult _Compile(string name, List<MetadataReference> references, string[] imports, string code, ref MemoryStream ms)
        {
            Info("Generating C# Code for script compilation for script element {0}",new object[] { id });
            StringBuilder sbUsing = new StringBuilder();
            foreach (string str in imports)
                sbUsing.AppendFormat("using {0};\n", str);
            string ccode = string.Format((_IsCondition ? _CODE_BASE_CONDITION_TEMPLATE : (_IsTimerEvent ? _CODE_BASE_TIMER_EVENT_TEMPLATE : _CODE_BASE_SCRIPT_TEMPLATE)), new object[]{
                sbUsing.ToString(),
                _ClassName,
                _FunctionName,
                code
            });
            List<SyntaxTree> tress = new List<SyntaxTree>();
            tress.Add(SyntaxFactory.SyntaxTree(CSharpSyntaxTree.ParseText(ccode).GetRoot()));
            CSharpCompilation comp = CSharpCompilation.Create(name, tress,references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            return comp.Emit(ms);
        }
#else
        protected override string _GenerateCode(string[] imports,string code)
        {
            Info("Generating C# Code for script compilation for script element {0}",new object[] { id });
            StringBuilder sbUsing = new StringBuilder();
            foreach (string str in imports)
                sbUsing.AppendFormat("using {0};\n", str);
            return string.Format((_IsCondition ? _CODE_BASE_CONDITION_TEMPLATE : (_IsTimerEvent ? _CODE_BASE_TIMER_EVENT_TEMPLATE : _CODE_BASE_SCRIPT_TEMPLATE)), new object[]{
                sbUsing.ToString(),
                _ClassName,
                _FunctionName,
                code
            });
        }

        protected override CodeDomProvider _CodeProvider
        {
            get { return new CSharpCodeProvider(); }
        }
#endif
    }
}
