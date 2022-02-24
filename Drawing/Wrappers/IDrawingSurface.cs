using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Wrappers
{
    internal interface IDrawingSurface:IDrawingObject
    {
        Size Size { get; }
        void TranslateTransform(float x, float y);
        void RotateTransform(float angle);
        void DrawImage(IDrawingSurface image, Rectangle rect);
        void DrawLines(Pen pen, Point[] points);
        void DrawLine(Pen pen, Point start,Point end);
        void DrawEllipse(Pen pen, Rectangle rect);
        void FillEllipse(SolidBrush brush, Rectangle rect);
        void FillPolygon(SolidBrush brush, Point[] points);
        Size MeasureString(string content, Size container);
        void DrawString(string content, SolidBrush brush, Rectangle rect);
        void DrawPath(Pen pen, GraphicsPath path);
        void Flush();
        Color GetPixel(int x, int y);
        void SetPixel(int x,int y,Color color);
        byte[] ToFile(ImageOuputTypes type);
        byte[] ToGif();
    }
}
