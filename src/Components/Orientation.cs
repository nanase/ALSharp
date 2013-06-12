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

using System.Runtime.InteropServices;

namespace ALSharp.Components
{
    /// <summary>
    /// AL_ORIENTATION is a pair of 3-tuples consisting of an 'at' vector and an 'up' vector, 
    /// where the 'at' vector represents the 'forward' direction of the listener and the orthogonal 
    /// projection of the 'up' vector into the subspace perpendicular to the 'at' vector represents 
    /// the 'up' direction for the listener. OpenAL expects two vectors that are linearly 
    /// independent. These vectors are not expected to be normalized. If the two vectors are
    /// linearly dependent, behavior is undefined.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 0, Size = Orientation.Size)]
    public struct Orientation
    {
        public const int Size = 24;
        /// <summary>
        /// Who you looking at? find out Here!
        /// </summary>
        public Vector3D At;
        /// <summary>
        /// Which way is Up? this one Knows! ok so i need better comments. 
        /// </summary>
        public Vector3D Up;

        public Orientation(Vector3D At, Vector3D Up)
        {
            this.Up = Up;
            this.At = At;
        }
        
		/*public static Orientation operator *(Matrix4x4 matrix, Orientation orientation)
        {
            Orientation rv;
            rv.At = matrix * orientation.At;
            rv.Up = matrix * orientation.Up;
            return rv;
        }
        public static Orientation operator *(Matrix3x3 matrix, Orientation orientation)
        {
            Orientation rv;
            rv.At = matrix * orientation.At;
            rv.Up = matrix * orientation.Up;
            return rv;
        }*/
    }
}