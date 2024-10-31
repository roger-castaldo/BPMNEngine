using BPMNEngine.Attributes;
using BPMNEngine.Interfaces;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Reflection;

namespace BPMNEngine
{
    public class ElementFactory : IElementFactory
    {
        private static readonly KeyValuePair<string,Type>[] baseElementMap = typeof(ElementFactory)
                .Assembly
                .GetTypes()
                .Select(t => new { Type = t, Tags = t.GetCustomAttributes<XMLTagAttribute>(false) })
                .Where(tt => tt.Tags.Any())
                .SelectMany(tt =>
                    tt.Tags.SelectMany(tag =>
                    {
                        IEnumerable<KeyValuePair<string,Type>> result = [new(tag.Name.ToLower(), tt.Type)];
                        if (!string.IsNullOrWhiteSpace(tag.Prefix))
                            result = result.Append(new($"{tag.Prefix}:{tag.Name}".ToLower(), tt.Type));
                        return result;
                    })
                ).ToArray();

        public static ElementFactory Instance(IServiceProvider serviceProvider, ILogger? logger = null)
            => new ElementFactory(serviceProvider,logger);

        private readonly IServiceProvider serviceProvider;
        private readonly XmlPrefixMap prefixMap;
        private readonly Dictionary<string, Type> elementMap = new(baseElementMap);
        private readonly ConcurrentDictionary<Type, ConstructorInfo?> constructorMap = [];

        private ElementFactory(IServiceProvider serviceProvider,ILogger? logger=null)
        {
            this.serviceProvider=serviceProvider;
            this.prefixMap = new(logger);
        }

        private Type? MapType<T>(XmlElement element)
        {
            prefixMap.Load(element);
            if (!elementMap.TryGetValue(element.Name.ToLower(),out var result))
            {
                result = elementMap.FirstOrDefault(pair => prefixMap.IsMatch(
                    pair.Key.Contains(':') ? pair.Key.Split(':')[0] : "",
                    pair.Key.Contains(':') ? pair.Key.Split(':')[1] : pair.Key,
                    element.Name)).Value;
            }
            return (result?.GetInterfaces().Contains(typeof(T))??false)||Equals(result,typeof(T)) ? result : null;
        }

        private T? ConstructInstance<T>(XmlElement element,IBaseElement? parent)
        {
            var type = MapType<T>(element);
            if (type==null)
                return default;
            if (!constructorMap.TryGetValue(type,out var constructor))
            {
                constructor = Array.Find(type.GetConstructors()
                    ,(c=>
                        Array.Exists(c.GetParameters(),(p=>Equals(typeof(XmlElement),p.ParameterType))) &&
                        Array.Exists(c.GetParameters(), (p => Equals(typeof(IBaseElement), p.ParameterType))) &&
                        Array.Exists(c.GetParameters(), (p => Equals(typeof(IElementFactory), p.ParameterType) || Equals(typeof(IExtensionElementFactory), p.ParameterType)))
                    )) ??
                    Array.Find(type.GetConstructors()
                    ,(c =>
                        Array.Exists(c.GetParameters(), (p => Equals(typeof(XmlElement), p.ParameterType))) &&
                        Array.Exists(c.GetParameters(), (p => Equals(typeof(IElementFactory), p.ParameterType) || Equals(typeof(IExtensionElementFactory), p.ParameterType)))
                    )) ??
                    Array.Find(type.GetConstructors()
                    , (c =>
                        Array.Exists(c.GetParameters(), (p => Equals(typeof(XmlElement), p.ParameterType))) &&
                        Array.Exists(c.GetParameters(), (p => Equals(typeof(IBaseElement), p.ParameterType)))
                    )) ??
                    Array.Find(type.GetConstructors()
                    , (c =>
                        Array.Exists(c.GetParameters(), (p => Equals(typeof(IBaseElement), p.ParameterType))) &&
                        Array.Exists(c.GetParameters(), (p => Equals(typeof(IElementFactory), p.ParameterType) || Equals(typeof(IExtensionElementFactory), p.ParameterType)))
                    )) ??
                    Array.Find(type.GetConstructors()
                    , (c =>
                        Array.Exists(c.GetParameters(), (p => Equals(typeof(XmlElement), p.ParameterType))) ||
                        Array.Exists(c.GetParameters(), (p => Equals(typeof(IBaseElement), p.ParameterType))) ||
                        Array.Exists(c.GetParameters(), (p => Equals(typeof(IElementFactory), p.ParameterType) || Equals(typeof(IExtensionElementFactory), p.ParameterType)))
                    ));
                constructorMap.TryAdd(type, constructor);
            }
            IEnumerable<object> pars = [];
            if (Array.Exists(constructor?.GetParameters()??[],(p => Equals(typeof(XmlElement), p.ParameterType))))
                pars=pars.Append(element);
            if (Array.Exists(constructor?.GetParameters()?? [], (p => Equals(typeof(IBaseElement), p.ParameterType))))
                pars=pars.Append(parent);
            if (Array.Exists(constructor?.GetParameters()?? [], (p => Equals(typeof(IElementFactory), p.ParameterType))))
                pars=pars.Append((IElementFactory)this);
            if (Array.Exists(constructor?.GetParameters()?? [], (p=>Equals(typeof(IExtensionElementFactory), p.ParameterType))))
                pars=pars.Append((IExtensionElementFactory)this);
            try
            {
                return (T)ActivatorUtilities.CreateInstance(serviceProvider, type!, pars.ToArray());
            }catch(System.InvalidOperationException ioe)
            {
                if (constructor!=null)
                {
                    var parameters = constructor.GetParameters()
                        .Select((p, idx) =>
                        {
                            if (Equals(typeof(XmlElement), p.ParameterType))
                                return element;
                            else if (Equals(typeof(IBaseElement), p.ParameterType))
                                return parent;
                            else if (Equals(typeof(IElementFactory), p.ParameterType))
                                return (IElementFactory)this;
                            else if (Equals(typeof(IExtensionElementFactory), p.ParameterType))
                                return (IExtensionElement)this;
                            return serviceProvider.GetService(p.ParameterType);
                        }).ToArray();
                    return (T)constructor.Invoke(parameters);
                }
            }
            return default;
        }

        IExtensionElement? IExtensionElementFactory.ProduceExtensionElement(XmlElement element, IBaseElement parent)
            => ConstructInstance<IExtensionElement>(element,parent);
        IElement? IElementFactory.ProduceInstance(XmlElement element, IElement? parent) 
            => ConstructInstance<IElement>(element, parent);
        IExtensionElementFactory IExtensionElementFactory.RegisterExtension<T>(string elementName, string? elementPrefix)
        {
            elementMap.Add(elementName.ToLower(), typeof(T));
            if (!string.IsNullOrWhiteSpace(elementPrefix))
                elementMap.Add($"{elementPrefix}:{elementName}".ToLower(), typeof(T));
            return this;
        }

        bool IElementFactory.IsOfType<T>(XmlElement element)
        {
            var type = MapType<T>(element);
            return Equals(type,typeof(T)) || 
                (type?.GetInterfaces().Contains(typeof(T))??false) || 
                (type?.IsSubclassOf(typeof(T))??false);
        }
    }
}
