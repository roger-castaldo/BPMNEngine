using Org.Reddragonit.BpmEngine.Drawing.Gif;
using Org.Reddragonit.BpmEngine.Drawing.Wrappers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing
{
    /// <summary>
    /// Cloned from https://raw.githubusercontent.com/DataDink/Bumpkit/master/BumpKit/BumpKit/GifEncoder.cs
    /// Encodes multiple images as an animated gif to a stream. <br />
    /// ALWAYS ALWAYS ALWAYS wire this up   in a using block <br />
    /// Disposing the encoder will complete the file. <br />
    /// Uses default .net GIF encoding and adds animation headers.
    /// </summary>
    internal class GifEncoder : IDisposable
    {
        private static readonly Color _TransparentColor = Color.White;

        private struct sGif
        {
            private short _canvasWidth;
            public short CanvasWidth { get { return _canvasWidth; } }
            private short _canvasHeight;
            public short CanvasHeight { get { return _canvasHeight; } }
            private byte _packedField;
            public byte PackedField { get { return _packedField; } }
            private byte[] _colorTable;
            public byte[] ColorTable { get { return _colorTable; } }
            private byte[] _data;
            public byte[] Data { get { return _data; } }

            private int _transparentColorIndex;
            public int TransparentColorIndex { get { return _transparentColorIndex; } }

            public sGif(Image img,bool isFirstFrame)
            {
                _packedField=0xF7;
                _canvasWidth = (short)img.Size.Width;
                _canvasHeight = (short)img.Size.Height;
                _transparentColorIndex = 0;
                List<Color> colors = new List<Color>();
                _data = new byte[img.Size.Width*img.Size.Height];
                if (!isFirstFrame)
                {
                    Image g = new Image(img.Size);
                    g.FillRectangle(new SolidBrush(_TransparentColor), new Rectangle(0f, 0f, (float)img.Size.Width, (float)img.Size.Height));
                    g.DrawImage(img, new Point(0, 0));
                    g.Flush();
                    img=g;
                    colors.Add(_TransparentColor);
                }

                for(int y = 0; y<img.Size.Height; y++)
                {
                    for(int x = 0; x<img.Size.Width; x++)
                    {
                        Color c = img.GetPixel(x, y);
                        if (!colors.Contains(c))
                        {
                            if (colors.Count==256)
                                c = _FindClosest(colors, c);
                            else
                                colors.Add(c);
                        }
                        _data[(y*img.Size.Width)+x]=(byte)colors.IndexOf(c);
                    }
                }

                _colorTable = new byte[3*256];

                for (int x = 0; x<_colorTable.Length; x++)
                    _colorTable[x]=0x00;

                int idx = 0;
                foreach (Color c in colors)
                {
                    _colorTable[idx]=(byte)c.R;
                    _colorTable[idx+1]=(byte)c.G;
                    _colorTable[idx+2]=(byte)c.B;
                    idx+=3;
                }

                MemoryStream ms = new MemoryStream();
                new LZWEncoder(img.Size.Width, img.Size.Height, _data, 8).Encode(ms);

                _data=ms.ToArray();
            }

            private static Color _FindClosest(List<Color> colors, Color c)
            {
                Color ret = null;
                int cc = int.MaxValue;
                foreach (Color col in colors)
                {
                    int hc = c.HowClose(col);
                    if (hc<cc)
                    {
                        ret=col;
                        cc=hc;
                    }
                }
                return ret;
            }
        }

        public struct sFramePart
        {
            private Image _image;
            public Image Image { get { return _image; } }

            private int _x;
            public int X { get { return _x; } }

            private int _y;
            public int Y { get { return _y; } }

            public sFramePart(Image image)
                : this(image, 0, 0)
            { }

            public sFramePart(Image image,int x,int y)
            {
                _image = image;
                _x = x;
                _y = y;
            }
        }

        #region Header Constants
        private const string FileType = "GIF";
        private const string FileVersion = "89a";
        private const byte FileTrailer = 0x3b;

        private const int ApplicationExtensionBlockIdentifier = 0xff21;
        private const byte ApplicationBlockSize = 0x0b;
        private const string ApplicationIdentification = "NETSCAPE2.0";

        private const int GraphicControlExtensionBlockIdentifier = 0xf921;
        private const byte GraphicControlExtensionBlockSize = 0x04;
        private const byte ImageSeperator = 0x2C;
#endregion

        private bool _isFirstImage = true;
        private readonly Stream _stream;

        // Public Accessors
        public TimeSpan FrameDelay { get; set; }

        /// <summary>
        /// Encodes multiple images as an animated gif to a stream. <br />
        /// ALWAYS ALWAYS ALWAYS wire this in a using block <br />
        /// Disposing the encoder will complete the file. <br />
        /// Uses default .net GIF encoding and adds animation headers.
        /// </summary>
        /// <param name="stream">The stream that will be written to.</param>
        public GifEncoder(Stream stream)
        {
            _stream = stream;
        }

        /// <summary>
        /// Adds a frame to this animation.
        /// </summary>
        /// <param name="parts">The image parts to generate a frame from</param>
        public void AddFrame(sFramePart[] parts)
        {
            for(int x = 0; x < parts.Length; x++)
            {
                _AddFramePart(parts[x], x == 0);
            }
        }

        private void _AddFramePart(sFramePart part, bool firstPart)
        {
            using (var gifStream = new MemoryStream())
            {
                sGif gif = new sGif(part.Image,_isFirstImage);
                if (_isFirstImage) // Steal the global color table info
                {
                    InitHeader(gif);
                }
                WriteGraphicControlBlock(gif, (firstPart ? FrameDelay : TimeSpan.FromSeconds(0)));
                WriteImageBlock(gif, !_isFirstImage, part.X,part.Y);
            }
            _isFirstImage = false;
        }

        private void InitHeader(sGif gif)
        {
            // File Header
            WriteString(FileType);
            WriteString(FileVersion);
            WriteShort(gif.CanvasWidth); // Initial Logical Width
            WriteShort(gif.CanvasHeight); // Initial Logical Height
            WriteByte(gif.PackedField); // Global Color Table Info
            WriteByte(0); // Background Color Index
            WriteByte(0); // Pixel aspect ratio
            WriteByte(gif.ColorTable);

            // App Extension Header
            WriteShort(ApplicationExtensionBlockIdentifier);
            WriteByte(ApplicationBlockSize);
            WriteString(ApplicationIdentification);
            WriteByte(3); // Application block length
            WriteByte(1);
            WriteShort(0); // Repeat count for images.
            WriteByte(0); // terminator
        }

        private void WriteGraphicControlBlock(sGif gif, TimeSpan frameDelay)
        {
            WriteShort(GraphicControlExtensionBlockIdentifier); // Identifier
            WriteByte(GraphicControlExtensionBlockSize); // Block Size
            WriteByte((_isFirstImage ? 0x00 : 0x01));
            WriteShort(Convert.ToInt32(frameDelay.TotalMilliseconds / 10)); // Setting frame delay
            WriteByte(gif.TransparentColorIndex); // Transparent color index
            WriteByte(0); // Terminator
        }

        private void WriteImageBlock(sGif gif, bool includeColorTable, int x, int y)
        {
            WriteByte(ImageSeperator); // Separator
            WriteShort(x); // Position X
            WriteShort(y); // Position Y
            WriteShort(gif.CanvasWidth); 
            WriteShort(gif.CanvasHeight);

            if (includeColorTable) // If first frame, use global color table - else use local
            {
                WriteByte((((int)gif.PackedField&0x70) >> 4) | 0x80); // Enabling local color table
                WriteByte(gif.ColorTable);
            }
            else
                WriteByte(0); // Disabling local color table
            WriteByte(gif.Data);
        }

        private void WriteByte(int value)
        {
            _stream.WriteByte(Convert.ToByte(value));
        }

        private void WriteByte(byte[] data)
        {
            _stream.Write(data, 0, data.Length);
        }

        private void WriteShort(int value)
        {
            _stream.WriteByte(Convert.ToByte(value & 0xff));
            _stream.WriteByte(Convert.ToByte((value >> 8) & 0xff));
        }

        private void WriteString(string value)
        {
            foreach (char c in value)
                _stream.WriteByte((byte)c);
        }

        public void Dispose()
        {
            // Complete File
            WriteByte(FileTrailer);

            // Pushing data
            _stream.Flush();
        }
    }
}
