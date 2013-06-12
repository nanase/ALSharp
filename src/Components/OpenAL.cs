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
using ALSharp.Bind;

namespace ALSharp.Components
{
	public static class OpenAL
	{
		public static bool Initalized { get; private set; }

		/// <summary>
		/// Gets the version string in format [spec major number].[spec minor number] [optional vendor version information]・
		/// </summary>
		public static string Version {
			get {
				OpenAL.AlutInit ();
				return Al.alGetString (Al.AL_VERSION);
			}
		}
		/// <summary>
		/// Gets the information about the specific renderer
		/// </summary>
		public static string Renderer {
			get {
				OpenAL.AlutInit ();
				return Al.alGetString (Al.AL_RENDERER);
			}
		}
		/// <summary>
		/// Gets the name of the vendor
		/// </summary>
		public static string Vendor {
			get {
				OpenAL.AlutInit ();
				return Al.alGetString (Al.AL_VENDOR);
			}
		}
		/// <summary>
		/// Gets a list of available extensions separated by spaces
		/// </summary>
		public static string[] Extensions {
			get {
				OpenAL.AlutInit ();
				return Al.alGetString (Al.AL_EXTENSIONS).Split (' ');
			}
		}

		public static bool AlutInit ()
		{
			if (Initalized)
				return false;

			bool rv = Alut.alutInit (null, null);
			OpenAlException.CheckAlut ();
			AudioContext.SetAlutContext ();

			Initalized = rv;

			return rv;
		}

		public static bool AlutInit (string[] argv)
		{
			if (Initalized)
				return false;

			bool rv = Alut.alutInit (GenerateArgcp (argv), argv);
			OpenAlException.CheckAlut ();
			AudioContext.SetAlutContext ();

			Initalized = rv;

			return rv;
		}

		public static bool AlutInitWithoutContext (string[] argv)
		{
			if (Initalized)
				return false;

			bool rv = Alut.alutInitWithoutContext (GenerateArgcp (argv), argv);
			OpenAlException.CheckAlut ();

			Initalized = rv;

			return rv;
		}

		public static bool AlutExit ()
		{
			if (!Initalized)
				return false;

			bool rv = Alut.alutExit ();
			OpenAlException.CheckAlut ();

			Initalized = !rv;

			return rv;
		}
		
		private static int[] GenerateArgcp (string[] argv)
		{
			int[] rv = new int[argv.Length + 1];
			rv [0] = argv.Length;
			
			for (int i = 0; i < argv.Length; i++)
				rv [i + 1] = argv [i].Length;
			
			return rv;
		}
		
//		public static string SupportedMIMETypes
//		{
//			get
//			{
//				OpenAL.AlutInit();
//				return Alut.alutGetMIMETypes(Alut.ALUT_LOADER_BUFFER);
//			}
//		}
//		public static string MemoryMIMETypes
//		{
//			get
//			{
//				OpenAL.AlutInit();
//				return Alut.alutGetMIMETypes(Alut.ALUT_LOADER_MEMORY);
//			}
//		}
	}
}
