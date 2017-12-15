using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.BpmEngine
{
    internal class ElementTypeCache
    {
        private Dictionary<string, Type> _cachedMaps;

        public ElementTypeCache() { _cachedMaps = new Dictionary<string, Type>(); }

        public Type this[string xmlTag]
        {
            get
            {
                if (_cachedMaps.ContainsKey(xmlTag.ToLower()))
                    return _cachedMaps[xmlTag.ToLower()];
                return null;
            }
            set
            {
                _cachedMaps.Add(xmlTag.ToLower(), value);
            }
        }

        public bool IsCached(string xmlTag)
        {
            return _cachedMaps.ContainsKey(xmlTag.ToLower());
        }
    }
}
