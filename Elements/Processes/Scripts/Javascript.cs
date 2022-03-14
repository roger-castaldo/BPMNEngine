using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Scripts
{
    [XMLTag("exts", "Javascript")]
    internal class Javascript : AScript
    {
        private const string _codeExecReturnFormat = "(function(){{ {0} }})();";

        private static Assembly _jintAssembly;
        private static Type _engineType;
        private static MethodInfo _setValue;
        private static MethodInfo _execute;
        private static MethodInfo _toObject;
        private static MethodInfo _getCompletionValue;

        static Javascript()
        {
            try
            {
                _jintAssembly = Assembly.Load("Jint");
            }catch(Exception e)
            {
                _jintAssembly = null;
            }
            if (_jintAssembly == null)
            {
                try
                {
                    _jintAssembly = Assembly.LoadFile(typeof(Javascript).Assembly.Location.Replace("BpmEngine.dll", "Jint.dll"));
                }
                catch (Exception e)
                {
                    _jintAssembly = null;
                }
            }
            if (_jintAssembly != null)
            {
                _engineType = _jintAssembly.GetType("Jint.Engine");
                _setValue = _engineType.GetMethod("SetValue", new Type[] { typeof(string), typeof(object) });
                _execute = _engineType.GetMethod("Execute", new Type[] { typeof(string) });
                _getCompletionValue = _engineType.GetMethod("GetCompletionValue", Type.EmptyTypes);
                _toObject = _jintAssembly.GetType("Jint.Native.JsValue").GetMethod("ToObject");
            }
        }

        public Javascript(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        protected override object _Invoke(IVariables variables)
        {
            Info("Attempting to invoke Javascript script {0}", new object[] { id });
            if (_engineType == null)
                throw new Exception("Unable to process Javascript because the Jint.dll was not located in the Assembly path.");
            Debug("Creating new Javascript Engine for script element {0}", new object[] { id });
            object engine = _engineType.GetConstructor(Type.EmptyTypes).Invoke(new object[] { });
            object[] pars = new object[] { "variables", variables };
            Debug("Invoking Javascript Engine for script element {0}", new object[] { id });
            _setValue.Invoke(engine, pars);
            object ret = null;
            if (_Code.Contains("return "))
                ret = _getCompletionValue.Invoke(_execute.Invoke(engine, new object[] { string.Format(_codeExecReturnFormat, _Code) }), new object[] { });
            else 
                ret = _getCompletionValue.Invoke(_execute.Invoke(engine, new object[] { _Code }),new object[]{});
            if (_IsCondition)
                return bool.Parse(_toObject.Invoke(ret, new object[] { }).ToString());
            else if (_IsTimerEvent)
                return DateTime.Parse(_toObject.Invoke(ret, new object[] { }).ToString());
            return pars[1];
        }

        protected override object _Invoke(IReadonlyVariables variables)
        {
            Info("Attempting to invoke Javascript script {0}", new object[] { id });
            if (_engineType == null)
                throw new Exception("Unable to process Javascript because the Jint.dll was not located in the Assembly path.");
            Debug("Creating new Javascript Engine for script element {0}", new object[] { id });
            object engine = _engineType.GetConstructor(Type.EmptyTypes).Invoke(new object[] { });
            object[] pars = new object[] { "variables", variables };
            Debug("Invoking Javascript Engine for script element {0}", new object[] { id });
            _setValue.Invoke(engine, pars);
            object ret = null;
            if (_Code.Contains("return "))
                ret = _getCompletionValue.Invoke(_execute.Invoke(engine, new object[] { string.Format(_codeExecReturnFormat, _Code) }), new object[] { });
            else
                ret = _getCompletionValue.Invoke(_execute.Invoke(engine, new object[] { _Code }), new object[] { });
            if (_IsCondition)
                return bool.Parse(_toObject.Invoke(ret, new object[] { }).ToString());
            else if (_IsTimerEvent)
                return DateTime.Parse(_toObject.Invoke(ret, new object[] { }).ToString());
            return pars[1];
        }

        protected override bool _IsValid(out string[] err)
        {
            try
            {
                if (_engineType == null)
                    throw new Exception("Unable to process Javascript because the Jint.dll was not located in the Assembly path.");
                object engine = _engineType.GetConstructor(Type.EmptyTypes).Invoke(new object[] { });
                object[] pars = new object[] { "variables", new ProcessVariablesContainer() };
                _setValue.Invoke(engine, pars);
                try
                {
                    _execute.Invoke(engine, new object[] { _Code });
                }catch(Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        if (ex.InnerException.Message.Contains("Illegal return statement"))
                        {
                            try
                            {
                                _execute.Invoke(engine, new object[] { string.Format(_codeExecReturnFormat, _Code) });
                            }catch(Exception ecx)
                            {
                                throw ecx;
                            }
                        }else
                            throw ex;
                    }else
                        throw ex;
                }
            }
            catch(Exception e)
            {
                err = new string[] { e.Message };
                return false;
            }
            err = null;
            return true;
        }
    }
}
