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
using System.Threading;
using ALSharp.Bind;

namespace ALSharp.Components
{
    public sealed class AudioSource : OpenAlIDResource
    {
        #region static
        public static AudioSource[] CreateSources(int count)
        {
            int[] ids = new int[count];
            Al.alGenSources(count, ids);
            OpenAlException.CheckAl();
            AudioSource[] rv = new AudioSource[count];
            AudioContext context = AudioContext.CurrentContext;
            for (int pos = 0; pos < count; ++pos)
            {
                rv[pos] = new AudioSource(ids[pos], context);
            }
            return rv;
        }
        public static void ReleaseSources(IList<AudioSource> sources)
        {
            if (sources == null)
            {
                throw new ArgumentNullException("buffers");
            }
            int length = sources.Count;
            if (length > 0)
            {
                int[] sourceIds = new int[length];
                for (int pos = 0; pos < length; ++pos)
                {
                    sourceIds[pos] = sources[pos].ID;
                }
                Al.alDeleteSources(length, sourceIds);
                OpenAlException.CheckAl();
                for (int pos = 0; pos < length; ++pos)
                {
                    sources[pos].Dispose(true, false);
                }
            }
        }
        #endregion

        #region Events
        public event EventHandler Disposed;
        public event EventHandler Rewound;
        public event EventHandler Playing;
        public event EventHandler Paused;
        private event EventHandler StoppedInternal;
        public event EventHandler Stopped
        {
            add
            {
                StoppedInternal += value;
                CheckStoppedEvent();
            }
            remove
            {
                StoppedInternal -= value;
                CheckStoppedEvent();
            }
        }

        #endregion

        #region fields

        LinkedList<AudioBuffer> queue = new LinkedList<AudioBuffer>();
        AudioContext context;

        bool isListenerWaiting;
        AutoResetEvent waitHandle;
        ReaderWriterLock queueLock = new ReaderWriterLock();

        #endregion

        #region constructors

        public AudioSource()
        {
            Al.alGenSources(1, out ID);
            OpenAlException.CheckAl();
            this.context = AudioContext.CurrentContext;
            this.context.Disposing += OnContextDisposing;
        }
        internal AudioSource(int iD, AudioContext context)
        {
            this.ID = iD;
            this.context = context;
            this.context.Disposing += OnContextDisposing;
        }

        #endregion

        #region properties
        /// <summary>
        /// Gets the AudioContext this source was created under.
        /// </summary>
        public AudioContext Context
        {
            get { return context; }
        }
        /// <summary>
        /// Gets or sets the direction of this source.
        /// </summary>
        public Vector3D Direction
        {
            get
            {
                Vector3D rv;
                Al.alGetSource3f(ID, Al.AL_DIRECTION, out rv.X, out rv.Y, out rv.Z);
                OpenAlException.CheckAl();
                return rv;
            }
            set
            {
                Al.alSource3f(ID, Al.AL_DIRECTION, value.X, value.Y, value.Z);
                OpenAlException.CheckAl();
            }
        }
        /// <summary>
        /// Gets or sets Position.
        /// specifies the current location of the object in the world coordinate system. 
        /// Any 3-tuple of valid float values is allowed. 
        /// Implementation behavior on encountering NaN and infinity is not defined. 
        /// The object position is always defined in the world coordinate system.
        /// </summary>
        public Vector3D Position
        {
            get
            {
                Vector3D rv;
                Al.alGetSource3f(ID, Al.AL_POSITION, out rv.X, out rv.Y, out rv.Z);
                OpenAlException.CheckAl();
                return rv;
            }
            set
            {
                Al.alSource3f(ID, Al.AL_POSITION, value.X, value.Y, value.Z);
                OpenAlException.CheckAl();
            }

        }
        /// <summary>
        /// Gets or sets the Velocity.
        /// specifies the current velocity (speed and direction) of the object, in the world coordinate system. 
        /// Any 3-tuple of valid float/double values is allowed. 
        /// The object AL_VELOCITY does not affect the source's position. 
        /// OpenAL does not calculate the velocity from subsequent position updates, nor does it adjust the position over time based on the specified velocity. 
        /// Any such calculation is left to the application. 
        /// For the purposes of sound processing, position and velocity are independent parameters affecting different aspects of the sounds. 
        /// </summary>
        public Vector3D Velocity
        {
            get
            {
                Vector3D rv;
                Al.alGetSource3f(ID, Al.AL_VELOCITY, out rv.X, out rv.Y, out rv.Z);
                OpenAlException.CheckAl();
                return rv;
            }
            set
            {
                Al.alSource3f(ID, Al.AL_VELOCITY, value.X, value.Y, value.Z);
                OpenAlException.CheckAl();
            }

        }

        protected override bool IsIDValid
        {
            get
            {
                return Al.alIsSource(ID) == Al.AL_TRUE;
            }
        }
        /// <summary>
        /// Gets if the Source while restart once its processed all its buffers
        /// </summary>
        public bool IsLooping
        {
            get
            {
                int rv;
                Al.alGetSourcei(ID, Al.AL_LOOPING, out rv);
                OpenAlException.CheckAl();
                return rv == Al.AL_TRUE;
            }
            set
            {
                Al.alSourcei(ID, Al.AL_LOOPING, (value) ? (Al.AL_TRUE) : (Al.AL_FALSE));
                OpenAlException.CheckAl();
            }
        }
        /// <summary>
        /// set to true indicates that the position, velocity, cone, and direction properties of a source are to be interpreted relative to the listener position.
        /// </summary>
        public bool IsRelative
        {
            get
            {
                int rv;
                Al.alGetSourcei(ID, Al.AL_SOURCE_RELATIVE, out rv);
                OpenAlException.CheckAl();
                return rv == Al.AL_TRUE;
            }
            set
            {
                Al.alSourcei(ID, Al.AL_SOURCE_RELATIVE, (value) ? (Al.AL_TRUE) : (Al.AL_FALSE));
                OpenAlException.CheckAl();
            }
        }
        /// <summary>
        /// the rolloff rate for the source default is 1.0
        /// </summary>
        public float RolloffFactor
        {
            get
            {
                float rv;
                Al.alGetSourcef(ID, Al.AL_ROLLOFF_FACTOR, out rv);
                OpenAlException.CheckAl();
                return rv;
            }
            set
            {
                Al.alSourcef(ID, Al.AL_ROLLOFF_FACTOR, value);
                OpenAlException.CheckAl();
            }
        }
        /// <summary>
        /// Gets or Sets the pitch multiplier, this must be positive.
        /// </summary>
        public float Pitch
        {
            get
            {
                float rv;
                Al.alGetSourcef(ID, Al.AL_PITCH, out rv);
                OpenAlException.CheckAl();
                return rv;
            }
            set
            {
                Al.alSourcef(ID, Al.AL_PITCH, value);
                OpenAlException.CheckAl();
            }
        }

        /// <summary>
        /// gets and sets a scalar amplitude multiplier. As a source attribute, it applies to that particular source only. 
        /// As a listener attribute, it effectively applies to all sources in the current context. 
        /// The default 1.0 means that the sound is unattenuated. 
        /// An AL_GAIN value of 0.5 is equivalent to an attenuation of 6 dB. 
        /// The value zero equals silence (no contribution to the output mix). 
        /// Driver implementations are free to optimize this case and skip mixing and processing stages where applicable. 
        /// The implementation is in charge of ensuring artifact-free (click-free) changes of gain values and is free to defer actual modification of the sound samples, within the limits of acceptable latencies.
        /// </summary>
        public float Gain
        {
            get
            {
                float rv;
                Al.alGetSourcef(ID, Al.AL_GAIN, out rv);
                OpenAlException.CheckAl();
                return rv;
            }
            set
            {
                Al.alSourcef(ID, Al.AL_GAIN, value);
                OpenAlException.CheckAl();
            }
        }
        /// <summary>
        /// Gets or Sets the maximum gain for this source
        /// </summary>
        public float MaxGain
        {
            get
            {
                float rv;
                Al.alGetSourcef(ID, Al.AL_MAX_GAIN, out rv);
                OpenAlException.CheckAl();
                return rv;
            }
            set
            {
                Al.alSourcef(ID, Al.AL_MAX_GAIN, value);
                OpenAlException.CheckAl();
            }
        }
        /// <summary>
        /// Gets or Sets the minimum gain for this source
        /// </summary>
        public float MinGain
        {
            get
            {
                float rv;
                Al.alGetSourcef(ID, Al.AL_MIN_GAIN, out rv);
                OpenAlException.CheckAl();
                return rv;
            }
            set
            {
                Al.alSourcef(ID, Al.AL_MIN_GAIN, value);
                OpenAlException.CheckAl();
            }
        }

        /// <summary>
        /// Gets or Sets  the MaxDistance.   
        ///used with the Inverse Clamped Distance Model
        ///to set the distance where there will no longer be
        ///any attenuation of the source
        /// </summary>
        public float MaxDistance
        {
            get
            {
                float rv;
                Al.alGetSourcef(ID, Al.AL_MAX_DISTANCE, out rv);
                OpenAlException.CheckAl();
                return rv;
            }
            set
            {
                Al.alSourcef(ID, Al.AL_MAX_DISTANCE, value);
                OpenAlException.CheckAl();
            }
        }
        /// <summary>
        ///Gets or Sets the distance under which the volume for the
        ///source would normally drop by half (before
        ///being influenced by rolloff factor or AL_MAX_DISTANCE)
        /// </summary>
        public float ReferenceDistance
        {
            get
            {
                float rv;
                Al.alGetSourcef(ID, Al.AL_REFERENCE_DISTANCE, out rv);
                OpenAlException.CheckAl();
                return rv;
            }
            set
            {
                Al.alSourcef(ID, Al.AL_REFERENCE_DISTANCE, value);
                OpenAlException.CheckAl();
            }
        }
        /// <summary>
        /// Gets or Sets the gain in the inside cone
        /// </summary>
        public float ConeInnerAngle
        {
            get
            {
                float rv;
                Al.alGetSourcef(ID, Al.AL_CONE_INNER_ANGLE, out rv);
                OpenAlException.CheckAl();
                return rv;
            }
            set
            {
                Al.alSourcef(ID, Al.AL_CONE_INNER_ANGLE, value);
                OpenAlException.CheckAl();
            }
        }
        /// <summary>
        /// Gets or Sets the outer angle of the sound cone, in degrees default is 360
        /// </summary>
        public float ConeOuterAngle
        {
            get
            {
                float rv;
                Al.alGetSourcef(ID, Al.AL_CONE_OUTER_ANGLE, out rv);
                OpenAlException.CheckAl();
                return rv;
            }
            set
            {
                Al.alSourcef(ID, Al.AL_CONE_OUTER_ANGLE, value);
                OpenAlException.CheckAl();
            }
        }
        /// <summary>
        /// Gets or Sets the gain when outside the oriented cone
        /// </summary>
        public float ConeOuterGain
        {
            get
            {
                float rv;
                Al.alGetSourcef(ID, Al.AL_CONE_OUTER_GAIN, out rv);
                OpenAlException.CheckAl();
                return rv;
            }
            set
            {
                Al.alSourcef(ID, Al.AL_CONE_OUTER_GAIN, value);
                OpenAlException.CheckAl();
            }
        }

        /// <summary>
        /// Gets or Sets the playback Position in bytes
        /// </summary>
        public int BytesOffset
        {
            get
            {
                int rv;
                Al.alGetSourcei(ID, Al.AL_BYTE_OFFSET, out rv);
                OpenAlException.CheckAl();
                return rv;
            }
            set
            {
                Al.alSourcei(ID, Al.AL_BYTE_OFFSET, value);
                OpenAlException.CheckAl();
            }
        }
        /// <summary>
        /// Gets or Sets the playback Position in samples
        /// </summary>
        public int SamplesOffset
        {
            get
            {
                int rv;
                Al.alGetSourcei(ID, Al.AL_SAMPLE_OFFSET, out rv);
                OpenAlException.CheckAl();
                return rv;
            }
            set
            {
                Al.alSourcei(ID, Al.AL_SAMPLE_OFFSET, value);
                OpenAlException.CheckAl();
            }
        }
        /// <summary>
        /// Gets or Sets the playback Position in seconds
        /// </summary>
        public float SecondsOffset
        {
            get
            {
                float rv;
                Al.alGetSourcef(ID, Al.AL_SEC_OFFSET, out rv);
                OpenAlException.CheckAl();
                return rv;
            }
            set
            {
                Al.alSourcef(ID, Al.AL_SEC_OFFSET, value);
                OpenAlException.CheckAl();
            }
        }

        /// <summary>
        /// Gets the total number of Bytes in all the AudioBuffers in the queue
        /// </summary>
        public int BytesQueued
        {
            get
            {
                try
                {
                    queueLock.AcquireReaderLock(Timeout.Infinite);
                    int result = 0;
                    foreach (AudioBuffer buffer in queue)
                    {
                        result += buffer.Size;
                    }
                    return result;
                }
                finally
                {
                    queueLock.ReleaseReaderLock();
                }
            }
        }
        /// <summary>
        /// Gets the total number of Samples in all the AudioBuffers in the queue
        /// </summary>
        public int SamplesQueued
        {
            get
            {
                try
                {
                    queueLock.AcquireReaderLock(Timeout.Infinite);
                    int result = 0;
                    foreach (AudioBuffer buffer in queue)
                    {
                        result += buffer.Samples;
                    }
                    return result;
                }
                finally
                {
                    queueLock.ReleaseReaderLock();
                }
            }
        }
        /// <summary>
        /// Gets the total number of Seconds in all the AudioBuffers in the queue
        /// </summary>
        public float SecondsQueued
        {
            get
            {
                try
                {
                    queueLock.AcquireReaderLock(Timeout.Infinite);
                    float result = 0;
                    foreach (AudioBuffer buffer in queue)
                    {
                        result += buffer.Seconds;
                    }

                    return result;
                }
                finally
                {
                    queueLock.ReleaseReaderLock();
                }
            }
        }

        /// <summary>
        /// Gets the number of AudioBuffers queued on this source
        /// </summary>
        public int BuffersQueued
        {
            get
            {
                int[] rv = new int[1];
                Al.alGetSourcei(ID, Al.AL_BUFFERS_QUEUED, rv);
                OpenAlException.CheckAl();
                return rv[0];
            }
        }
        /// <summary>
        /// Gets the number of buffers in the queue that have been processed
        /// </summary>
        public int BuffersProcessed
        {
            get
            {
                int rv;
                Al.alGetSourcei(ID, Al.AL_BUFFERS_PROCESSED, out  rv);
                OpenAlException.CheckAl();
                return rv;
            }
        }
        /// <summary>
        /// Gets or Sets the current AudioBuffer.
        /// If setting it clears the buffer queue and adds the Buffer.
        /// If it is playing it throws an exception.
        /// </summary>
        public AudioBuffer CurrentBuffer
        {
            get
            {
                int curId = CurrentBufferID;
                if (curId == AudioBuffer.NullBufferID)
                {
                    return null;
                }
                foreach (AudioBuffer buffer in queue)
                {
                    if (buffer != null && buffer.ID == curId)
                    {
                        return buffer;
                    }
                }
                return null;
            }
            set
            {
                CurrentBufferID = AudioBuffer.GetID(value);
                queue.AddLast(value);
            }
        }
        private int CurrentBufferID
        {
            get
            {
                int rv;
                Al.alGetSourcei(ID, Al.AL_BUFFER, out rv);
                return rv;
            }
            set
            {

                try
                {
                    queueLock.AcquireWriterLock(Timeout.Infinite);
                    Al.alSourcei(ID, Al.AL_BUFFER, value);
                    OpenAlException.CheckAl();
                    queue.Clear();
                }
                finally
                {
                    queueLock.ReleaseWriterLock();
                }
            }
        }

        /// <summary>
        /// Gets the SourceType of the Source
        /// </summary>
        public AudioSourceTypeEnum SourceType
        {
            get
            {
                int rv;
                Al.alGetSourcei(ID, Al.AL_SOURCE_TYPE, out rv);
                OpenAlException.CheckAl();
                return (AudioSourceTypeEnum)rv;
            }
        }
        /// <summary>
        /// Gets the currentState of the source
        /// </summary>
        public AudioSourceStateEnum SourceState
        {
            get
            {
                int rv;
                Al.alGetSourcei(ID, Al.AL_SOURCE_STATE, out rv);
                OpenAlException.CheckAl();
                return (AudioSourceStateEnum)rv;
            }
        }


        #endregion

        #region methods
        /// <summary>
        /// Enqueues a Buffer for playing
        /// </summary>
        /// <param name="buffer">the buffer to be queued</param>
        public void EnqueueBuffer(AudioBuffer buffer)
        {
            EnqueueBufferInternal(AudioBuffer.GetID(buffer), buffer);
        }
        /// <summary>
        /// Enqueues a Range of buffers for playing
        /// </summary>
        /// <param name="buffers">the list of buffers to play</param>
        public void EnqueueBufferRange(IList<AudioBuffer> buffers)
        {
            if (buffers == null)
            {
                throw new ArgumentNullException("buffers");
            }
            if (buffers.Count > 0)
            {
                int[] ids = new int[buffers.Count];
                OpenAlIDResource.GetIDs<AudioBuffer>(buffers, ids, AudioBuffer.NullBufferID);
                EnqueueBufferRangeInternal(ids, buffers);
            }
        }

        internal void EnqueueBufferInternal(int id, AudioBuffer buffer)
        {
            try
            {
                queueLock.AcquireWriterLock(Timeout.Infinite);
                Al.alSourceQueueBuffers(ID, 1, ref id);
                OpenAlException.CheckAl();
                queue.AddLast(buffer);
            }
            finally
            {
                queueLock.ReleaseWriterLock();
            }
        }
        internal void EnqueueBufferRangeInternal(int[] ids, IList<AudioBuffer> buffers)
        {
            try
            {
                queueLock.AcquireWriterLock(Timeout.Infinite);
                int bufferCount = buffers.Count;
                Al.alSourceQueueBuffers(ID, bufferCount, ids);
                OpenAlException.CheckAl();
                for (int pos = 0; pos < bufferCount; ++pos)
                {
                    queue.AddLast(buffers[pos]);
                }
            }
            finally
            {
                queueLock.ReleaseWriterLock();
            }
        }
        /// <summary>
        /// Dequeues a single buffer from the queue. 
        /// </summary>
        /// <returns>A AudioBuffer</returns>
        public AudioBuffer DequeueBuffer()
        {
            try
            {
                queueLock.AcquireWriterLock(Timeout.Infinite);
                DequeueBuffersInternal(1);
                AudioBuffer rv = queue.First.Value;
                queue.RemoveFirst();
                return rv;
            }
            finally
            {
                queueLock.ReleaseWriterLock();
            }
        }
        /// <summary>
        /// Dequeues a Range of buffers 
        /// </summary>
        /// <param name="count">the number of buffers to dequeue</param>
        /// <returns>An array of AudioBuffers</returns>
        public AudioBuffer[] DequeueBufferRange(int count)
        {
            try
            {
                queueLock.AcquireWriterLock(Timeout.Infinite);
                DequeueBuffersInternal(count);
                AudioBuffer[] rv = new AudioBuffer[count];
                for (int pos = 0; pos < count; ++pos)
                {
                    rv[pos] = queue.First.Value;
                    queue.RemoveFirst();
                }
                return rv;
            }
            finally
            {
                queueLock.ReleaseWriterLock();
            }
        }
        int[] DequeueBuffersInternal(int count)
        {
            int[] ids = new int[count];
            Al.alSourceUnqueueBuffers(ID, ids.Length, ids);
            OpenAlException.CheckAl();
            return ids;
        }

        /// <summary>
        ///Plays the source by changing it's state to change to Playing. 
        ///When called on a source that is already playing, the source will restart at the beginning. When the attached
        ///buffer(s) are done playing, the source will progress to the Stopped state.
        /// </summary>
        public void Play()
        {
            AudioSourceStateEnum lastState = SourceState;
            Al.alSourcePlay(ID);
            OpenAlException.CheckAl();
            OnPlaying(lastState);
        }
        /// <summary>
        /// Pauses the source by changing it's state to change to Paused.
        /// </summary>
        public void Pause()
        {
            AudioSourceStateEnum lastState = SourceState;
            Al.alSourcePause(ID);
            OpenAlException.CheckAl();
            OnPaused(lastState);
        }
        /// <summary>
        /// Stops the source by changing it's state to change to Stopped.
        /// </summary>
        public void Stop()
        {
            AudioSourceStateEnum lastState = SourceState;
            Al.alSourceStop(ID);
            OpenAlException.CheckAl();
            OnStopped(lastState);
        }
        /// <summary>
        /// Rewinds the source by changing it's state to change to Initial.
        /// </summary>
        public void Rewind()
        {
            AudioSourceStateEnum lastState = SourceState;
            Al.alSourceRewind(ID);
            OpenAlException.CheckAl();
            OnRewound(lastState);
        }
        /// <summary>
        /// Stops the Source and Empties it's Queue
        /// </summary>
        public void Clear()
        {
            Stop();
            CurrentBufferID = AudioBuffer.NullBufferID;
        }

        void OnContextDisposing(object sender, EventArgs e)
        {
            if (context.AlutOwned)
            {
                this.Dispose(true, false);
            }
            else
            {
                this.Dispose();
            }
        }

        protected internal override void Dispose(bool disposing, bool releaseID)
        {
            if (IsValid)
            {
                if (releaseID)
                {
                    Al.alDeleteSources(1, ref ID);
                    OpenAlException.CheckAl();
                }
                try
                {
                    queueLock.AcquireWriterLock(Timeout.Infinite);
                    queue.Clear();
                }
                finally
                {
                    queueLock.ReleaseWriterLock();
                }
                SetInvalid();
                this.context.Disposing -= OnContextDisposing;

                if (disposing)
                {
                    lock (queueLock)
                    {
                        CloseWait();
                    }
                    queueLock = null;
                }
                OnDisposed();
            }
        }


        void CheckStoppedEvent()
        {
            lock (queueLock)
            {
                if (StoppedInternal != null ^ waitHandle != null)
                {
                    if (IsValid && waitHandle == null)
                    {
                        this.waitHandle = new AutoResetEvent(false);
                        RegisterWait();
                    }
                    else
                    {
                        CloseWait();
                    }
                }
            }
        }
        void CloseWait()
        {
            AutoResetEvent waitHandle = this.waitHandle;
            this.waitHandle = null;
            if (waitHandle != null)
            {
                waitHandle.Set();
                waitHandle.Close();
            }
        }
        void RegisterWait()
        {
            isListenerWaiting = 
                SourceState == AudioSourceStateEnum.Playing &&
                !IsLooping;
            if (isListenerWaiting)
            {
				int milliseconds = (int)Math.Ceiling((this.SecondsQueued - this.SecondsOffset) * 1000f);
                //int milliseconds = (int)MathHelper.Ceiling((this.SecondsQueued - this.SecondsOffset) * 1000);
                ThreadPool.RegisterWaitForSingleObject(waitHandle, OnPoll, null, milliseconds, true);
            }
        }

        void StartPolling()
        {
            lock (queueLock)
            {
                if (!isListenerWaiting && 
                    waitHandle != null &&
                    IsValid)
                {
                    RegisterWait();
                }
            }
        }
        void StopPolling()
        {
            lock (queueLock)
            {
                if (isListenerWaiting)
                {
                    waitHandle.Set();
                }
            }
        }
        void OnPoll(object state, bool timedOut)
        {
            lock (queueLock)
            {
                if (waitHandle == null || !IsValid)
                {
                    return;
                }
                RegisterWait();
                if (timedOut &&
                   this.SourceState == AudioSourceStateEnum.Stopped)
                {
                    OnStopped(AudioSourceStateEnum.Playing);
                }
            }
        }

        internal void OnPaused(AudioSourceStateEnum lastState)
        {
            if (lastState != AudioSourceStateEnum.Paused &&
                lastState != AudioSourceStateEnum.Initial)
            {
                StopPolling();
                if (Paused != null)
                {
                    Paused(this, EventArgs.Empty);
                }
                lastState = SourceState;
            }
        }
        internal void OnStopped(AudioSourceStateEnum lastState)
        {
            if (lastState != AudioSourceStateEnum.Stopped && 
                lastState != AudioSourceStateEnum.Initial)
            {
                StopPolling();
                if (StoppedInternal != null)
                {
                    StoppedInternal(this, EventArgs.Empty);
                }
            }
        }
        internal void OnRewound(AudioSourceStateEnum lastState)
        {
            if (lastState != AudioSourceStateEnum.Initial)
            {
                StopPolling();
                if (lastState == AudioSourceStateEnum.Playing)
                {
                    OnStopped(lastState);
                }
                if (Rewound != null)
                {
                    Rewound(this, EventArgs.Empty);
                }
            }
        }
        internal void OnPlaying(AudioSourceStateEnum lastState)
        {
            if (lastState != AudioSourceStateEnum.Playing)
            {
                StartPolling();
                if (Playing != null)
                {
                    Playing(this, EventArgs.Empty);
                }
            }
        }

        void OnDisposed()
        {
            if (Disposed != null)
            {
                Disposed(this, EventArgs.Empty);
            }
        }
        #endregion
    }
}