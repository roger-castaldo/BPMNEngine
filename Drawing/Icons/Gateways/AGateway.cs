using Org.Reddragonit.BpmEngine.Drawing.Icons.IconParts;
using Org.Reddragonit.BpmEngine.Drawing.Wrappers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Icons.Gateways
{
    internal abstract class AGateway : AIcon
    {
        public new const int IMAGE_SIZE = 63;

        private static readonly Point[] _POINTS = new Point[] {
            new Point((float)AGateway.IMAGE_SIZE/2f,0),
            new Point((float)AGateway.IMAGE_SIZE-2,(float)AGateway.IMAGE_SIZE/2f),
            new Point((float)AGateway.IMAGE_SIZE/2f,(float)AGateway.IMAGE_SIZE-2),
            new Point(0,(float)AGateway.IMAGE_SIZE/2f),
            new Point((float)AGateway.IMAGE_SIZE/2f,0)
        };

        protected override sealed int _ImageSize
        {
            get { return IMAGE_SIZE; }
        }

        protected override void _Draw(Image gp, Color color)
        {
            base._Draw(gp, color);
            gp.DrawLines(new Pen(color, 2f), _POINTS);
        }
    }
}
