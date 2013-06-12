using System;
using ALSharp.Utils;

namespace ALSharp
{
    public class DoublePlayer : WavePlayer
    {
        private readonly Func<double[], int, int, int> bufferProcess;
        readonly private double[] buffer = null;

        public DoublePlayer(Func<double[], int, int, int> bufferProcess)
            : this(bufferProcess, new PlayerSettings())
        {
        }

        public DoublePlayer(Func<double[], int, int, int> bufferProcess, PlayerSettings settings)
            : base(settings)
        {
            this.bufferProcess = bufferProcess;
            this.buffer = new double[this.settings.BufferSize / (this.settings.BitPerSample / 8)];
        }

        public override int WaveGenerate(byte[] buffer, int offset, int count)
        {
            int bufferedCount;

            if (this.settings.BitPerSample == 16)
            {
                bufferedCount = this.bufferProcess(this.buffer, offset / 2, count / 2);
                ArrayConverter.DoubleToBytesAsInt16(this.buffer, buffer, offset / 2, bufferedCount);
                bufferedCount *= 2;
            }
            else
            {
                bufferedCount = this.bufferProcess(this.buffer, offset, count);
                ArrayConverter.DoubleToBytesAsInt8(this.buffer, buffer, offset, bufferedCount);
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
