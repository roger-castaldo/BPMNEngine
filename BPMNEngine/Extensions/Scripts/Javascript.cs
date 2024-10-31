using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Elements;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

namespace BPMNEngine.Extensions.Scripts
{
    [XMLTagAttribute("exts", "Javascript")]
    internal record Javascript : AScript
    {
        private const string codeExecReturnFormat = "(function(){{ {0} }})();";

        private static readonly Assembly? jintAssembly;
        private static readonly Type? engineType;
        private static readonly MethodInfo? setValue;
        private static readonly MethodInfo? evaluate;
        private static readonly MethodInfo? toObject;

        public BusinessProcess? OwningProcess {
            get {
                var parent = Parent;
                while (parent!=null)
                {
                    if (parent is BPMNEngine.Elements.Definition def)
                        return def.OwningProcess;
                    parent = parent.Parent;
                }
                return null;
            }
        }

        [ExcludeFromCodeCoverage(Justification = "This portion of the code is used to dynamically load the Jint.dll during runtime and cannot be properly tested")]
        static Javascript()
        {
            try
            {
                jintAssembly = Assembly.Load("Jint");
            }
            catch (Exception)
            {
                jintAssembly = null;
            }
            if (jintAssembly == null)
            {
                try
                {
#pragma warning disable S3885 // "Assembly.Load" should be used
                    jintAssembly = Assembly.LoadFrom(typeof(Javascript).Assembly.Location.Replace("BPMNEngine.dll", "Jint.dll"));
#pragma warning restore S3885 // "Assembly.Load" should be used
                }
                catch (Exception)
                {
                    jintAssembly = null;
                }
            }
            if (jintAssembly != null)
            {
                engineType = jintAssembly.GetType("Jint.Engine");
                setValue = engineType?.GetMethod("SetValue", [typeof(string), typeof(object)]);
                evaluate = engineType?.GetMethod("Evaluate", [typeof(string), typeof(string)]);
                toObject = jintAssembly?.GetType("Jint.Native.JsValue")?.GetMethod("ToObject");
            }
        }

        public Javascript(XmlElement xmlElement, IBaseElement parent) 
            : base(xmlElement, parent) {}

        protected override void ScriptInvoke<T>(T variables, ILogger? logger, out object? result)
        {
            logger?.LogInformation("Attempting to invoke Javascript script");
            if (engineType == null)
                throw new JintAssemblyMissingException();
            logger?.LogDebug("Creating new Javascript Engine for script element");
            var engine = Activator.CreateInstance(engineType);
            object[] pars = ["variables", variables];
            logger?.LogDebug("Invoking Javascript Engine for script element");
            setValue?.Invoke(engine, pars);
            if (Code.Contains("return "))
                result = evaluate?.Invoke(engine, [string.Format(codeExecReturnFormat, Code), null]);
            else
                result = evaluate?.Invoke(engine, [Code, null]);
            if (IsCondition)
                result = bool.Parse(toObject?.Invoke(result, [])?.ToString()??"");
            else if (IsTimerEvent)
                result = ConvertJsDateToDateTime(toObject?.Invoke(result, []));
        }

        protected override bool ScriptIsValid(ILogger? logger, out IEnumerable<string>? err)
        {
            try
            {
                if (engineType == null)
                    throw new JintAssemblyMissingException();
                var engine = Activator.CreateInstance(engineType);
                object[] pars = ["variables", new ProcessVariablesContainer(null, OwningProcess)];
                setValue?.Invoke(engine, pars);
                try
                {
                    evaluate?.Invoke(engine, [Code, null]);
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        if (ex.InnerException.Message.Contains("Illegal return statement"))
                            evaluate.Invoke(engine, [string.Format(codeExecReturnFormat, Code), null]);
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

        static DateTime ConvertJsDateToDateTime(object jsDate)
            => (jsDate) switch
            {
                (double timestamp) => DateTime.UnixEpoch.AddMilliseconds(timestamp).ToLocalTime(),
                (DateTime time) => TimeZoneInfo.ConvertTime(time, TimeZoneInfo.Local),
                (string str) => DateTime.Parse(str, CultureInfo.InvariantCulture),
                _ => throw new ArgumentException("Invalid JavaScript Date format.")
            };
    }
}
