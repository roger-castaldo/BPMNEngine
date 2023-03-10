using System;
using System.Collections.Generic;
using System.Linq;
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

        public void MapIdeals(XmlPrefixMap map)
        {
            Dictionary<string, Dictionary<string, Type>> ideals = Utility.IdealMap;
            foreach (string prefix in ideals.Keys)
            {
                IEnumerable<string> tmp = map.Translate(prefix);
                if (tmp!=null && tmp.Any())
                {
                    foreach (string trans in tmp)
                    {
                        foreach (string tag in ideals[prefix].Keys)
                        {
                            if (!_cachedMaps.ContainsKey(string.Format("{0}:{1}", trans, tag)))
                                _cachedMaps.Add(string.Format("{0}:{1}", trans, tag), ideals[prefix][tag]);
                        }
                    }
                }
                else
                {
                    foreach (String tag in ideals[prefix].Keys)
                    {
                        _cachedMaps.Add(string.Format("{0}:{1}", prefix, tag), ideals[prefix][tag]);
                        _cachedMaps.Add(tag, ideals[prefix][tag]);
                    }
                }
            }
        }
    }
}
