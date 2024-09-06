using BPMNEngine.Elements;
using Microsoft.Maui.Graphics;

namespace BPMNEngine.Drawing
{
#pragma warning disable S101 // Types should be named in PascalCase
    internal class AnimatedPNG : IDisposable
#pragma warning restore S101 // Types should be named in PascalCase
    {
        internal class CrcCalculator
        {
            readonly long[] pTable = new long[256];

            //Für Standard CRC32:
            //(Wert kann verändert werden)
            private const long Poly = 0xEDB88320;

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
                            CRC >>= 1;
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

        private readonly List<AnimatedFrame> parts;
        public TimeSpan DefaultFrameDelay { get; set; } = new TimeSpan(0, 0, 1);

        public AnimatedPNG(IImage initialImage)
        {
            var g = Diagram.ProduceImage((int)Math.Ceiling(initialImage.Width), (int)Math.Ceiling(initialImage.Height));
            var surface = g.Canvas;
            surface.FillColor = Colors.White;
            surface.FillRectangle(0, 0, initialImage.Width, initialImage.Height);
            surface.DrawImage(initialImage, 0, 0, g.Width, g.Height);
            parts = [new(g.Image)];
        }

        public void AddFrame(IImage image, int x, int y, TimeSpan? delay = null)
            => parts.Add(new AnimatedFrame(image, x, y, delay: delay));

        private static readonly byte[] SIGNATURE = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];
        private static readonly Byte[] IEND = [0x00, 0x00, 0x00, 0x00, 0x49, 0x45, 0x4E, 0x44, 0xAE, 0x42, 0x60, 0x82];
        private bool disposedValue;

        public byte[] ToBinary()
        {
            using var memoryStream = new MemoryStream();
            WriteHeader(memoryStream);
            var idx = 0;
            int chunkSequenceNumber = 0;
            parts.ForEach(part =>
            {
                WriteFrameHeader(memoryStream, part, ref chunkSequenceNumber);
                AnimatedPNG.WriteFrame(memoryStream, idx, part, ref chunkSequenceNumber);
                idx++;
            });
            AnimatedPNG.Write(memoryStream, IEND);
            return memoryStream.ToArray();
        }

        private static void WriteFrame(Stream output, int idx, AnimatedFrame part, ref int chunkSequenceNumber)
        {
            if (idx==0)
                AnimatedPNG.Write(output, part.IDAT.SelectMany(c => c));
            else
            {
                var sequenceNumber = chunkSequenceNumber;
                part.IDAT
                    .Where(idat => idat!=null)
                    .ForEach(idat =>
                    {
                        using var memoryStream = new MemoryStream();

                        var length = idat.Length - 8;
                        AnimatedPNG.Write(memoryStream, length);
                        AnimatedPNG.Write(memoryStream, "fdAT");
                        AnimatedPNG.Write(memoryStream, sequenceNumber);

                        AnimatedPNG.Write(memoryStream, idat.Take(idat.Length-4).Skip(8));

                        var data = memoryStream.ToArray();

                        AnimatedPNG.Write(output, data);
                        AnimatedPNG.WriteCRC(output, data);

                        sequenceNumber++;
                    });
                chunkSequenceNumber=sequenceNumber;
            }
        }

        private void WriteFrameHeader(Stream output, AnimatedFrame part, ref int chunkSequenceNumber)
        {
            AnimatedPNG.Write(output, [0, 0, 0, 26]);
            using var memoryStream = new MemoryStream();
            AnimatedPNG.Write(memoryStream, "fcTL");
            AnimatedPNG.Write(memoryStream, chunkSequenceNumber);
            AnimatedPNG.Write(memoryStream, (int)part.Size.Width);
            AnimatedPNG.Write(memoryStream, (int)part.Size.Height);
            AnimatedPNG.Write(memoryStream, part.X);
            AnimatedPNG.Write(memoryStream, part.Y);
            AnimatedPNG.Write(memoryStream, part.Delay ?? DefaultFrameDelay);
            AnimatedPNG.Write(memoryStream, [0, 1]);
            chunkSequenceNumber++;
            var data = memoryStream.ToArray();
            AnimatedPNG.Write(output, data);
            AnimatedPNG.WriteCRC(output, data);
        }

        private void WriteHeader(Stream output)
        {
            AnimatedPNG.Write(output, SIGNATURE);

            var frame = parts[0];

            AnimatedPNG.Write(output, frame.IHDR);

            var textSignature = "Software Roger Castaldo " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + "_" +
                System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

            AnimatedPNG.Write(output, textSignature.Length);
            AnimatedPNG.Write(output, "tEXt"+textSignature);
            AnimatedPNG.WriteCRC(output, "tEXt"+textSignature);

            AnimatedPNG.Write(output, 8);
            AnimatedPNG.Write(output, "acTL");
            AnimatedPNG.Write(output, parts.Count);
            AnimatedPNG.Write(output, 0);
            AnimatedPNG.Write(output, 0);
        }

        private static void Write(Stream output, IEnumerable<byte> data)
            => AnimatedPNG.Write(output, data.ToArray());

        private static void Write(Stream output, byte[] data)
        {
            if (data!=null)
                output.Write(data, 0, data.Length);
        }

        private static void Write(Stream output, int value)
            => AnimatedPNG.Write(output, BitConverter.GetBytes(value).Reverse().ToArray());

        private static void Write(Stream output, uint value)
            => AnimatedPNG.Write(output, BitConverter.GetBytes(value).Reverse().ToArray());

        private static void Write(Stream output, string value)
            => AnimatedPNG.Write(output, value.ToCharArray().Select(c => (byte)c).ToArray());

        private static void Write(Stream output, TimeSpan timespan)
        {
            AnimatedPNG.Write(output, BitConverter.GetBytes((short)timespan.TotalSeconds).Reverse().ToArray());
            AnimatedPNG.Write(output, BitConverter.GetBytes((short)1).Reverse().ToArray());
        }

        private static void WriteCRC(Stream output, byte[] value)
            => AnimatedPNG.Write(output, new CrcCalculator().GetCRC32(value));

        private static void WriteCRC(Stream output, string value)
            => AnimatedPNG.WriteCRC(output, value.ToCharArray().Select(c => (byte)c).ToArray());

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    parts.ForEach(part => part.Dispose());
                    parts.Clear();
                }
                disposedValue=true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
