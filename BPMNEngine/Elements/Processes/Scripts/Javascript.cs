using BPMNEngine.Attributes;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

namespace BPMNEngine.Elements.Processes.Scripts
{
    [XMLTagAttribute("exts", "Javascript")]
    internal record Javascript : AScript
    {
        private const string _codeExecReturnFormat = "(function(){{ {0} }})();";

        private static readonly Assembly _jintAssembly;
        private static readonly Type _engineType;
        private static readonly MethodInfo _setValue;
        private static readonly MethodInfo _evaluate;
        private static readonly MethodInfo _toObject;

        [ExcludeFromCodeCoverage(Justification = "This portion of the code is used to dynamically load the Jint.dll during runtime and cannot be properly tested")]
        static Javascript()
        {
            try
            {
                _jintAssembly = Assembly.Load("Jint");
            }
            catch (Exception)
            {
                _jintAssembly = null;
            }
            if (_jintAssembly == null)
            {
                try
                {
#pragma warning disable S3885 // "Assembly.Load" should be used
                    _jintAssembly = Assembly.LoadFrom(typeof(Javascript).Assembly.Location.Replace("BPMNEngine.dll", "Jint.dll"));
#pragma warning restore S3885 // "Assembly.Load" should be used
                }
                catch (Exception)
                {
                    _jintAssembly = null;
                }
            }
            if (_jintAssembly != null)
            {
                _engineType = _jintAssembly.GetType("Jint.Engine");
                _setValue = _engineType.GetMethod("SetValue", [typeof(string), typeof(object)]);
                _evaluate = _engineType.GetMethod("Evaluate", [typeof(string), typeof(string)]);
                _toObject = _jintAssembly.GetType("Jint.Native.JsValue").GetMethod("ToObject");
            }
        }

        public Javascript(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        protected override void ScriptInvoke<T>(T variables, out object result)
        {
            Info("Attempting to invoke Javascript script {0}", ID);
            if (_engineType == null)
                throw new JintAssemblyMissingException();
            Debug("Creating new Javascript Engine for script element {0}", ID);
            object engine = Activator.CreateInstance(_engineType);
            object[] pars = ["variables", variables];
            Debug("Invoking Javascript Engine for script element {0}", ID);
            _setValue.Invoke(engine, pars);
            if (Code.Contains("return "))
                result = _evaluate.Invoke(engine, [string.Format(_codeExecReturnFormat, Code), null]);
            else
                result = _evaluate.Invoke(engine, [Code, null]);
            if (IsCondition)
                result = bool.Parse(_toObject.Invoke(result, []).ToString());
            else if (IsTimerEvent)
                result = ConvertJsDateToDateTime(_toObject.Invoke(result, []));
        }

        static DateTime ConvertJsDateToDateTime(object jsDate)
            => (jsDate) switch
            {
                (double timestamp) => DateTime.UnixEpoch.AddMilliseconds(timestamp).ToLocalTime(),
                (DateTime time) => TimeZoneInfo.ConvertTime(time, TimeZoneInfo.Local),
                (string str) => DateTime.Parse(str, CultureInfo.InvariantCulture),
                _ => throw new ArgumentException("Invalid JavaScript Date format.")
            };

        protected override bool ScriptIsValid(out IEnumerable<string> err)
        {
            try
            {
                if (_engineType == null)
                    throw new JintAssemblyMissingException();
                object engine = _engineType.GetConstructor(Type.EmptyTypes).Invoke([]);
                object[] pars = ["variables", new ProcessVariablesContainer(null, OwningDefinition.OwningProcess)];
                _setValue.Invoke(engine, pars);
                try
                {
                    _evaluate.Invoke(engine, [Code, null]);
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        if (ex.InnerException.Message.Contains("Illegal return statement"))
                            _evaluate.Invoke(engine, [string.Format(_codeExecReturnFormat, Code), null]);
                        else
                            throw;
                    }
                    else
                        throw;
                }
            }
            catch (Exception e)
            {
                err = [e.Message];
                return false;
            }
            err = null;
            return true;
        }
    }
}
