using System;
using System.IO;

namespace WoomLink
{
    public class TemporarySeekHandle : IDisposable
    {
        private readonly Stream Stream;
        private readonly long RetPos;
        public TemporarySeekHandle(Stream stream, long retpos)
        {
            Stream = stream;
            RetPos = retpos;
        }
        public void Dispose()
        {
            Stream.Seek(RetPos, SeekOrigin.Begin);
            GC.SuppressFinalize(this);
        }
    }
}
