using System.Runtime.InteropServices;

namespace PotatoEval {

	[StructLayout(LayoutKind.Explicit, Size = 8)]
	internal struct Instruction {

		[FieldOffset(0)]
		public OpCode OpCode;
		[FieldOffset(4)]
		public uint Value;

		public Instruction(OpCode opCode, uint value) {
			OpCode = opCode;
			Value = value;
		}

		public static uint Encode(double value) {
			float single = (float)value;
			unsafe {
				return *(uint*)(&single);
			}
		}
		public static uint Encode(uint value) {
			return value;
		}
		public static uint Encode(bool value) {
			return (value ? 1u : 0u);
		}
		public static uint Encode(int value) {
			unsafe {
				return *(uint*)(&value);
			}
		}

		public static double DecodeDouble(uint value) {
			unsafe {
				return (double)*(float*)(&value);
			}
		}
		public static uint DecodeHash(uint value) {
			return value;
		}
		public static bool DecodeBool(uint value) {
			return value > 0;
		}
		public static int DecodeInt(uint value) {
			unsafe {
				return *(int*)(&value);
			}
		}


	}

}