#region Licensing
#region OpenALDotNet LGPL
/*
 * OpenALDotNet is a Object Oriented C# wrapper for the OpenAL library. 
 * For the latest info, see https://sourceforge.net/projects/OpenALDotNet
 * Copyright (C) 2006  Jonathan Mark Porter
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 * 
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA
 */
#endregion
#region Manual (comments) License
/*
 * Copyright © 1999-2000 by Loki Software
 * Permission is granted to make and distribute verbatim copies of this manual 
 * provided the copyright notice and this permission notice are preserved 
 * on all copies.
 */
#endregion
#endregion

using System;
using ALSharp.Components;

namespace ALSharp.Streams
{
	public sealed class AudioCaptureStream : AudioStream
	{
		private AudioCaptureDevice captureDevice;

		public AudioCaptureStream(AudioCaptureDevice device)
		{
			this.captureDevice = device;
		}

		public AudioCaptureDevice CaptureDevice
		{
			get { return captureDevice; }
		}

		public override AudioFormatEnum Format
		{
			get { return captureDevice.Format; }
		}
		public override int Frequency
		{
			get { return captureDevice.Frequency; }
		}

		public override bool CanRead
		{
			get { return true; }
		}
		public override bool CanSeekTime
		{
			get { return false; }
		}
		public override bool CanSeekTrack
		{
			get { return false; }
		}
		public override bool CanSeek
		{
			get { return false; }
		}
		public override bool CanWrite
		{
			get { return false; }
		}

		public override TimeSpan TimePosition
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}
		public override TimeSpan TimeLength
		{
			get { throw new NotSupportedException(); }
		}
		public override int TrackPosition
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}
		public override long Length
		{
			get { throw new NotSupportedException(); }
		}
		public override long Position
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}
		public override int TrackCount
		{
			get { throw new NotSupportedException(); }
		}

		public override TimeSpan TimeSeek(TimeSpan offset, System.IO.SeekOrigin origin)
		{
			throw new NotSupportedException();
		}
		public override int TrackSeek(int offset, System.IO.SeekOrigin origin)
		{
			throw new NotSupportedException();
		}
		public override long Seek(long offset, System.IO.SeekOrigin origin)
		{
			throw new NotSupportedException();
		}
		public override void Flush()
		{
			throw new NotSupportedException();
		}
		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}
		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if ((offset < 0) || (offset < 0))
			{
				throw new ArgumentOutOfRangeException((offset < 0) ? ("index") : ("offset"), "The value must be greater or equal to 0.");
			}
			if ((buffer.Length - offset) < count)
			{
				throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
			}
			int sampleCount = Math.Min(count / captureDevice.SampleSize, captureDevice.AvaliabeSampleCount);
			if (sampleCount > 0)
			{
				captureDevice.CaptureSamples(buffer, offset, sampleCount);
			}
			return sampleCount * captureDevice.SampleSize;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				captureDevice.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}