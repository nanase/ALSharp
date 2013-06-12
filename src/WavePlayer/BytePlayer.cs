using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ALSharp.Utils;

namespace ALSharp
{
    public class BytePlayer : WavePlayer
    {
        private readonly Func<byte[], int, int, int> bufferProcess;

        public BytePlayer(Func<byte[], int, int, int> bufferProcess)
            : this(bufferProcess, new PlayerSettings())
        {
        }

        public BytePlayer(Func<byte[], int, int, int> bufferProcess, PlayerSettings settings)
            : base(settings)
        {
            this.bufferProcess = bufferProcess;
        }

        public override int WaveGenerate(byte[] buffer, int offset, int count)
        {
            int bufferedCount;

            bufferedCount = this.bufferProcess(buffer, offset, count);

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
