using Org.Reddragonit.BpmEngine.Elements.Diagrams;
using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Icons
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    internal class IconTypeAttribute : Attribute
    {
        private BPMIcons _icon;
        public BPMIcons Icon
        {
            get { return _icon; }
        }

        public IconTypeAttribute(BPMIcons icon)
        {
            _icon = icon;
        }
    }
}
