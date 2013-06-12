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
using System.Runtime.InteropServices;
using ALSharp.Bind;

namespace ALSharp.Components
{
	public sealed class AudioCaptureDevice : OpenAlHandleResource
	{
		#region static
		const int ALC_CAPTURE_DEVICE_SPECIFIER = 0x310;
		const int ALC_CAPTURE_DEFAULT_DEVICE_SPECIFIER = 0x311;
		const int ALC_CAPTURE_SAMPLES = 0x312;

		public static string DefaultDeviceSpecifier
		{
			get
			{
				return Alc.alcGetString(IntPtr.Zero, ALC_CAPTURE_DEFAULT_DEVICE_SPECIFIER);
			}
		}
		public static string DeviceSpecifiers
		{
			get
			{
				return Alc.alcGetString(IntPtr.Zero, ALC_CAPTURE_DEVICE_SPECIFIER);
			}
		}
		#endregion

		#region fields
		AudioFormatEnum format;
		int frequency;
		int maxSampleCount;
		#endregion

		#region constructors
		public AudioCaptureDevice(string deviceName, AudioFormatEnum format, int frequency, int maxSampleCount)
			: base(false)
		{
			Handle = Alc.alcCaptureOpenDevice(deviceName, frequency, (int)format, maxSampleCount);
			this.format = format;
			this.maxSampleCount = maxSampleCount;
			this.frequency = frequency;
			if (Handle == IntPtr.Zero)
			{
				throw new OpenAlException("The Device failed to initialize");
			}
		}
		#endregion

		#region properties
		public AudioFormatEnum Format
		{
			get { return format; }
		}
		public int Frequency
		{
			get { return frequency; }
		}
		public int SampleSize
		{
			get
			{
				return AudioBuffer.GetSampleSize(format);
			}
		}
		public int MaxSampleCount
		{
			get { return maxSampleCount; }
		}
		public int AvaliabeSampleCount
		{
			get
			{
				if (!IsValid) { throw new ObjectDisposedException("AudioCaptureDevice"); }
				int rv;
				Alc.alcGetIntegerv(Handle, ALC_CAPTURE_SAMPLES, 1, out rv);
				OpenAlException.CheckAlc(Handle);
				return rv;
			}
		}
		public string DeviceSpecifier
		{
			get
			{
				if (!IsValid) { throw new ObjectDisposedException("AudioCaptureDevice"); }
				return Alc.alcGetString(Handle, ALC_CAPTURE_DEVICE_SPECIFIER);
			}
		}
		#endregion

		#region methods
		public byte[] CaptureSamples()
		{
			return CaptureSamples(AvaliabeSampleCount);
		}
		public byte[] CaptureSamples(int sampleCount)
		{
			byte[] rv = new byte[sampleCount * SampleSize];
			CaptureSamples(rv, sampleCount);
			return rv;
		}

		public void CaptureSamples(AudioBuffer buffer)
		{
			CaptureSamples(buffer, AvaliabeSampleCount);
		}
		public void CaptureSamples(AudioBuffer buffer, int sampleCount)
		{
			buffer.BufferData(format, frequency, CaptureSamples(sampleCount));
		}
		public void CaptureSamples(Byte[] buffer, int sampleCount)
		{
			this.CaptureSamples(buffer, 0, sampleCount);
		}
		public void CaptureSamples(Byte[] buffer, int index, int sampleCount)
		{
			if (!IsValid) { throw new ObjectDisposedException("AudioCaptureDevice"); }
			if (sampleCount * SampleSize > buffer.Length - index)
			{
				throw new ArgumentOutOfRangeException("count", "The buffer cannot hold the number of samples");
			}
			GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
			try
			{
				IntPtr bufferPtr = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, index);
				Alc.alcCaptureSamples(Handle, bufferPtr, sampleCount);
			}
			finally
			{
				handle.Free();
			}
			OpenAlException.CheckAlc(Handle);
		}

		public void Start()
		{
			if (!IsValid) { throw new ObjectDisposedException("AudioCaptureDevice"); }
			Alc.alcCaptureStart(Handle);
			OpenAlException.CheckAlc(Handle);
		}
		public void Stop()
		{
			if (!IsValid) { throw new ObjectDisposedException("AudioCaptureDevice"); }
			Alc.alcCaptureStop(Handle);
			OpenAlException.CheckAlc(Handle);
		}
		protected override void CloseHandle()
		{
			Alc.alcCaptureCloseDevice(Handle);
		}
		#endregion
	}

}