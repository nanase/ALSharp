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
using System.Collections.Generic;
using ALSharp.Bind;

namespace ALSharp.Components
{
	public sealed class AudioBuffer : OpenAlIDResource
	{
		#region static
		public static readonly AudioBuffer NullBuffer = new AudioBuffer (NullBufferID);
		internal const int NullBufferID = Al.AL_NONE;

		public static void ReleaseBuffers (IList<AudioBuffer> buffers)
		{
			if (buffers == null)
			{
				throw new ArgumentNullException ("buffers");
			}
			int count = buffers.Count;
			if (count > 0)
			{
				int[] bufferIds = new int[count];
				for (int pos = 0; pos < count; ++pos)
				{
					bufferIds [pos] = buffers [pos].ID;
				}
				
				if (Environment.OSVersion.Platform != PlatformID.Unix)
				{
					Al.alDeleteBuffers (count, bufferIds);
					OpenAlException.CheckAl ();
				}
				
				for (int pos = 0; pos < count; ++pos)
				{
					buffers [pos].Dispose (true, false);
				}
			}
		}

		public static AudioBuffer[] CreateBuffers (int count)
		{
			int[] ids = new int[count];
			Al.alGenBuffers (count, ids);
			OpenAlException.CheckAl ();
			return Array.ConvertAll<int, AudioBuffer> (ids, FromID);
		}

		public static AudioBuffer LoadFile (string fileName)
		{
			int Id = (int)Alut.alutCreateBufferFromFile (fileName);
			OpenAlException.CheckAlut ();
			return new AudioBuffer (Id);
		}

		public static AudioBuffer CreateWaveForm (WaveFormEnum waveShape, float frequency, float phase, float duration)
		{
			int Id = (int)Alut.alutCreateBufferWaveform ((int)waveShape, frequency, phase, duration);
			OpenAlException.CheckAlut ();
			return new AudioBuffer (Id);
		}

		internal static int GetID (AudioBuffer buffer)
		{
			return (buffer == null) ? (NullBufferID) : (buffer.ID);
		}

		internal static AudioBuffer FromID (int id)
		{
			return new AudioBuffer (id);
		}

		public static int GetSampleSize (AudioFormatEnum format)
		{
			switch (format)
			{
				case AudioFormatEnum.Mono16:
					return 2;
				case AudioFormatEnum.Mono8:
					return 1;
				case AudioFormatEnum.Stereo16:
					return 4;
				case AudioFormatEnum.Stereo8:
					return 2;
				default:
					throw new Exception ();
			}
		}

		public static int GetSampleSize (int bits, int channels)
		{
			return (bits / 8) * channels;
		}

		public static AudioFormatEnum GetFormat (int bits, int channels)
		{
			switch (bits)
			{
				case 8:
					switch (channels)
					{
						case 1:
							return AudioFormatEnum.Mono8;
						case 2:
							return AudioFormatEnum.Stereo8;
					}
					break;
				case 16:
					switch (channels)
					{
						case 1:
							return AudioFormatEnum.Mono16;
						case 2:
							return AudioFormatEnum.Stereo16;
					}
					break;
			}
			throw new Exception ("Invalid format");
		}

		/// <summary>
		/// Dont know if it works or if it even is close, but thought it might be usefull.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="bits"></param>
		/// <param name="channels"></param>
		private static void SplitMultiChannelAudio (Byte[] source, int bits, params Byte[][] channels)
		{

			if (source == null)
			{
				throw new ArgumentNullException ("source");
			}
			if (channels == null)
			{
				throw new ArgumentNullException ("channels");
			}
			int channelCount = channels.Length;
			int channelLength = source.Length / channelCount;
			if (channelCount * channelLength != source.Length)
			{
				throw new ArgumentException ("wrong length", "source");
			}
			foreach (Byte[] arr in channels)
			{
				if (arr == null)
				{
					throw new ArgumentNullException ("channels");
				}
				if (arr.Length != channelLength)
				{
					throw new ArgumentException ("wrong length", "channels");
				}
			}
			int byteCount = bits / 8;
			for (int index = 0; index < source.Length; ++index)
			{
				for (int channel = 0; channel < channelCount; ++channel)
				{
					for (int index2 = 0; index2 < byteCount; ++index2)
					{
						channels [channel] [index + index2] = source [index * (channelCount * index2) + channel + index2];
					}
				}
			}
		}
		#endregion

		#region constructors
		public AudioBuffer ()
		{
			Al.alGenBuffers (1, out this.ID);
			OpenAlException.CheckAl ();
		}

		internal AudioBuffer (int iD)
		{
			this.ID = iD;
		}
		#endregion

		#region properties

		/// <summary>
		/// Gets the frequency of buffer in Hz
		/// </summary>
		public int Frequency
		{
			get
			{
				int rv;
				Al.alGetBufferi (ID, Al.AL_FREQUENCY, out rv);
				OpenAlException.CheckAl ();
				return rv;
			}
		}
		/// <summary>
		/// Gets the size of buffer in bytes
		/// </summary>
		public int Size
		{
			get
			{
				int rv;
				Al.alGetBufferi (ID, Al.AL_SIZE, out rv);
				OpenAlException.CheckAl ();
				return rv;
			}
		}
		/// <summary>
		/// Gets the bit depth of buffer
		/// </summary>
		public int Bits
		{
			get
			{
				int rv;
				Al.alGetBufferi (ID, Al.AL_BITS, out rv);
				OpenAlException.CheckAl ();
				return rv;
			}
		}
		/// <summary>
		/// Gets the number of channels in buffer
		/// > 1 is valid, but buffer won稚 be positioned when played
		/// </summary>
		public int Channels
		{
			get
			{
				int rv;
				Al.alGetBufferi (ID, Al.AL_CHANNELS, out rv);
				OpenAlException.CheckAl ();
				return rv;
			}
		}

		/// <summary>
		/// Gets the Format based on Bits and Channels;
		/// </summary>
		public AudioFormatEnum Format
		{
			get
			{
				switch (Bits)
				{
					case 8:
						switch (Channels)
						{
							case 1:
								return AudioFormatEnum.Mono8;
							case 2:
								return AudioFormatEnum.Stereo8;
						}
						break;
					case 16:
						switch (Channels)
						{
							case 1:
								return AudioFormatEnum.Mono16;
							case 2:
								return AudioFormatEnum.Stereo16;
						}
						break;
				}
				throw new Exception ();
			}
		}
		/// <summary>
		/// Gets the number of Samples by Size / GetSampleSize(Bits, Channels)
		/// </summary>
		public int Samples
		{
			get
			{
				return Size / GetSampleSize (Bits, Channels);
			}
		}
		/// <summary>
		/// Gets the seconds of play time in the buffer based on Samples / Frequency
		/// </summary>
		public float Seconds
		{
			get
			{
				return (float)Samples / (float)Frequency;
			}
		}

		/// <summary>
		/// Gets if the buffer is null.
		/// </summary>
		public bool IsNull
		{
			get
			{
				return ID == NullBufferID;
			}
		}

		protected override bool IsIDValid
		{
			get
			{
				return Al.alIsBuffer (ID) == Al.AL_TRUE;
			}
		}
		#endregion

		#region methods
		public void BufferData (AudioFormatEnum format, int frequency, Byte[] data)
		{
			this.BufferData (format, frequency, data, data.Length);
		}

		public void BufferData (AudioFormatEnum format, int frequency, Byte[] data, int dataLength)
		{
			Al.alBufferData (ID, (int)format, data, dataLength, frequency);
			OpenAlException.CheckAl ();
		}

		protected internal override void Dispose (bool disposing, bool releaseID)
		{
			if (IsValid)
			{
				if (releaseID)
				{
					Al.alDeleteBuffers (1, ref ID);
					OpenAlException.CheckAl ();
				}
				SetInvalid ();
			}
		}
		#endregion
	}
}