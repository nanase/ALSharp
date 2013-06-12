using System;

namespace ALSharp.Utils
{
	unsafe public static class ArrayConverter
	{
		#region Double
		public static void DoubleToBytesAsInt16(double[] from, byte[] to)
		{
			DoubleToBytesAsInt16(from, to, 0, from.Length, BitConverter.IsLittleEndian);
		}

		public static void DoubleToBytesAsInt16(double[] from, byte[] to, int offset, int count)
		{
			DoubleToBytesAsInt16(from, to, offset, count, BitConverter.IsLittleEndian);
		}

		public static void DoubleToBytesAsInt16(double[] from, byte[] to, int offset, int count, bool littleEndian)
		{
			if (from == null)
				throw new ArgumentNullException("from");
			if (to == null)
				throw new ArgumentNullException("to");
			if (from.Length > offset + count || to.Length < from.Length * 2)
				throw new ArgumentOutOfRangeException("from");

			short s;
			const double factor = Int16.MaxValue - 1.0;
			double d;
			byte* b0 = (byte*)&s, b1 = b0 + 1;

			if (BitConverter.IsLittleEndian ^ littleEndian)
				for (int i = offset; i < count; i++)
				{
					d = from[i];
					if (d > 1.0)
						d = 1.0;
					else if (d < -1.0)
						d = -1.0;
					s = (short)(d * factor);
					to[i * 2] = *b1;
					to[i * 2 + 1] = *b0;
				}
			else
				for (int i = offset; i < count; i++)
				{
					d = from[i];
					if (d > 1.0)
						d = 1.0;
					else if (d < -1.0)
						d = -1.0;
					s = (short)(d * factor);
					to[i * 2] = *b0;
					to[i * 2 + 1] = *b1;
				}
		}

		public static void DoubleToBytesAsInt8(double[] from, byte[] to)
		{
			DoubleToBytesAsInt8(from, to, 0, from.Length);
		}

		public static void DoubleToBytesAsInt8(double[] from, byte[] to, int offset, int count)
		{
			if (from == null)
				throw new ArgumentNullException("from");
			if (to == null)
				throw new ArgumentNullException("to");
			if (from.Length < offset + count || to.Length < from.Length)
				throw new ArgumentOutOfRangeException("from");

			const double factor = 127.0;
			double d;

			for (int i = offset; i < count; i++)
			{
				d = from[i];
				if (d > 1.0)
					d = 1.0;
				else if (d < -1.0)
					d = -1.0;

				to[i] = (byte)(d * factor + 127.0);
			}
		}

		#endregion

		#region Single
		public static void SingleToBytesAsInt16(float[] from, byte[] to)
		{
			SingleToBytesAsInt16(from, to, 0, from.Length, BitConverter.IsLittleEndian);
		}

		public static void SingleToBytesAsInt16(float[] from, byte[] to, int offset, int count)
		{
			SingleToBytesAsInt16(from, to, offset, count, BitConverter.IsLittleEndian);
		}

		public static void SingleToBytesAsInt16(float[] from, byte[] to, int offset, int count, bool littleEndian)
		{
			if (from == null)
				throw new ArgumentNullException("from");
			if (to == null)
				throw new ArgumentNullException("to");
			if (from.Length > offset + count || to.Length < from.Length * 2)
				throw new ArgumentOutOfRangeException("from");

			short s;
			const float factor = Int16.MaxValue - 1.0f;
			float d;
			byte* b0 = (byte*)&s, b1 = b0 + 1;

			if (BitConverter.IsLittleEndian ^ littleEndian)
				for (int i = offset; i < count; i++)
				{
					d = from[i];
					if (d > 1.0f)
						d = 1.0f;
					else if (d < -1.0f)
						d = -1.0f;
					s = (short)(d * factor);
					to[i * 2] = *b1;
					to[i * 2 + 1] = *b0;
				}
			else
				for (int i = offset; i < count; i++)
				{
					d = from[i];
					if (d > 1.0f)
						d = 1.0f;
					else if (d < -1.0f)
						d = -1.0f;
					s = (short)(d * factor);
					to[i * 2] = *b0;
					to[i * 2 + 1] = *b1;
				}
		}

		public static void SingleToBytesAsInt8(float[] from, byte[] to)
		{
			SingleToBytesAsInt8(from, to, 0, from.Length);
		}

		public static void SingleToBytesAsInt8(float[] from, byte[] to, int offset, int count)
		{
			if (from == null)
				throw new ArgumentNullException("from");
			if (to == null)
				throw new ArgumentNullException("to");
			if (from.Length < offset + count || to.Length < from.Length)
				throw new ArgumentOutOfRangeException("from");

			const float factor = 127.0f;
			float d;

			for (int i = offset; i < count; i++)
			{
				d = from[i];
				if (d > 1.0f)
					d = 1.0f;
				else if (d < -1.0f)
					d = -1.0f;

				to[i] = (byte)(d * factor + 127.0f);
			}
		}

		#endregion

		#region Byte
		public static void BytesToDoubleAsInt8(byte[] input, double[] output, bool stereo)
		{
			BytesToDoubleAsInt8(input, output, stereo, 0, output.Length);
		}

		public static void BytesToDoubleAsInt8(byte[] input, double[] output, bool stereo, int offset, int count)
		{
			if (stereo)
				for (int i = offset; i < count; i++)
					output[i] = (input[i * 2] + input[i * 2 + 1]) / 256.0 - 1.0;
			else
				for (int i = offset; i < count; i++)
					output[i] = input[i] / 128.0 - 1.0;
		}

		public static void BytesToDoubleAsInt16(byte[] input, double[] output, bool stereo)
		{
			BytesToDoubleAsInt16(input, output, stereo, 0, output.Length);
		}

		public static void BytesToDoubleAsInt16(byte[] input, double[] output, bool stereo, int offset, int count)
		{
			if (stereo)
			{
				short sl, sr;
				byte* b0 = (byte*)&sl, b1 = b0 + 1, b2 = (byte*)&sr, b3 = b2 + 1;
				for (int i = offset; i < count / 2; i++)
				{
					*
				b0 = input[i * 4];
					*
				b1 = input[i * 4 + 1];
					*
				b2 = input[i * 4 + 2];
					*
				b3 = input[i * 4 + 3];
					output[i * 2] = sl / (double)Int16.MaxValue;
					output[i * 2 + 1] = sr / (double)Int16.MaxValue;
				}
			}
			else
			{
				short s;
				byte* b0 = (byte*)&s, b1 = b0 + 1;
				for (int i = offset; i < count; i++)
				{
					*
					b0 = input[i * 2];
					*
					b1 = input[i * 2 + 1];
					output[i] = s / (double)Int16.MaxValue;
				}
			}
		}
		#endregion

		public static void SplitChannel (double[] source, double[] lch, double[] rch)
		{
			for (int i = 0; i < source.Length / 2; i++)
			{
				lch [i] = source [i * 2];
				rch [i] = source [i * 2 + 1];
			}
		}

		public static void JoinChannel (double[] lch, double[] rch, double[] dest)
		{
			for (int i = 0; i < dest.Length; i++)
			{
				if ((i & 1) == 0)
					dest [i] = lch [i / 2];
				else
					dest [i] = rch [i / 2];
			}
		}
	}
}
