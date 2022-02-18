using Org.Reddragonit.BpmEngine.Drawing.Icons.IconParts;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Icons.Gateways
{
    internal abstract class AGateway : AIcon
    {
        public new const int IMAGE_SIZE = 63;

        private static readonly PointF[] _POINTS = new PointF[] {
            new PointF((float)AGateway.IMAGE_SIZE/2f,0),
            new PointF((float)AGateway.IMAGE_SIZE-2,(float)AGateway.IMAGE_SIZE/2f),
            new PointF((float)AGateway.IMAGE_SIZE/2f,(float)AGateway.IMAGE_SIZE-2),
            new PointF(0,(float)AGateway.IMAGE_SIZE/2f),
            new PointF((float)AGateway.IMAGE_SIZE/2f,0)
        };

        protected override sealed int _ImageSize
        {
            get { return IMAGE_SIZE; }
        }

        protected override void _Draw(Graphics gp, Color color)
        {
            base._Draw(gp, color);
            gp.DrawLines(new Pen(color, 2f), _POINTS);
        }
    }
}
