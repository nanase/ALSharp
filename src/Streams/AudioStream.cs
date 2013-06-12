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
using System.IO;
using ALSharp.Components;

namespace ALSharp.Streams
{
    /// <summary>
    /// A base class for wrapping streams from audio files.
    /// </summary>
    public abstract class AudioStream : Stream
    {
        public abstract AudioFormatEnum Format { get;}
        public abstract int Frequency { get;}

        public abstract bool CanSeekTime { get;}
        public abstract TimeSpan TimePosition { get;set;}
        public abstract TimeSpan TimeSeek(TimeSpan offset, SeekOrigin origin);
        public abstract TimeSpan TimeLength { get;}

        public abstract bool CanSeekTrack { get;}
        public abstract int TrackPosition { get;set;}
        public abstract int TrackSeek(int offset, SeekOrigin origin);
        public abstract int TrackCount { get;}

        public Byte[] ReadAll()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                WriteTo(stream);
                return stream.ToArray();
            }
        }
        public void WriteTo(Stream stream)
        {
            if (stream == null) { throw new ArgumentNullException("stream"); }
            if (!stream.CanWrite) { throw new ArgumentException("stream"); }
            if (!CanSeek || !CanRead) { throw new InvalidOperationException(); }
            Seek(0, SeekOrigin.Begin);
            Byte[] buffer = new Byte[0x1000];
            int bytesRead;
            while ((bytesRead = Read(buffer, 0, buffer.Length)) > 0)
            {
                stream.Write(buffer, 0, bytesRead);
            }
        }
    }

}