using Microsoft.CSharp;
using Org.Reddragonit.BpmEngine.Attributes;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Scripts
{
    [XMLTag("cSharpScript")]
    internal class CSharpScript : ACompiledScript
    {
        private const string _CODE_BASE_SCRIPT_TEMPLATE = @"{0}

public class {1} {{
    public void {2}(ref ProcessVariablesContainer variables){{
        {3}
    }}
}}";

        private const string _CODE_BASE_CONDITION_TEMPLATE = @"{0}

public class {1} {{
    public bool {2}(ProcessVariablesContainer variables){{
        {3}
    }}
}}";

        public CSharpScript(XmlElement elem, XmlPrefixMap map)
            : base(elem, map)
        { }

        protected override string _GenerateCode(string[] imports,string code)
        {
            StringBuilder sbUsing = new StringBuilder();
            foreach (string str in imports)
                sbUsing.AppendFormat("using {0};\n", str);
            return string.Format((_IsCondition ? _CODE_BASE_CONDITION_TEMPLATE : _CODE_BASE_SCRIPT_TEMPLATE), new object[]{
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
    }
}
