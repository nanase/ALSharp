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
using System.Collections.Generic;
using ALSharp.Components;
using ALSharp.Bind;

namespace ALSharp.Collections
{
	public sealed class AudioBufferList : OpenAlIDResourceList<AudioBuffer>
	{
		public AudioBufferList () : base()
		{
		}

		public AudioBufferList (IEnumerable<AudioBuffer> collection) : base(collection)
		{
		}

		public AudioBufferList (int capacity) : base(capacity)
		{
		}

		protected sealed override void Release (int count, ref int id)
		{
			Al.alDeleteBuffers (count, ref id);
		}

		protected sealed override int NullID {
			get { return AudioBuffer.NullBufferID; }
		}
	}
}