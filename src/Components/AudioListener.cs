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
    public static class AudioListener
    {
        /// <summary>
        /// Gets and Sets a pair of 3-tuples consisting of an 'at' vector and an 'up' vector, where the 'at'
        /// vector represents the 'forward' direction of the listener and the
        /// orthogonal projection of the 'up' vector into the subspace 
        /// perpendicular to the 'at' vector represents the 'up' direction 
        /// for the listener. OpenAL expects two vectors that are linearly 
        /// independent. These vectors are not expected to be normalized. 
        /// If the two vectors are linearly dependent, behavior is undefined.
        /// </summary>
        public static Orientation Orientation
        {
            get
            {
                Orientation rv = new Orientation();
                Al.alGetListenerfv(Al.AL_ORIENTATION, out rv.At.X);
                OpenAlException.CheckAl();
                return rv;
            }
            set
            {
                Al.alListenerfv(Al.AL_ORIENTATION, ref value.At.X);
                OpenAlException.CheckAl();
            }
        }
        /// <summary>
        /// Gets and Sets a scalar amplitude multiplier. As a source attribute, it applies to that particular source only. 
        /// As a listener attribute, it effectively applies to all sources in the current context. 
        /// The default 1.0 means that the sound is unattenuated. 
        /// An AL_GAIN value of 0.5 is equivalent to an attenuation of 6 dB. 
        /// The value zero equals silence (no contribution to the output mix). 
        /// Driver implementations are free to optimize this case and skip mixing and processing stages where applicable. 
        /// The implementation is in charge of ensuring artifact-free (click-free) changes of gain values and is free to defer actual modification of the sound samples, within the limits of acceptable latencies.
        /// </summary>
        public static float Gain
        {
            get
            {
                float rv;
                Al.alGetListenerf(Al.AL_GAIN, out rv);
                OpenAlException.CheckAl();
                return rv;
            }
            set
            {
                Al.alListenerf(Al.AL_GAIN, value);
                OpenAlException.CheckAl();
            }
        }
        /// <summary>
        /// Gets and Sets the current velocity (speed and direction) of the object, in the world coordinate system.
        /// Any 3-tuple of valid float/double values is allowed. The object AL_VELOCITY does not affect the source's position. 
        /// OpenAL does not calculate the velocity from subsequent position updates, nor does it adjust the position over 
        /// time based on the specified velocity. Any such calculation is left to the application. For the purposes of sound
        /// processing, position and velocity are independent parameters affecting different aspects of the sounds. 
        /// </summary>
        public static Vector3D Velocity
        {
            get
            {
                Vector3D rv;
                Al.alGetListener3f(Al.AL_VELOCITY, out rv.X, out rv.Y, out rv.Z);
                OpenAlException.CheckAl();
                return rv;
            }
            set
            {
                Al.alListener3f(Al.AL_VELOCITY, value.X, value.Y, value.Z);
                OpenAlException.CheckAl();
            }

        }
        /// <summary>
        /// Gets and Sets the current location of the object in the world coordinate system. 
        /// Any 3-tuple of valid float values is allowed. 
        /// Implementation behavior on encountering NaN and infinity is not defined. 
        /// The object position is always defined in the world coordinate system.
        /// </summary>
        public static Vector3D Position
        {
            get
            {
                Vector3D rv;
                Al.alGetListener3f(Al.AL_POSITION, out rv.X, out rv.Y, out rv.Z);
                OpenAlException.CheckAl();
                return rv;
            }
            set
            {
                Al.alListener3f(Al.AL_POSITION, value.X, value.Y, value.Z);
                OpenAlException.CheckAl();
            }

        }
    }
}