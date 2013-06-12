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
	public enum WaveFormEnum : int
	{
		Sine = Alut.ALUT_WAVEFORM_SINE,
		Square = Alut.ALUT_WAVEFORM_SQUARE,
		SawTooth = Alut.ALUT_WAVEFORM_SAWTOOTH,
		WhiteNoise = Alut.ALUT_WAVEFORM_WHITENOISE,
		Impulse = Alut.ALUT_WAVEFORM_IMPULSE,
	}

	public enum AudioFormatEnum : int
	{
		Mono16 = Al.AL_FORMAT_MONO16,
		Mono8 = Al.AL_FORMAT_MONO8,
		Stereo16 = Al.AL_FORMAT_STEREO16,
		Stereo8 = Al.AL_FORMAT_STEREO8,
	}

	public enum AudioSourceStateEnum : int
	{
		Initial = Al.AL_INITIAL,
		Playing = Al.AL_PLAYING,
		Paused = Al.AL_PAUSED,
		Stopped = Al.AL_STOPPED,
	}

	public enum AudioSourceTypeEnum : int
	{
		Undetermined = Al.AL_UNDETERMINED,
		Static = Al.AL_STATIC,
		Streaming = Al.AL_STREAMING
	}

	public enum DistanceModelEnum : int
	{
		None = Al.AL_NONE,

		InverseDistance = Al.AL_INVERSE_DISTANCE,
		InverseDistanceClamped = Al.AL_INVERSE_DISTANCE_CLAMPED,

		LinearDistance = Al.AL_LINEAR_DISTANCE,
		LinearDistanceClamped = Al.AL_LINEAR_DISTANCE_CLAMPED,

		ExponentDistance = Al.AL_EXPONENT_DISTANCE,
		ExponentDistanceClamped = Al.AL_EXPONENT_DISTANCE_CLAMPED,
	}
}