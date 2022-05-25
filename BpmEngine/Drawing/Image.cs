using Org.Reddragonit.BpmEngine.Drawing.Wrappers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing
{
    internal class Image : IDrawingSurface
    {
        public static bool CanUse
        {
            get
            {
                return DrawingImage.CAN_USE||SkiaImage.CAN_USE;
            }
        }

        IDrawingSurface _surface;

        public Size Size { get { return _surface.Size; } }
        public object DrawingObject
        {
            get { return _surface.DrawingObject; }
        }

        public object SkiaObject
        {
            get { return _surface.SkiaObject; }
        }

        public Image(Rectangle rect)
            : this((int)rect.Width, (int)rect.Height) { }

        public Image(int width,int height)
            : this(new Size(width, height)) { }

        public Image(Size size)
        {
            _surface = (SkiaImage.CAN_USE ? new SkiaImage(size) : (IDrawingSurface)(DrawingImage.CAN_USE ? new DrawingImage(size) : null));
        }

        public static int VerticalTextShift
        {
            get { return (SkiaImage.CAN_USE ? 5 : -7); }
        }

        public static float EdgeLabelVerticalShift
        {
            get { return (SkiaImage.CAN_USE ? 6f : 0); }
        }

        public static float EdgeLabelHorizontalShift
        {
            get { return (SkiaImage.CAN_USE ? -1.5f : 0); }
        }

        private Image(Stream str)
        {
            _surface = (SkiaImage.CAN_USE ? new SkiaImage(str) : (IDrawingSurface)(DrawingImage.CAN_USE ? new DrawingImage(str) : null));
        }

        public void Clear(Color color)
        {
            _surface.Clear(color);
        }

        public void TranslateTransform(float x, float y)
        {
            _surface.TranslateTransform(x, y);
        }

        public void RotateTransform(float angle)
        {
            _surface.RotateTransform(angle);
        }

        public void DrawImage(IDrawingSurface image, Point point)
        {
            DrawImage(image, new Rectangle(point.X, point.Y, image.Size.Width, image.Size.Height));
        }

        public void DrawImage(IDrawingSurface image, Rectangle rect)
        {
            _surface.DrawImage(image, rect);
        }

        public void DrawLines(Pen pen, Point[] points)
        {
            _surface.DrawLines(pen, points);
        }

        public void DrawLine(Pen pen, Point start, Point end)
        {
            _surface.DrawLine(pen, start, end);
        }

        public void DrawEllipse(Pen pen, Rectangle rect)
        {
            _surface.DrawEllipse(pen, rect);
        }

        public void FillEllipse(SolidBrush brush,Rectangle rect)
        {
            _surface.FillEllipse(brush, rect);
        }

        public void FillPolygon(SolidBrush brush, Point[] points)
        {
            _surface.FillPolygon(brush, points);
        }

        public void DrawRectangle(Pen pen, Rectangle rect)
        {
            this.DrawLines(pen, new Point[]
            {
                new Point(rect.X,rect.Y),
                new Point(rect.X+rect.Width,rect.Y),
                new Point(rect.X+rect.Width,rect.Y+rect.Height),
                new Point(rect.X,rect.Y+rect.Height),
                new Point(rect.X,rect.Y)
            });
        }
        public void FillRectangle(SolidBrush brush, Rectangle rect)
        {
            this.FillPolygon(brush, new Point[]
            {
                new Point(rect.X,rect.Y),
                new Point(rect.X+rect.Width,rect.Y),
                new Point(rect.X+rect.Width,rect.Y+rect.Height),
                new Point(rect.X,rect.Y+rect.Height),
                new Point(rect.X,rect.Y)
            });
        }

        public Size MeasureString(string content)
        {
            return _surface.MeasureString(content, null);
        }

        public Size MeasureString(string content, Size container)
        {
            return _surface.MeasureString(content, container);
        }

        public void DrawString(string content, Color color,Point point)
        {
            Size sf = MeasureString(content);
            DrawString(content, new SolidBrush(color), new Rectangle(point.X, point.Y, sf.Width, sf.Height),false);
        }

        public void DrawString(string content, SolidBrush brush, Rectangle rect,bool center)
        {
            _surface.DrawString(content, brush, rect,center);
        }

        public void DrawRoundRectangle(Pen pen, RoundRectangle rect)
        {
            _surface.DrawRoundRectangle(pen, rect);
        }

        public void Flush()
        {
            _surface.Flush();
        }

        public static Image FromStream(Stream str)
        {
            return new Image(str);
        }

        public Color GetPixel(int x, int y)
        {
            return _surface.GetPixel(x, y);
        }

        public void SetPixel(int x, int y, Color color)
        {
            _surface.SetPixel(x, y, color);
        }

        public byte[] ToFile(ImageOuputTypes type)
        {
            return _surface.ToFile(type);
        }

        public void Dispose()
        {
            if (_surface!=null)
                _surface.Dispose();
        }
    }
}
