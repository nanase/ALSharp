using System;
using ALSharp.Components;

namespace ALSharp.Streams
{
	public sealed class RawWaveStream : AudioStream
	{
		private int bitDepth;
		private int frequency;
		private int channelCount;

		public RawWaveStream (int bitDepth, int frequency, int channelCount)
		{
			this.bitDepth = bitDepth;
			this.frequency = frequency;
			this.channelCount = channelCount;
		}

		public override AudioFormatEnum Format
		{
			get
			{
				return AudioBuffer.GetFormat (this.bitDepth, this.channelCount);
			}
		}

		public override int Frequency
		{
			get
			{
				return this.frequency;
			}
		}

		public int BitDepth
		{
			get
			{
				return this.bitDepth;
			}
		}

		public int ChannelCount
		{
			get
			{
				return this.channelCount;
			}
		}

		public override bool CanSeekTime
		{
			get
			{
				return false;
			}
		}

		public override TimeSpan TimePosition
		{
			get
			{
				throw new NotImplementedException ();
			}
			set
			{
				throw new NotImplementedException ();
			}
		}

		public override TimeSpan TimeLength
		{
			get
			{
				throw new NotImplementedException ();
			}
		}

		public override bool CanSeekTrack
		{
			get
			{
				return false;
			}
		}

		public override int TrackPosition
		{
			get
			{
				throw new NotImplementedException ();
			}
			set
			{
				throw new NotImplementedException ();
			}
		}

		public override int TrackCount
		{
			get
			{
				throw new NotImplementedException ();
			}
		}

		public override bool CanRead
		{
			get
			{
				return true;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return false;
			}
		}

		public override long Length
		{
			get
			{
				throw new NotImplementedException ();
			}
		}

		public override long Position
		{
			get
			{
				throw new NotImplementedException ();
			}
			set
			{
				throw new NotImplementedException ();
			}
		}

		public override TimeSpan TimeSeek (TimeSpan offset, System.IO.SeekOrigin origin)
		{
			throw new NotImplementedException ();
		}

		public override int TrackSeek (int offset, System.IO.SeekOrigin origin)
		{
			throw new NotImplementedException ();
		}

		public override void Flush ()
		{
			throw new NotImplementedException ();
		}

		public override int Read (byte[] buffer, int offset, int count)
		{
            return (this.Reading != null) ? this.Reading(buffer, offset, count) : 0;
		}

		public override long Seek (long offset, System.IO.SeekOrigin origin)
		{
			throw new NotImplementedException ();
		}

		public override void SetLength (long value)
		{
			throw new NotImplementedException ();
		}

		public override void Write (byte[] buffer, int offset, int count)
		{
			throw new NotImplementedException ();
		}

		public event Func<byte[], int, int, int> Reading;
	}
}