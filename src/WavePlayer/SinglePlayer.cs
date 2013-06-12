using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ALSharp.Utils;

namespace ALSharp
{
    public class SinglePlayer : WavePlayer
    {
        private readonly Func<float[], int, int, int> bufferProcess;
        readonly private float[] buffer = null;

        public SinglePlayer(Func<float[], int, int, int> bufferProcess)
            : this(bufferProcess, new PlayerSettings())
        {
        }

        public SinglePlayer(Func<float[], int, int, int> bufferProcess, PlayerSettings settings)
            : base(settings)
        {
            this.bufferProcess = bufferProcess;
            this.buffer = new float[this.settings.BufferSize / (this.settings.BitPerSample / 8)];
        }

        public override int WaveGenerate(byte[] buffer, int offset, int count)
        {
            int bufferedCount;

            if (this.settings.BitPerSample == 16)
            {
                bufferedCount = this.bufferProcess(this.buffer, offset / 2, count / 2);
                ArrayConverter.SingleToBytesAsInt16(this.buffer, buffer, offset / 2, bufferedCount);
                bufferedCount *= 2;
            }
            else
            {
                bufferedCount = this.bufferProcess(this.buffer, offset, count);
                ArrayConverter.SingleToBytesAsInt8(this.buffer, buffer, offset, bufferedCount);
            }

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
