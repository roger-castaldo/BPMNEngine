using Microsoft.Maui.Graphics;
using System.IO;

namespace BPMNEngine.Drawing
{
    internal class AnimatedFrame : IDisposable
    {
        public int X { get; private init; }
        public int Y { get; private init; }
        public TimeSpan? Delay { get; private init; }
        public Size Size { get; private init; }

        private readonly Stream data;

        public byte[] IHDR =>Find("IHDR")?[0];

        public List<byte[]> IDAT => Find("IDAT");

        public AnimatedFrame(IImage image)
            : this(image, 0, 0)
        { }

        public AnimatedFrame(IImage image, int x, int y, TimeSpan? delay = null)
        {
            Size = new Size(image.Width, image.Height);
            X = x;
            Y = y;
            Delay=delay;
            data = new MemoryStream();
            image.Save(data,ImageFormat.Png);
        }

        private List<byte[]> Find(string search)
        {
            var result = new List<byte[]>();
            var searchBytes = search.Select(c => (byte)c).ToArray();
            byte[] bytes = new byte[search.Length];
            int i = 0;
            int found = 0;
            while (i < data.Length - 4)
            {
                data.Flush();
                data.Position = i;
                var debug = data.Read(bytes, 0, search.Length);
                i++;
                if (bytes.SequenceEqual(searchBytes))
                {
                    var rawLength = new byte[4];
                    data.Position -= 8;
                    data.Read(rawLength, 0, 4);
                    Array.Reverse(rawLength);
                    UInt32 length = BitConverter.ToUInt32(rawLength, 0);
                    result.Add(new byte[length + 12]);
                    data.Position -= 4;
                    data.Read(result[found], 0, (int)(length + 12));
                    found++;
                }
            }
            return result;
        }


        public void Dispose()
        {
            try
            {
                data.Dispose();
            }
            catch (Exception) { }
        }
    }
}
