using Microsoft.Maui.Graphics;
using Org.Reddragonit.BpmEngine.Drawing.Icons;

using Org.Reddragonit.BpmEngine.Elements.Diagrams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing
{
    internal static class IconGraphic
    {
        private static Dictionary<BPMIcons, AIcon> _icons;

        static IconGraphic()
        {
            _icons = new Dictionary<BPMIcons,AIcon>();
            foreach (Type t in Assembly.GetAssembly(typeof(IconGraphic)).GetTypes().Where(t => t.IsSubclassOf(typeof(AIcon))))
            {
                object obj = t.GetCustomAttribute(typeof(IconTypeAttribute));
                if (obj != null)
                    _icons.Add(((IconTypeAttribute)obj).Icon, (AIcon)Activator.CreateInstance(t));
            }
        }

        public static void AppendIcon(Rect destination,BPMIcons icon,Image gp,Color color)
        {
            lock (_icons)
            {
                if (_icons.ContainsKey(icon))
                {
                    _icons[icon].Draw(destination, gp, color);
                }
            }
        }
    }
}
