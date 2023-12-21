using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Variables;
using System;
using System.Reflection;

namespace BPMNEngine.Elements.Processes.Scripts
{
    [XMLTag("exts", "Javascript")]
    internal class Javascript : AScript
    {
        private const string _codeExecReturnFormat = "(function(){{ {0} }})();";

        private static readonly Assembly _jintAssembly;
        private static readonly Type _engineType;
        private static readonly MethodInfo _setValue;
        private static readonly MethodInfo _execute;
        private static readonly MethodInfo _toObject;
        private static readonly MethodInfo _getCompletionValue;

        static Javascript()
        {
            try
            {
                _jintAssembly = Assembly.Load("Jint");
            }catch(Exception)
            {
                _jintAssembly = null;
            }
            if (_jintAssembly == null)
            {
                try
                {
                    _jintAssembly = Assembly.LoadFile(typeof(Javascript).Assembly.Location.Replace("BPMNEngine.dll", "Jint.dll"));
                }
                catch (Exception)
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

        protected override object ScriptInvoke(IVariables variables)
        {
            Info("Attempting to invoke Javascript script {0}", new object[] { ID });
            if (_engineType == null)
                throw new Exception("Unable to process Javascript because the Jint.dll was not located in the Assembly path.");
            Debug("Creating new Javascript Engine for script element {0}", new object[] { ID });
            object engine = _engineType.GetConstructor(Type.EmptyTypes).Invoke(Array.Empty<object>());
            object[] pars = new object[] { "variables", variables };
            Debug("Invoking Javascript Engine for script element {0}", new object[] { ID });
            _setValue.Invoke(engine, pars);
            object ret;
            if (Code.Contains("return "))
                ret = _getCompletionValue.Invoke(_execute.Invoke(engine, new object[] { string.Format(_codeExecReturnFormat, Code) }), Array.Empty<object>());
            else 
                ret = _getCompletionValue.Invoke(_execute.Invoke(engine, new object[] { Code }), Array.Empty<object>());
            if (IsCondition)
                return bool.Parse(_toObject.Invoke(ret, Array.Empty<object>()).ToString());
            else if (IsTimerEvent)
                return DateTime.Parse(_toObject.Invoke(ret, Array.Empty<object>()).ToString());
            return pars[1];
        }

        protected override object ScriptInvoke(IReadonlyVariables variables)
        {
            Info("Attempting to invoke Javascript script {0}", new object[] { ID });
            if (_engineType == null)
                throw new Exception("Unable to process Javascript because the Jint.dll was not located in the Assembly path.");
            Debug("Creating new Javascript Engine for script element {0}", new object[] { ID });
            object engine = _engineType.GetConstructor(Type.EmptyTypes).Invoke(Array.Empty<object>());
            object[] pars = new object[] { "variables", variables };
            Debug("Invoking Javascript Engine for script element {0}", new object[] { ID });
            _setValue.Invoke(engine, pars);
            object ret;
            if (Code.Contains("return "))
                ret = _getCompletionValue.Invoke(_execute.Invoke(engine, new object[] { string.Format(_codeExecReturnFormat, Code) }), Array.Empty<object>());
            else
                ret = _getCompletionValue.Invoke(_execute.Invoke(engine, new object[] { Code }), Array.Empty<object>());
            if (IsCondition)
                return bool.Parse(_toObject.Invoke(ret, Array.Empty<object>()).ToString());
            else if (IsTimerEvent)
                return ConvertJsDateToDateTime(_toObject.Invoke(ret, Array.Empty<object>()));
            return pars[1];
        }

        static DateTime ConvertJsDateToDateTime(object jsDate)
        {
            if (jsDate is double timestamp)
            {
                var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                return unixEpoch.AddMilliseconds(timestamp).ToLocalTime();
            }
            else if (jsDate is DateTime time)
                return TimeZoneInfo.ConvertTime(time, TimeZoneInfo.Local);

            throw new ArgumentException("Invalid JavaScript Date format.");
        }

        protected override bool ScriptIsValid(out IEnumerable<string> err)
        {
            try
            {
                if (_engineType == null)
                    throw new Exception("Unable to process Javascript because the Jint.dll was not located in the Assembly path.");
                object engine = _engineType.GetConstructor(Type.EmptyTypes).Invoke(Array.Empty<object>());
                object[] pars = new object[] { "variables", new ProcessVariablesContainer(null,((Definition)this.Definition).OwningProcess)};
                _setValue.Invoke(engine, pars);
                try
                {
                    _execute.Invoke(engine, new object[] { Code });
                }catch(Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        if (ex.InnerException.Message.Contains("Illegal return statement"))
                            _execute.Invoke(engine, new object[] { string.Format(_codeExecReturnFormat, Code) });
                        else
                            throw;
                    }
                    else
                        throw;
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
