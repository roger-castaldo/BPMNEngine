using Microsoft.Maui.Graphics;
using BPMNEngine.Drawing.Icons;

using BPMNEngine.Elements.Diagrams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BPMNEngine.Drawing
{
    internal static class IconGraphic
    {
        private static Dictionary<BPMIcons, AIcon> _icons;

        static IconGraphic()
        {
            _icons = new Dictionary<BPMIcons,AIcon>();
            Assembly.GetAssembly(typeof(IconGraphic)).GetTypes().Where(t => t.IsSubclassOf(typeof(AIcon))).ForEach(t =>
            {
                object obj = t.GetCustomAttribute(typeof(IconTypeAttribute));
                if (obj != null)
                    _icons.Add(((IconTypeAttribute)obj).Icon, (AIcon)Activator.CreateInstance(t));
            });
        }

        public static void AppendIcon(Rect destination,BPMIcons icon, ICanvas surface,Color color)
        {
            lock (_icons)
            {
                if (_icons.ContainsKey(icon))
                {
                    _icons[icon].Draw(destination, surface, color);
                }
            }
        }
    }
}
