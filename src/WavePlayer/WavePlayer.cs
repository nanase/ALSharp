using System;
using System.Threading;
using System.Threading.Tasks;
using ALSharp.Components;
using ALSharp.Streams;

namespace ALSharp
{
	public abstract class WavePlayer : IDisposable
	{
		readonly private RawWaveStream baseStream;
		readonly private AudioSource audioSource;		
		readonly protected PlayerSettings settings;

        private AudioStreamPlayer basePlayer;
        private Task updater;

		public AudioStreamPlayer BasePlayer { get { return this.basePlayer; } }
		public RawWaveStream BaseStream { get { return this.baseStream; } }
		public AudioSource AudioSource { get { return this.audioSource; } }
		public PlayerSettings Settings { get { return this.settings; } }

        public WavePlayer()
            : this(new PlayerSettings())
        {
        }

        public WavePlayer(PlayerSettings settings)
        {
            OpenAL.AlutInit();
            this.settings = settings;            

            this.baseStream = new RawWaveStream(this.settings.BitPerSample, this.settings.SamplingFrequency, this.settings.ChannelCount);
            this.audioSource = new AudioSource();

            this.baseStream.Reading += Generate;
        }

        public void Play()
        {
            if (this.updater == null)
            {
                this.basePlayer = new AudioStreamPlayer(this.baseStream, this.audioSource, this.settings.BufferSize, this.settings.BufferCount);
                this.basePlayer.Play();
                this.updater = Task.Factory.StartNew(this.Update, TaskCreationOptions.LongRunning);
            }
        }

        public void Stop()
        {
            if (this.updater != null)
            {
                this.basePlayer.Stop();
                this.updater.Wait();
                this.updater.Dispose();

                this.updater = null;
            }
        }

        public abstract int Generate(byte[] buffer, int offset, int count);

		private void Update()
		{
			while (this.basePlayer.Playing)
			{
				this.basePlayer.Update();
				Thread.Sleep(this.settings.UpdateInterval);
			}
		}

		public void Dispose()
		{
            this.Stop();

			this.basePlayer.Dispose();
			this.audioSource.Dispose();
			this.baseStream.Dispose();

			OpenAL.AlutExit();
		}
	}
}
