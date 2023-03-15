using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Org.Reddragonit.BpmEngine.Drawing
{
    internal class AnimatedFrame : IDisposable
    {
        private readonly int _x;
        public int X => _x;

        private readonly int _y;
        public int Y => _y;

        private TimeSpan? _delay;
        public TimeSpan? Delay => _delay;

        private Size _size;
        public Size Size => _size;

        private Stream _data;

        public byte[] IHDR =>find("IHDR")?[0];

        public List<byte[]> IDAT => find("IDAT");

        public AnimatedFrame(IImage image)
            : this(image, 0, 0)
        { }

        public AnimatedFrame(IImage image, int x, int y, TimeSpan? delay = null)
        {
            _size = new Size(image.Width, image.Height);
            _x = x;
            _y = y;
            _delay=delay;
            _data = new MemoryStream();
            image.Save(_data,ImageFormat.Png);
        }

        private List<Byte[]> find(string search)
        {
            List<Byte[]> result = new List<byte[]>();
            var searchBytes = search.Select(c => (byte)c).ToArray();
            byte[] bytes = new byte[search.Length];
            int i = 0;
            int found = 0;
            while (i < _data.Length - 4)
            {
                _data.Flush();
                _data.Position = i;
                var debug = _data.Read(bytes, 0, search.Length);
                i++;
                if (bytes.SequenceEqual(searchBytes))
                {
                    Byte[] rawLength = new Byte[4];
                    _data.Position -= 8;
                    _data.Read(rawLength, 0, 4);
                    Array.Reverse(rawLength);
                    UInt32 length = BitConverter.ToUInt32(rawLength, 0);
                    result.Add(new Byte[length + 12]);
                    _data.Position -= 4;
                    _data.Read(result[found], 0, (int)(length + 12));
                    found++;
                }
            }
            return result;
        }


        public void Dispose()
        {
            try
            {
                _data.Dispose();
            }
            catch (Exception) { }
        }
    }
}
