using Microsoft.CSharp;
using Microsoft.VisualBasic;
using Org.Reddragonit.BpmEngine.Attributes;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
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

    Public Sub {2}(ByRef variables As ProcessVariablesContainer)
        {3}
    End Sub
End Class";

        private const string _CODE_BASE_CONDITION_TEMPLATE = @"{0}

Public Class {1}
    Public Sub New()
    End Sub

    Public Function {2}(ByRef variables As ProcessVariablesContainer) AS Boolean
        {3}
    End Sub
End Class";

        private const string _CODE_BASE_TIMER_EVENT_TEMPLATE = @"{0}

Public Class {1}
    Public Sub New()
    End Sub

    Public Function {2}(ByRef variables As ProcessVariablesContainer) AS DateTime
        {3}
    End Sub
End Class";

        public VBScript(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent)
        { }

        protected override string _GenerateCode(string[] imports, string code)
        {
            Log.Info("Generating VB Code for script compilation for script element {0}", new object[] { id });
            StringBuilder sbUsing = new StringBuilder();
            foreach (string str in imports)
                sbUsing.AppendFormat("Imports {0}\n", str);

            return string.Format((_IsCondition ? _CODE_BASE_CONDITION_TEMPLATE : (_IsTimerEvent ? _CODE_BASE_TIMER_EVENT_TEMPLATE : _CODE_BASE_SCRIPT_TEMPLATE)), new object[]{
                sbUsing.ToString(),
                _ClassName,
                _FunctionName,
                code
            });
        }

        protected override CodeDomProvider _CodeProvider
        {
            get { return new VBCodeProvider(); }
        }
    }
}
