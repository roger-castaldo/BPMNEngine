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
    public class GifEncoder : IDisposable
    {
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

            public sGif(Image img)
            {
                int len;
                _canvasWidth = (short)img.Width;
                _canvasHeight = (short)img.Height;
                MemoryStream ms = new MemoryStream();
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
                _colorTable = br.ReadBytes(colorTableLength * 3);
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
        /// <param name="width">Sets the width for this gif or null to use the first frame's width.</param>
        /// <param name="height">Sets the height for this gif or null to use the first frame's height.</param>
        public GifEncoder(Stream stream)
        {
            _stream = stream;
        }

        /// <summary>
        /// Adds a frame to this animation.
        /// </summary>
        /// <param name="img">The image to add</param>
        /// <param name="x">The positioning x offset this image should be displayed at.</param>
        /// <param name="y">The positioning y offset this image should be displayed at.</param>
        public void AddFrame(Image img)
        {
            using (var gifStream = new MemoryStream())
            {
                sGif gif = new sGif(img);
                if (_isFirstImage) // Steal the global color table info
                {
                    InitHeader(gif);
                }
                WriteGraphicControlBlock(gif, FrameDelay);
                WriteImageBlock(gif, !_isFirstImage, 0, 0);
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
            WriteByte(0x00);
            WriteShort(Convert.ToInt32(frameDelay.TotalMilliseconds / 10)); // Setting frame delay
            WriteByte(0); // Transparent color index
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
