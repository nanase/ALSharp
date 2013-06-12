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
	public class OpenAlException : Exception
	{
		public static void CheckAl ()
		{
			CheckAl (Al.alGetError ());
		}

		public static void CheckAl (int errorcode)
		{
			if (errorcode != Al.AL_NO_ERROR)
			{
				throw new OpenAlException (Al.alGetString (errorcode));
			}
		}

		public static void CheckAlc (IntPtr handle)
		{
			CheckAlc (Alc.alcGetError (handle));
		}

		public static void CheckAlc (int errorcode)
		{
			switch (errorcode)
			{
				case Alc.ALC_NO_ERROR:
					return;
				case Alc.ALC_INVALID_DEVICE:
					throw new OpenAlException ("ALC_INVALID_DEVICE");
				case Alc.ALC_INVALID_CONTEXT:
					throw new OpenAlException ("ALC_INVALID_CONTEXT");
				case Alc.ALC_INVALID_ENUM:
					throw new OpenAlException ("ALC_INVALID_ENUM");
				case Alc.ALC_INVALID_VALUE:
					throw new OpenAlException ("ALC_INVALID_VALUE");
				case Alc.ALC_OUT_OF_MEMORY:
					throw new OpenAlException ("ALC_OUT_OF_MEMORY");
			}
		}

		public static void CheckAlut ()
		{
			CheckAlut (Alut.alutGetError ());
		}

		public static void CheckAlut (int errorcode)
		{
			if (errorcode != Alut.ALUT_ERROR_NO_ERROR)
			{
				throw new OpenAlException (Alut.alutGetErrorString (errorcode));
			}
		}

		public OpenAlException (string message)
			: base(message)
		{
		}
	}
}