namespace BPMNEngine
{
    internal class ElementTypeCache
    {
        private readonly struct SCachedType
        {
            public SCachedType(){}
            public string Tag { get; init; } = string.Empty;
            public Type Type { get; init; } = null;
        }

        private readonly List<SCachedType> cache;

        public ElementTypeCache() { cache= []; }

        public Type this[string xmlTag] => cache.Find(ct=>ct.Tag.Equals(xmlTag,StringComparison.CurrentCultureIgnoreCase)).Type;

        public bool IsCached(string xmlTag) => cache.Exists(ct=>ct.Tag.Equals(xmlTag,StringComparison.CurrentCultureIgnoreCase));

        public void MapIdeals(XmlPrefixMap map)
        {
            Dictionary<string, Dictionary<string, Type>> ideals = Utility.IdealMap;
            ideals.ForEach(prefixPair =>
            {
                IEnumerable<string> tmp = map.Translate(prefixPair.Key);
                if (tmp.Any())
                {
                    tmp.ForEach(trans =>
                    {
                        prefixPair.Value.ForEach(tagPair =>
                        {
                            var tag = $"{trans}:{tagPair.Key}";
                            if (!cache.Exists(ct => ct.Tag.Equals(tag, StringComparison.CurrentCultureIgnoreCase)))
                                cache.Add(new()
                                {
                                    Tag=tag,
                                    Type=tagPair.Value
                                });
                        });
                    });
                }
                else
                {
                    prefixPair.Value.ForEach(tagPair =>
                    {
                        cache.Add(new() { Tag=$"{prefixPair.Key}:{tagPair.Key}",Type=tagPair.Value });
                        cache.Add(new() { Tag=tagPair.Key, Type=tagPair.Value });
                    });
                }
            });
        }
    }
}
