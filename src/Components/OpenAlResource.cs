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

namespace ALSharp.Components
{
	public abstract class OpenAlResource : IDisposable
	{
		public abstract bool IsValid { get; }
		protected abstract void Dispose(bool disposing);
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
	public abstract class OpenAlHandleResource : OpenAlResource
	{
		public event EventHandler Disposing;
		private IntPtr handle;
		private bool alutOwned;
		public OpenAlHandleResource(bool alutOwned)
		{
			this.alutOwned = alutOwned;
		}
		~OpenAlHandleResource()
		{
			Dispose(false);
		}
		public IntPtr Handle
		{
			get { return handle; }
			protected set { handle = value; }
		}
		public bool AlutOwned
		{
			get { return alutOwned; }
		}
		public sealed override bool IsValid
		{
			get { return handle != IntPtr.Zero; }
		}
		protected abstract void CloseHandle();
		protected void OnDisposing(EventArgs e)
		{
			if (Disposing != null)
			{
				Disposing(this, e);
			}
		}
		protected override void Dispose(bool disposing)
		{
			lock (this)
			{
				if (handle != IntPtr.Zero)
				{
					OnDisposing(EventArgs.Empty);
					//if its was manualy created destroy it otherwise allow alut to do this
					if (!alutOwned)
					{
						CloseHandle();
					}
					handle = IntPtr.Zero;
				}
			}
		}
	}
	public abstract class OpenAlIDResource : OpenAlResource
	{
		internal static void GetIDs<T>(IList<T> resources, int[] ids, int nullValue)
			where T : OpenAlIDResource
		{
			int count = resources.Count;
			for (int index = 0; index < count; ++index)
			{
				T val = resources[index];
				ids[index] = (val == null) ? (nullValue) : (val.ID);
			}
		}

		protected internal int ID;
		bool isValid = true;

		/// <summary>
		/// Gets if it is a valid and will be reconized by Openal.
		/// </summary>
		public sealed override bool IsValid
		{
			get { return isValid && IsIDValid; }
		}
		protected abstract bool IsIDValid { get; }

		protected internal abstract void Dispose(bool disposing, bool releaseID);

		protected sealed override void Dispose(bool disposing)
		{
			Dispose(disposing, true);
		}

		protected void SetInvalid()
		{
			this.isValid = false;
			this.ID = -1;
		}
	}
}