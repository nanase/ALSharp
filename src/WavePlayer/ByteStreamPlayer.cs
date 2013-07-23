using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ALSharp.Utils;

namespace ALSharp
{
    public class ByteStreamPlayer : WavePlayer
    {
        private readonly Stream stream;

        public ByteStreamPlayer(Stream stream)
            : this(stream, new PlayerSettings())
        {
        }

        public ByteStreamPlayer(Stream stream, PlayerSettings settings)
            : base(settings)
        {
            this.stream = stream;
        }

        public override int Generate(byte[] buffer, int offset, int count)
        {
            int bufferedCount;

            bufferedCount = this.stream.Read(buffer, offset, count);

            if (bufferedCount == 0)
            {
                for (int i = offset; i < count; i++)
                    buffer[i] = 0;

                return count;
            }
            else
                return bufferedCount;
        }
    }
}
