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

namespace ALSharp.Streams
{
    /// <summary>
    /// Plays a AudioStream using multiple buffers to ensure a countinous playback.
    /// </summary>
    public class AudioStreamPlayer : IDisposable
    {
        bool playing;

        bool disposed;
        AudioSource source;
        AudioStream stream;
        AudioBuffer[] buffers;
        Queue<AudioBuffer> emptyBuffers;
        Byte[] buffer;

        public AudioStreamPlayer(AudioStream audioStream, AudioSource audioSource, int buffersize, int buffercount)
        {
            if (audioStream == null)
            {
                throw new ArgumentNullException("audioStream");
            }
            if (audioSource == null)
            {
                throw new ArgumentNullException("audioSource");
            }
            if (buffersize <= 0)
            {
                throw new ArgumentOutOfRangeException("buffersize", "the buffer's size must be greater then 0");
            }
            if (buffercount <= 0)
            {
                throw new ArgumentOutOfRangeException("buffercount", "the buffercount must be greater then 0");
            }
            if (Environment.OSVersion.Platform != PlatformID.Unix && !audioSource.IsValid)
            {
                throw new ArgumentException("The audioSource must be a valid source", "audioSource");
            }

            this.emptyBuffers = new Queue<AudioBuffer>();
            this.stream = audioStream;
            this.source = audioSource;
            this.buffer = new byte[buffersize];
            this.source.Stop();
            OpenAlException.CheckAl();
            buffers = AudioBuffer.CreateBuffers(buffercount);
            for (int pos = 0; pos < buffercount; ++pos)
            {
                FillBuffer(buffers[pos]);
            }
            audioSource.EnqueueBufferRange(buffers);
        }

        public AudioSource Source
        {
            get { return source; }
        }

        public long Position
        {
            get
            {
                if (!stream.CanSeek)
                {
                    throw new NotSupportedException("The stream does not support Seeking");
                }
                return stream.Position;
            }
            set
            {
                if (!stream.CanSeek)
                {
                    throw new NotSupportedException("The stream does not support Seeking");
                }
                stream.Position = value;
                Reset();
            }
        }

        public TimeSpan TimePosition
        {
            get
            {
                if (!stream.CanSeekTime)
                {
                    throw new NotSupportedException("The stream does not support Seeking Time");
                }
                return stream.TimePosition;
            }
            set
            {
                if (!stream.CanSeekTime)
                {
                    throw new NotSupportedException("The stream does not support Seeking Time");
                }
                stream.TimePosition = value;
                Reset();
            }
        }

        public int TrackPosition
        {
            get
            {
                if (!stream.CanSeekTrack)
                {
                    throw new NotSupportedException("The stream does not support Seeking to a Track");
                }
                return stream.TrackPosition;
            }
            set
            {
                if (!stream.CanSeekTrack)
                {
                    throw new NotSupportedException("The stream does not support Seeking to a Track");
                }
                stream.TrackPosition = value;
                Reset();
            }
        }

        private void Reset()
        {
            source.Stop();
            AudioBuffer[] buffers = source.DequeueBufferRange(source.BuffersQueued);
            for (int pos = 0; pos < buffers.Length; ++pos)
            {
                FillBuffer(buffers[pos]);
            }
            source.EnqueueBufferRange(buffers);
            source.Play();
        }

        private int FillBuffer(AudioBuffer audioBuffer)
        {
            int bytes;
            int totalBytes = 0;
            while (totalBytes < buffer.Length && (bytes = stream.Read(buffer, totalBytes, buffer.Length - totalBytes)) > 0)
            {
                totalBytes += bytes;
            }
            audioBuffer.BufferData(stream.Format, stream.Frequency, buffer, totalBytes);
            return totalBytes;
        }

        public bool Playing
        {
            get { return playing; }
        }

        public void Update()
        {
            int filled = 0;
            int processed = source.BuffersProcessed;
            while (processed-- > 0)
            {
                AudioBuffer buff = source.DequeueBuffer();
                FillBuffer(buff);
                if (buff.Size > 0)
                {
                    filled++;
                    source.EnqueueBuffer(buff);
                }
                else
                {
                    emptyBuffers.Enqueue(buff);
                    break;
                }
            }
            if (filled > 0)
            {

                while (emptyBuffers.Count > 0)
                {
                    AudioBuffer buff = emptyBuffers.Dequeue();
                    FillBuffer(buff);
                    if (buff.Size > 0)
                    {
                        filled++;
                        source.EnqueueBuffer(buff);
                    }
                    else
                    {
                        emptyBuffers.Enqueue(buff);
                    }
                }

                if (playing &&
                    source.SourceState != AudioSourceStateEnum.Playing)
                {
                    source.Play();
                }
            }
        }

        public void Play()
        {
            source.Play();
            playing = true;
        }

        public void Pause()
        {
            source.Pause();
            playing = false;
        }

        public void Stop()
        {
            source.Stop();
            playing = false;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!disposed)
                {
                    if (source.IsValid)
                    {
                        source.Clear();
                    }

                    AudioBuffer.ReleaseBuffers(buffers);
                    stream.Close();
                    stream = null;
                    buffer = null;
                    disposed = true;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}