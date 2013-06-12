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
using ALSharp.Components;

namespace ALSharp.Collections
{
	public abstract class OpenAlIDResourceList<T> : List<T> where T : OpenAlIDResource
	{
		static int[] EmptyIds = new int[0];
		private int[] ids;

		public OpenAlIDResourceList () : base()
		{
			this.ids = EmptyIds;
		}

		public OpenAlIDResourceList (IEnumerable<T> collection) : base(collection)
		{
			this.ids = new int[this.Capacity];
		}

		public OpenAlIDResourceList (int capacity) : base(capacity)
		{
			this.ids = new int[this.Capacity];
		}

		protected int[] IDs
		{
			get
			{
				if (ids.Length < Count)
					ids = new int[Capacity];
				
				OpenAlIDResource.GetIDs<T> (this, ids, NullID);
				return ids;
			}
		}

		protected abstract void Release (int count, ref int id);

		protected abstract int NullID { get; }

		public void ForRange (Action<T> action, int index, int count)
		{
			if ((index < 0) || (count < 0)) 
				throw new ArgumentOutOfRangeException ((index < 0) ? ("index") : ("count"), "The value must be greater or equal to 0.");
			
			if ((Count - index) < count) 
				throw new ArgumentException ("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
			
			if (count == 0)
				return;
			
			for (int i = index, j = 0; j < count; ++i, ++j)
				action (base [i]);
		}

		public void Release ()
		{
			ReleaseRange (0, Count);
		}

		public void ReleaseAt (int index)
		{
			ReleaseRange (index, 1);
		}

		public void ReleaseRange (int index, int count)
		{
			if ((index < 0) || (count < 0)) 
				throw new ArgumentOutOfRangeException ((index < 0) ? ("index") : ("count"), "The value must be greater or equal to 0.");
			
			if ((Count - index) < count) 
				throw new ArgumentException ("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
			
			
			if (count == 0)
				return;
			
			Release (count, ref  IDs [index]);
			OpenAlException.CheckAl ();
			for (int i = index, j = 0; j < count; ++i, ++j) 
				base [i].Dispose (true, false);
				
			RemoveRange (index, count);
		}
	}
}