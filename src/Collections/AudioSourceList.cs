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
using ALSharp.Bind;

namespace ALSharp.Collections
{
	public class AudioSourceList : OpenAlIDResourceList<AudioSource>
	{
		public AudioSourceList () : base()
		{
		}

		public AudioSourceList (IEnumerable<AudioSource> collection) : base(collection)
		{
		}

		public AudioSourceList (int capacity) : base(capacity)
		{
		}

		public Vector3D Direction {
			set {
				for (int i = 0; i < Count; ++i) 
					base [i].Direction = value;
			}
		}

		public Vector3D Position {
			set {
				for (int i = 0; i < Count; ++i) 
					base [i].Position = value;
			}
		}

		public Vector3D Velocity {
			set {
				for (int i = 0; i < Count; ++i)
					base [i].Velocity = value;
			}
		}

		public float ConeInnerAngle {
			set {
				for (int i = 0; i < Count; ++i)
					base [i].ConeInnerAngle = value;
			}
		}

		public float ConeOuterAngle {
			set {
				for (int i = 0; i < Count; ++i)
					base [i].ConeOuterAngle = value;
			}
		}

		public float ConeOuterGain {
			set {
				for (int i = 0; i < Count; ++i)
					base [i].ConeOuterGain = value;
			}
		}

		public float Gain {
			set {
				for (int i = 0; i < Count; ++i)
					base [i].Gain = value;
			}
		}

		public bool IsLooping {
			set {
				for (int i = 0; i < Count; ++i)
					base [i].IsLooping = value;
			}
		}

		public bool IsRelative {
			set {
				for (int i = 0; i < Count; ++i)
					base [i].IsRelative = value;
			}
		}

		public float MaxDistance {
			set {
				for (int i = 0; i < Count; ++i)
					base [i].MaxDistance = value;
			}
		}

		public float MaxGain {
			set {
				for (int i = 0; i < Count; ++i)
					base [i].MaxGain = value;
			}
		}

		public float MinGain {
			set {
				for (int i = 0; i < Count; ++i)
					base [i].MinGain = value;
			}
		}

		public float Pitch {
			set {
				for (int i = 0; i < Count; ++i)
					base [i].Pitch = value;
			}
		}

		public float ReferenceDistance {
			set {
				for (int i = 0; i < Count; ++i)
					base [i].ReferenceDistance = value;
			}
		}

		public float RolloffFactor {
			set {
				for (int i = 0; i < Count; ++i)
					base [i].RolloffFactor = value;
			}
		}

		public void EnqueueBuffer (AudioBuffer buffer)
		{
			if (Count == 0)
				return;
			
			int id = AudioBuffer.GetID (buffer);
			for (int i = 0; i < Count; ++i) 
				base [i].EnqueueBufferInternal (id, buffer);
		}

		public void EnqueueBufferRange (IList<AudioBuffer> buffers)
		{
			if (buffers == null)
				throw new ArgumentNullException ("buffers");
			
			int bufferCount = buffers.Count;
			if (bufferCount == 0 || Count == 0)
				return;
			
			int[] ids = new int[bufferCount];
			OpenAlIDResource.GetIDs<AudioBuffer> (buffers, ids, AudioBuffer.NullBufferID);
			for (int i = 0; i < Count; ++i)
				base [i].EnqueueBufferRangeInternal (ids, buffers);
		}

		private AudioSourceStateEnum[] GetStates (int index, int count)
		{
			AudioSourceStateEnum[] rv = new AudioSourceStateEnum[count];
			for (int i = 0; i < count; i++, index++) 
				rv [i] = base [index].SourceState;
			
			return rv;
		}

		private AudioSourceStateEnum[] GetStates ()
		{
			AudioSourceStateEnum[] rv = new AudioSourceStateEnum[Count];
			for (int i = 0; i < Count; i++) 
				rv [i] = base [i].SourceState;
			
			return rv;
		}

		public void PauseRange (int index, int count)
		{
			if ((index < 0) || (count < 0))
				throw new ArgumentOutOfRangeException ((index < 0) ? ("index") : ("count"), "The value must be greater or equal to 0.");
			
			if ((Count - index) < count) 
				throw new ArgumentException ("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
			
			if (Count == 0)
				return;
			
			AudioSourceStateEnum[] states = GetStates (index, count);
			Al.alSourcePausev (count, ref  IDs [index]);
			OpenAlException.CheckAl ();
			for (int i = 0; i < count; i++, index++)
				base [index].OnPaused (states [i]);
		}

		public void Pause ()
		{
			if (Count == 0)
				return;
			
			AudioSourceStateEnum[] states = GetStates ();
			Al.alSourcePausev (Count, IDs);
			OpenAlException.CheckAl ();
			for (int i = 0; i < Count; i++)
				base [i].OnPaused (states [i]);
		}

		public void PlayRange (int index, int count)
		{
			if ((index < 0) || (count < 0)) 
				throw new ArgumentOutOfRangeException ((index < 0) ? ("index") : ("count"), "The value must be greater or equal to 0.");
			
			if ((Count - index) < count) 
				throw new ArgumentException ("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
			
			if (Count == 0)
				return;
			
			AudioSourceStateEnum[] states = GetStates (index, count);
			Al.alSourcePlayv (count, ref  IDs [index]);
			OpenAlException.CheckAl ();
			for (int i = 0; i < count; i++, index++)
				base [index].OnPlaying (states [i]);
		}

		public void Play ()
		{
			if (Count == 0)
				return;
			
			AudioSourceStateEnum[] states = GetStates ();
			Al.alSourcePlayv (Count, IDs);
			OpenAlException.CheckAl ();
			for (int i = 0; i < Count; i++)
				base [i].OnPlaying (states [i]);
		}

		public void RewindRange (int index, int count)
		{
			if ((index < 0) || (count < 0)) 
				throw new ArgumentOutOfRangeException ((index < 0) ? ("index") : ("count"), "The value must be greater or equal to 0.");
			
			if ((Count - index) < count) 
				throw new ArgumentException ("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
			
			if (Count == 0)
				return;
			
			AudioSourceStateEnum[] states = GetStates (index, count);
			Al.alSourceRewindv (count, ref  IDs [index]);
			OpenAlException.CheckAl ();
			for (int i = 0; i < count; i++, index++) 
				base [index].OnRewound (states [i]);
		}

		public void Rewind ()
		{
			if (Count == 0)
				return;
			
			AudioSourceStateEnum[] states = GetStates ();
			Al.alSourceRewindv (Count, IDs);
			OpenAlException.CheckAl ();
			for (int i = 0; i < Count; i++) 
				base [i].OnRewound (states [i]);
		}

		public void StopRange (int index, int count)
		{
			if ((index < 0) || (count < 0)) 
				throw new ArgumentOutOfRangeException ((index < 0) ? ("index") : ("count"), "The value must be greater or equal to 0.");
			
			if ((Count - index) < count) 
				throw new ArgumentException ("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
			
			if (Count == 0)
				return;
			
			AudioSourceStateEnum[] states = GetStates (index, count);
			Al.alSourceStopv (count, ref  IDs [index]);
			OpenAlException.CheckAl ();
			OpenAlException.CheckAl ();
			for (int i = 0; i < count; i++, index++) 
				base [index].OnStopped (states [i]);
		}

		public void Stop ()
		{
			if (Count == 0)
				return;
			
			AudioSourceStateEnum[] states = GetStates ();
			Al.alSourceStopv (Count, IDs);
			OpenAlException.CheckAl ();
			for (int i = 0; i < Count; i++)
				base [i].OnStopped (states [i]);
		}

		protected sealed override int NullID {
			get { return -1; }
		}

		protected sealed override void Release (int count, ref int id)
		{
			Al.alDeleteSources (count, ref id);
		}
	}
}