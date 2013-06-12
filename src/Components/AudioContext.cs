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
	/// <summary>
	/// Parameters for Creating a context
	/// </summary>
	public sealed class AudioContextParameters
	{
		bool setSync;
		bool setRefreshInterval;
		bool setFrequency;
		bool setMonoSources;
		bool setStereoSources;
		bool sync = true;
		int refreshInterval = 100;
		int frequency = 44100;
		int monoSources;
		int stereoSources;

		public bool SetMonoSources
		{
			get { return setMonoSources; }
			set { setMonoSources = value; }
		}
		public bool SetStereoSources
		{
			get { return setStereoSources; }
			set { setStereoSources = value; }
		}
		public bool SetSync
		{
			get { return setSync; }
			set { setSync = value; }
		}
		public bool SetRefreshInterval
		{
			get { return setRefreshInterval; }
			set { setRefreshInterval = value; }
		}
		public bool SetFrequency
		{
			get { return setFrequency; }
			set { setFrequency = value; }
		}
		public int MonoSources
		{
			get { return monoSources; }
			set { monoSources = value; }
		}
		public int StereoSources
		{
			get { return stereoSources; }
			set { stereoSources = value; }
		}
		public bool Sync
		{
			get { return sync; }
			set { sync = value; }
		}
		public int RefreshInterval
		{
			get { return refreshInterval; }
			set { refreshInterval = value; }
		}
		public int Frequency
		{
			get { return frequency; }
			set { frequency = value; }
		}
		internal int[] GetValue()
		{
			List<int> rv = new List<int>(4);
			if (setSync)
			{
				rv.Add(Alc.ALC_SYNC);
				rv.Add((sync) ? (Alc.ALC_TRUE) : (Alc.ALC_FALSE));
			}
			if (setRefreshInterval)
			{
				rv.Add(Alc.ALC_REFRESH);
				rv.Add(refreshInterval);
			}
			if (setMonoSources)
			{
				rv.Add(Alc.ALC_MONO_SOURCES);
				rv.Add(monoSources);
			}
			if (setStereoSources)
			{
				rv.Add(Alc.ALC_STEREO_SOURCES);
				rv.Add(stereoSources);
			}
			if (setFrequency)
			{
				rv.Add(Alc.ALC_FREQUENCY);
				rv.Add(frequency);
			}
			if (rv.Count == 0)
			{
				return null;
			}
			else
			{
				rv.Add(Alc.ALC_INVALID);
				return rv.ToArray();
			}

		}
	}
	public class ContextChangedEventArgs : EventArgs
	{
		AudioContext oldContext;
		AudioContext newContext;
		public ContextChangedEventArgs(AudioContext oldContext, AudioContext newContext)
		{
			this.oldContext = oldContext;
			this.newContext = newContext;
		}
		public AudioContext OldContext
		{
			get { return oldContext; }
		}
		public AudioContext NewContext
		{
			get { return newContext; }
		}
	}

	public sealed class AudioContext : OpenAlHandleResource
	{
		#region static
		/// <summary>
		/// An event for when the current Context has Changed.
		/// </summary>
		public static event EventHandler<ContextChangedEventArgs> ContextChanged;

		static AudioContext currentContext;
		/// <summary>
		/// The current Context that all sources being created will belong to.
		/// </summary>
		public static AudioContext CurrentContext
		{
			get
			{
				if (currentContext != null && currentContext.IsCurrent)
				{
					return currentContext;
				}
				return null;
			}
			set
			{
				if (currentContext != value)
				{
					ContextChangedEventArgs e = new ContextChangedEventArgs(currentContext, value);
					Alc.alcMakeContextCurrent((value != null) ? (value.Handle) : (IntPtr.Zero));
					currentContext = value;
					if (ContextChanged != null)
					{
						ContextChanged(null, e);
					}
				}
			}
		}
		internal static void SetAlutContext()
		{
			IntPtr context = Alc.alcGetCurrentContext();
			AudioDevice device = new AudioDevice(Alc.alcGetContextsDevice(context));
			currentContext = new AudioContext(device, context);
			if (ContextChanged != null)
			{
				ContextChanged(null, new ContextChangedEventArgs(null, currentContext));
			}
		}
		/// <summary>
		/// Gets or Sets Speed of sound in same units as velocities
		/// </summary>
		public static float SpeedOfSound
		{
			get
			{
				return Al.alGetFloat(Al.AL_SPEED_OF_SOUND);
			}
			set
			{
				Al.alSpeedOfSound(value);
			}
		}
		/// <summary>
		/// (Obsolete) Simular to Speed of Sound
		/// </summary>
		[Obsolete("Use SpeedOfSound instead")]
		public static float DopplerVelocity
		{
			get
			{
				return Al.alGetFloat(Al.AL_DOPPLER_VELOCITY);
			}
			set
			{
				Al.alDopplerVelocity(value);
			}
		}
		/// <summary>
		/// Gets or Sets a simple scaling of source and listener velocities to exaggerate or deemphasize the Doppler (pitch) shift resulting from the calculation.
		/// </summary>
		public static float DopplerFactor
		{
			get
			{
				return Al.alGetFloat(Al.AL_DOPPLER_FACTOR);
			}
			set
			{
				Al.alDopplerFactor(value);
			}
		}
		/// <summary>
		/// The current distance model
		/// </summary>
		public static DistanceModelEnum DistanceModel
		{
			get
			{
				return (DistanceModelEnum)Al.alGetInteger(Al.AL_DISTANCE_MODEL);
			}
			set
			{
				Al.alDistanceModel((int)value);
			}
		}
		#endregion

		#region fields
		AudioDevice device;
		#endregion

		#region constructors
		private AudioContext(AudioDevice device, IntPtr handle)
			: base(true)
		{
			this.Handle = handle;
			this.device = device;
		}
		public AudioContext(AudioDevice device)
			: this(device, null) { }
		public AudioContext(AudioDevice device, AudioContextParameters contextParameters)
			: base(false)
		{
			this.device = device;
			int[] realParameters = null;
			if (contextParameters != null)
			{
				realParameters = contextParameters.GetValue();
			}
			Handle = Alc.alcCreateContext(device.Handle, realParameters);
			if (Handle == IntPtr.Zero)
			{
				throw new OpenAlException("The Context failed to be Created");
			}
			device.Disposing += OnDeviceDisposing;
		}
		#endregion

		#region properties
		/// <summary>
		/// The device the Context belongs to.
		/// </summary>
		public AudioDevice Device
		{
			get { return device; }
		}
		/// <summary>
		/// Gets or Sets if the Context is the current one.
		/// </summary>
		public bool IsCurrent
		{
			get
			{
				if (!IsValid) { throw new ObjectDisposedException("AudioContext"); }
				return Alc.alcGetCurrentContext() == Handle;
			}
			set
			{
				if (!IsValid) { throw new ObjectDisposedException("AudioContext"); }
				if (IsCurrent ^ value)
				{
					CurrentContext = (value) ? (this) : (null);
				}
			}
		}
		/// <summary>
		/// Cuases the Context to be processed even if it is the not the currrent one.
		/// </summary>
		public void Process()
		{
			if (!IsValid) { throw new ObjectDisposedException("AudioContext"); }
			Alc.alcProcessContext(Handle);
		}
		/// <summary>
		/// Stops the the processing of the context.
		/// </summary>
		public void Suspend()
		{
			if (!IsValid) { throw new ObjectDisposedException("AudioContext"); }
			Alc.alcSuspendContext(Handle);
		}
		#endregion

		#region methods
		protected override void CloseHandle()
		{
			if (IsCurrent)
			{
				CurrentContext = null;
			}
			Alc.alcDestroyContext(Handle);
			device.Disposing -= OnDeviceDisposing;
		}
		void OnDeviceDisposing(object sender, EventArgs e)
		{
			this.Dispose();
		}
		#endregion
	}
}