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
using ALSharp.Bind;

namespace ALSharp.Components
{
	public sealed class AudioDevice : OpenAlHandleResource
	{
		#region static
		public static string DefaultDeviceSpecifier
		{
			get
			{
				return Alc.alcGetString(IntPtr.Zero, Alc.ALC_DEFAULT_DEVICE_SPECIFIER);
			}
		}
		public static string DeviceSpecifiers
		{
			get
			{
				return Alc.alcGetString(IntPtr.Zero, Alc.ALC_DEVICE_SPECIFIER);
			}
		}
		#endregion

		#region constructors
		internal AudioDevice(IntPtr handle)
			: base(true)
		{
			Handle = handle;
		}
		public AudioDevice() : this(DefaultDeviceSpecifier) { }
		public AudioDevice(string deviceName)
			: base(false)
		{
			if (deviceName == null)
			{
				throw new System.ArgumentNullException("deviceName");
			}
			Handle = Alc.alcOpenDevice(deviceName);
			if (Handle == IntPtr.Zero)
			{
				throw new OpenAlException("The Device failed to initialize");
			}
		}
		#endregion

		#region properties
		public string DeviceSpecifier
		{
			get
			{
				if (!IsValid) { throw new ObjectDisposedException("AudioDevice"); }
				return Alc.alcGetString(Handle, Alc.ALC_DEVICE_SPECIFIER);
			}
		}
		public string AlcExtensions
		{
			get
			{
				if (!IsValid) { throw new ObjectDisposedException("AudioDevice"); }
				return Alc.alcGetString(Handle, Alc.ALC_EXTENSIONS);
			}
		}
		#endregion

		#region methods
		public bool HasExtension(string extensionName)
		{
			if (extensionName == null)
			{
				throw new System.ArgumentNullException("extensionName");
			}
			if (!IsValid) { throw new ObjectDisposedException("AudioDevice"); }
			return Alc.alcIsExtensionPresent(Handle, extensionName) == Alc.ALC_TRUE;
		}
		protected override void CloseHandle()
		{
			Alc.alcCloseDevice(Handle);
		}
		#endregion
	}
}