using Microsoft.Maui.Graphics;
using BPMNEngine.Drawing.Icons;

using BPMNEngine.Elements.Diagrams;
using System.Reflection;

namespace BPMNEngine.Drawing
{
    internal static class IconGraphic
    {
        private static readonly Dictionary<BPMIcons, AIcon> icons
            = new(
                Assembly.GetAssembly(typeof(IconGraphic)).GetTypes()
                .Where(t => t.IsSubclassOf(typeof(AIcon)))
                .Select(t => new {Type=t,IconType=t.GetCustomAttribute<IconTypeAttribute>()})
                .Where(ai=>ai.IconType!=null)
                .Select(ai=>new KeyValuePair<BPMIcons, AIcon>(ai.IconType.Icon,(AIcon)Activator.CreateInstance(ai.Type)))
        );

        public static void AppendIcon(Rect destination,BPMIcons icon, ICanvas surface,Color color)
        {
            if (icons.TryGetValue(icon,out AIcon aIcon))
                aIcon.Draw(destination, surface, color);
        }
    }
}
