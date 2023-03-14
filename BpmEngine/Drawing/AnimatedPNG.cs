using Microsoft.Maui.Graphics;
using Org.Reddragonit.BpmEngine.Elements;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Org.Reddragonit.BpmEngine.Drawing
{
    internal class AnimatedPNG
    {
        internal class CrcCalculator
        {
            readonly long[] pTable = new long[256];

            //Für Standard CRC32:
            //(Wert kann verändert werden)
            readonly long Poly = 0xEDB88320;

            public CrcCalculator()
            {
                long CRC;
                int i, j;
                for (i = 0; i < 256; i++)
                {
                    CRC = i;
                    for (j = 0; j < 8; j++)
                        if ((CRC & 0x1) == 1)
                            CRC = (CRC >> 1) ^ Poly;
                        else
                            CRC = (CRC >> 1);
                    pTable[i] = CRC;
                }
            }

            public uint GetCRC32(byte[] input)
            {
                long CRC;
                CRC = 0xFFFFFFFF;
                for (int i = 0; i < input.Length; i++)
                    CRC = ((CRC & 0xFFFFFF00) / 0x100) & 0xFFFFFF ^ pTable[input[i] ^ CRC & 0xFF];
                CRC = (-(CRC)) - 1; // !(CRC)
                return (uint)CRC;
            }
        }

        private readonly List<AnimatedFrame> _parts;
        public TimeSpan DefaultFrameDelay { get; set; } = new TimeSpan(0, 0, 1);

        public AnimatedPNG(IImage initialImage)
        {
            var g = Diagram.ProduceImage((int)Math.Ceiling(initialImage.Width), (int)Math.Ceiling(initialImage.Height));
            var surface = g.Canvas;
            surface.FillColor = Colors.White;
            surface.FillRectangle(0, 0, initialImage.Width, initialImage.Height);
            surface.DrawImage(initialImage, 0, 0, g.Width, g.Height);
            _parts = new List<AnimatedFrame>()
            {
                new AnimatedFrame(g.Image)
            };
        }

        public void AddFrame(IImage image)
        {
            AddFrame(image, 0, 0);
        }

        public void AddFrame(IImage image,int x,int y,TimeSpan? delay=null)
        {
            _parts.Add(new AnimatedFrame(image,x,y,delay:delay));
        }

        private static readonly byte[] SIGNATURE = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
        private static readonly Byte[] IEND = { 0x00, 0x00, 0x00, 0x00, 0x49, 0x45, 0x4E, 0x44, 0xAE, 0x42, 0x60, 0x82 };

        public byte[] ToBinary()
        {
            byte[] result = null;
            using (var memoryStream = new MemoryStream())
            {
                try
                {
                    WriteHeader(memoryStream);

                    var idx = 0;
                    int chunkSequenceNumber = 0;
                    _parts.ForEach(part =>
                    {
                        WriteFrameHeader(memoryStream, part, ref chunkSequenceNumber);
                        WriteFrame(memoryStream, idx, part, ref chunkSequenceNumber);
                        idx++;
                    });
                    Write(memoryStream, IEND);

                    result = memoryStream.ToArray();
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return result;
        }

        private void WriteFrame(Stream output,int idx,AnimatedFrame part,ref int chunkSequenceNumber)
        {
            if (idx==0)
                Write(output, part.IDAT.SelectMany(c=>c));
            else
            {
                var sequenceNumber = chunkSequenceNumber;
                part.IDAT.ForEach(idat =>
                {
                    if (idat != null)
                    {
                        using (var memoryStream = new MemoryStream())
                        {

                            var length = idat.Count() - 8;
                            Write(memoryStream, length);
                            Write(memoryStream, "fdAT");
                            Write(memoryStream, sequenceNumber);

                            Write(memoryStream, idat.Take(idat.Count()-4).Skip(8));

                            var data = memoryStream.ToArray();

                            Write(output, data);
                            WriteCRC(output, data);
                        }

                        sequenceNumber++;
                    }
                });
                chunkSequenceNumber=sequenceNumber;
            }
        }

        private void WriteFrameHeader(Stream output, AnimatedFrame part,ref int chunkSequenceNumber)
        {
            Write(output, new byte[] { 0, 0, 0, 26 });
            using (var memoryStream = new MemoryStream())
            {
                Write(memoryStream, "fcTL");
                Write(memoryStream, chunkSequenceNumber);
                Write(memoryStream, (int)part.Size.Width);
                Write(memoryStream, (int)part.Size.Height);
                Write(memoryStream, part.X);
                Write(memoryStream, part.Y);
                Write(memoryStream, part.Delay ?? DefaultFrameDelay);
                Write(memoryStream, new byte[] { 0,1 });
                chunkSequenceNumber++;
                var data = memoryStream.ToArray();
                Write(output, data);
                WriteCRC(output, data);
            }
        }

        private void WriteHeader(Stream output)
        {
            Write(output, SIGNATURE);

            var frame = _parts.First();

            Write(output, frame.IHDR);

            var textSignature = "Software Reddragonit " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + "_" +
                System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

            Write(output, textSignature.Length);
            Write(output, "tEXt"+textSignature);
            WriteCRC(output, "tEXt"+textSignature);

            Write(output, 8);
            Write(output,"acTL");
            Write(output,_parts.Count);
            Write(output, 0);
            Write(output, 0);
        }

        private void Write(Stream output,IEnumerable<byte> data)
        {
            Write(output,data.ToArray());
        }

        private void Write(Stream output, byte[] data)
        {
            if (data!=null)
                output.Write(data, 0, data.Length);
        }

        private void Write(Stream output,int value)
        {
            Write(output, BitConverter.GetBytes(value).Reverse().ToArray());
        }

        private void Write(Stream output, uint value)
        {
            Write(output, BitConverter.GetBytes(value).Reverse().ToArray());
        }

        private void Write(Stream output,string value)
        {
            Write(output,value.ToCharArray().Select(c=>(byte)c).ToArray());
        }

        private void Write(Stream output,TimeSpan timespan)
        {
            Write(output,BitConverter.GetBytes((short)timespan.TotalSeconds).Reverse().ToArray());
            Write(output, BitConverter.GetBytes((short)1).Reverse().ToArray());
        }

        private void WriteCRC(Stream output, byte[] value)
        {
            Write(output, new CrcCalculator().GetCRC32(value));
        }

        private void WriteCRC(Stream output,string value)
        {
            WriteCRC(output, value.ToCharArray().Select(c => (byte)c).ToArray());
        }
    }
}
