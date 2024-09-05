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

        private readonly MemoryStream data;
        private bool disposedValue;

        public byte[] IHDR => Find("IHDR")?[0];

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
            data = new();
            image.Save(data, ImageFormat.Png);
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
                data.Read(bytes, 0, search.Length);
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

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    data.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue=true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~AnimatedFrame()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
