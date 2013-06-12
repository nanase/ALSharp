using System;

namespace ALSharp
{
	public class PlayerSettings
	{
		public int BitPerSample { get; set; }

		public int SamplingFrequency { get; set; }

		public int ChannelCount { get; set; }

		public int BufferSize { get; set; }

		public int BufferCount { get; set; }

		public int UpdateInterval { get; set; }

		public PlayerSettings ()
		{
			this.BitPerSample = 16;
			this.SamplingFrequency = 44100;
			this.ChannelCount = 2;
			this.BufferSize = 2048;
			this.BufferCount = 32;
			this.UpdateInterval = 10;
		}
	}
}
