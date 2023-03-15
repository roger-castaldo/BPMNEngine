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

        public Type this[string xmlTag] => (_cachedMaps.ContainsKey(xmlTag.ToLower()) ? _cachedMaps[xmlTag.ToLower()] : null);

        public bool IsCached(string xmlTag)
        {
            return _cachedMaps.ContainsKey(xmlTag.ToLower());
        }

        public void MapIdeals(XmlPrefixMap map)
        {
            Dictionary<string, Dictionary<string, Type>> ideals = Utility.IdealMap;
            ideals.Keys.ForEach(prefix =>
            {
                IEnumerable<string> tmp = map.Translate(prefix);
                if (tmp!=null && tmp.Any())
                {
                    tmp.ForEach(trans =>
                    {
                        ideals[prefix].Keys.ForEach(tag =>
                        {
                            if (!_cachedMaps.ContainsKey(string.Format("{0}:{1}", trans, tag)))
                                _cachedMaps.Add(string.Format("{0}:{1}", trans, tag), ideals[prefix][tag]);
                        });
                    });
                }
                else
                {
                    ideals[prefix].Keys.ForEach(tag =>
                    {
                        _cachedMaps.Add(string.Format("{0}:{1}", prefix, tag), ideals[prefix][tag]);
                        _cachedMaps.Add(tag, ideals[prefix][tag]);
                    });
                }
            });
        }
    }
}
