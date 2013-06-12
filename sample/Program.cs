using System;
using ALSharp;

namespace sample
{
    class Program
    {
        static void Main(string[] args)
        {
            double phase = 0.0;
            Func<double[], int, int, int> process = (buffer, offset, count) =>
            {
                for (int i = offset; i < count; i++)
                    buffer[i] = Math.Sin((440.0 * Math.PI) / 44100.0 * (phase + i));

                phase += count;
                return count;
            };

            using (var player = new DoublePlayer(process))
            {
                player.Play();
                Console.ReadKey(true);
            }
        }
    }
}
