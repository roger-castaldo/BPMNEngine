using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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

            private int _colorTableByteLength;
            public int ColorTableByteLength { get { return _colorTableByteLength; } }

            private int _transparentColorIndex;
            public int TransparentColorIndex { get { return _transparentColorIndex; } }

            public sGif(Image img,bool isFirstFrame)
            {
                int len;
                _canvasWidth = (short)img.Width;
                _canvasHeight = (short)img.Height;
                MemoryStream ms = new MemoryStream();
                if (!isFirstFrame)
                {
                    Bitmap bmp = new Bitmap(img.Width, img.Height);
                    Graphics g = Graphics.FromImage(bmp);
                    g.FillRectangle(new SolidBrush(_TransparentColor), 0, 0, bmp.Width, bmp.Height);
                    g.DrawImage(img, 0, 0);
                    g.Flush();
                    bmp.Save(ms, ImageFormat.Gif);
                }else
                    img.Save(ms, ImageFormat.Gif);
                ms.Position = 0;
                BinaryReader br = new BinaryReader(ms);
                br.ReadBytes(4); //skip gif8
                bool gif87a = !(br.ReadByte() == (byte)0x39);
                br.ReadBytes(1); //skip remaining header
                br.ReadBytes(4); //skip dimensions
                _packedField = (byte)br.ReadByte();
                br.ReadBytes(2); //skip background color and aspect ratio
                int colorTableLength = 0;
                if (!gif87a)
                {
                    _colorTableByteLength = (int)((int)_packedField & 0x07);
                    colorTableLength = (int)Math.Pow(2, _colorTableByteLength + 1);
                }
                else
                {
                    _colorTableByteLength = (int)((int)_packedField & 0x70)>>4;
                    colorTableLength = (int)Math.Pow(2, _colorTableByteLength + 1);
                    _packedField = (byte)(0x80 | (_colorTableByteLength<<4) | (((int)_packedField & 0x10) >> 1) | _colorTableByteLength);
                }
                _transparentColorIndex = 0;
                _colorTable = br.ReadBytes(colorTableLength * 3);
                for(int x = 0; x < _colorTable.Length; x+=3)
                {
                    if (_colorTable[x]==_TransparentColor.R
                        && _colorTable[x+1]==_TransparentColor.G
                        && _colorTable[x+2] == _TransparentColor.B)
                    {
                        _transparentColorIndex = x / 3;
                        break;
                    }
                }
                if (!gif87a)
                {
                    while (br.ReadByte() == 0x21)
                    {
                        switch (br.ReadByte())
                        {
                            case 0xF9:
                                len = br.ReadByte();
                                br.ReadBytes(len); //skip gce content
                                br.ReadByte(); //skip extension end
                                break;
                            case 0xFF:
                            case 0x01:
                                len = br.ReadByte();
                                br.ReadBytes(len); //skip descriptor
                                len = br.ReadByte(); //read len of next block
                                while (len != 0)
                                {
                                    br.ReadBytes(len); //skip block content
                                    len = br.ReadByte(); //read len of next block
                                }
                                break;
                        }
                    }

                }
                else
                    br.ReadByte(); //skip image sperator char
                br.ReadBytes(8); //skip dimensions/image seperator/positions
                byte tb = br.ReadByte();
                if (((int)tb & 0x80) == (int)0x80)
                {
                    len = (int)Math.Pow(2, ((int)tb & 0x07) + 1);
                    if (len !=2 )
                    {
                        _colorTableByteLength = tb & 0x07;
                        _colorTable = br.ReadBytes(len * 3);
                    }
                }
                MemoryStream msData = new MemoryStream();
                BinaryWriter bw = new BinaryWriter(msData);
                bw.Write(br.ReadByte());
                len = br.ReadByte();
                while (len != 0)
                {
                    bw.Write((byte)len);
                    bw.Write(br.ReadBytes(len));
                    len = br.ReadByte();
                }
                bw.Write((byte)0x00);
                if (br.ReadByte()==0x00)
                    bw.Write((byte)0x00);
                bw.Flush();
                _data = msData.ToArray();
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
        /// <param name="img">The image to add</param>
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
