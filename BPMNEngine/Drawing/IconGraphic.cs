using Microsoft.Maui.Graphics;
using BPMNEngine.Drawing.Icons;

using BPMNEngine.Elements.Diagrams;
using System.Reflection;

namespace BPMNEngine.Drawing
{
    internal static class IconGraphic
    {
        private static readonly Dictionary<BPMIcons, AIcon> icons;

        static IconGraphic()
        {
            icons = new Dictionary<BPMIcons,AIcon>();
            Assembly.GetAssembly(typeof(IconGraphic)).GetTypes().Where(t => t.IsSubclassOf(typeof(AIcon))).ForEach(t =>
            {
                object obj = t.GetCustomAttribute(typeof(IconTypeAttribute));
                if (obj != null)
                    icons.Add(((IconTypeAttribute)obj).Icon, (AIcon)Activator.CreateInstance(t));
            });
        }

        public static void AppendIcon(Rect destination,BPMIcons icon, ICanvas surface,Color color)
        {
            if (icons.TryGetValue(icon,out AIcon aIcon))
                aIcon.Draw(destination, surface, color);
        }
    }
}
