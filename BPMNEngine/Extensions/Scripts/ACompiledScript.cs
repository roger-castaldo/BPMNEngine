using BPMNEngine.Interfaces.Elements;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace BPMNEngine.Extensions.Scripts
{
    internal abstract record ACompiledScript : AScript
    {
        private const string _NAME_CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvzwxyz";
        private const int _NAME_LENGTH = 32;

        private static readonly string[] IMPORTS = ["System", "BPMNEngine", "BPMNEngine.Interfaces", "BPMNEngine.Interfaces.Variables", "System.Linq"];

        protected ACompiledScript(XmlElement xmlElement, IBaseElement parent) 
            : base(xmlElement, parent) {}

        protected string ClassName { get; private init; } = NextName();
        protected string FunctionName { get; private init; } = NextName();

        private readonly object lockable = new();

        private IEnumerable<string> Imports
            => IMPORTS
            .Concat(SubNodes
                .Where(n => n.NodeType==XmlNodeType.Element && n.Name.Equals("using", StringComparison.InvariantCultureIgnoreCase))
                .Select(n => n.InnerText)
            )
            .Distinct();

        private IEnumerable<string> Dlls
            => new string[] { Assembly.GetAssembly(this.GetType()).Location }
            .Concat(SubNodes
                .Where(n => n.NodeType==XmlNodeType.Element && n.Name.Equals("dll", StringComparison.InvariantCultureIgnoreCase))
                .Select(n => n.InnerText)
            );

        private Assembly? assembly;

        protected abstract EmitResult Compile(string name, IEnumerable<MetadataReference> references, IEnumerable<string> imports, string code, out byte[] compiled, ILogger? logger);

        private bool CompileAssembly(ILogger? logger, out string? errors)
        {
            errors = null;
            lock (lockable)
            {
                if (assembly == null)
                {
                    var references = AppDomain.CurrentDomain.GetAssemblies()
                        .Where(ass => GetAssemblyLocation(ass) != null)
                        .Select(ass => MetadataReference.CreateFromFile(GetAssemblyLocation(ass)))
                        .Concat(
                            Dlls
                            .Select(d => MetadataReference.CreateFromFile(d))
                        );
                    EmitResult res = Compile(NextName(), references, Imports, Code, out byte[] compiled, logger);
                    if (!res.Success)
                    {
                        var error = new StringBuilder();
                        res.Diagnostics.ForEach(diag => error.AppendLine(diag.ToString()));
                        errors = $"Unable to compile script Code.  Errors:{error}";
                        assembly = null;
                    }
                    else
                        assembly = Assembly.Load(compiled);
                }
            }
            return errors == null;
        }

        protected static string? GetAssemblyLocation(Assembly ass)
        {
            try
            {
                return string.IsNullOrEmpty(ass.Location) ? null : ass.Location;
            }
            catch (Exception)
            {
                return null;
            }
        }

        protected override void ScriptInvoke<T>(T variables, ILogger? logger, out object? result)
        {
            if (!CompileAssembly(logger, out _))
                throw new Exception("Failed to compile script");
            logger?.LogDebug("Creating new instance of compiled script class for script element");
            var o = assembly?.CreateInstance(ClassName);
            logger?.LogDebug("Accesing method from new instance of compiled script class for script element");
            var mi = o?.GetType()?.GetMethod(FunctionName);
            object[] args = [variables];
            logger?.LogDebug("Executing method from new instance of compiled script class for script element");
            if (Equals(mi?.ReturnType,typeof(void)))
            {
                mi.Invoke(o, args);
                result=null;
            }
            else
                result = mi?.Invoke(o, args);
        }

        protected override bool ScriptIsValid(ILogger? logger, out IEnumerable<string>? err)
        {
            assembly = null;
            if (!CompileAssembly(logger, out var error))
            {
                err = [error!];
                return false;
            }
            else
                err = null;
            return true;
        }

        private static string NextName()
        {
            var result = new StringBuilder();
            for (var x = 0; x<_NAME_LENGTH; x++)
                result.Append(_NAME_CHARS[RandomNumberGenerator.GetInt32(_NAME_CHARS.Length - 1)]);
            return result.ToString();
        }
    }
}
