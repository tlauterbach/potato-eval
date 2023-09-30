namespace PotatoEval {

	internal static class Util {

		/// <summary>
		/// Generates a hash value from a string input
		/// using the Fowler-Noll-Vol algorithm from
		/// http://www.isthe.com/chongo/tech/comp/fnv/
		/// </summary>
		/// <param name="input">string used for hash</param>
		/// <returns>hash value of the input string</returns>
		internal static uint FNVHash32(string input) {
			if (string.IsNullOrEmpty(input)) {
				return 0;
			}
			uint hash = 2166136261;
			for (int ix = 0; ix < input.Length; ix++) {
				hash ^= input[ix];
				hash *= 16777619;
			}
			return hash;
		}

		/// <summary>
		/// Generates a hash value from a string input
		/// using the Fowler-Noll-Vol algorithm from
		/// http://www.isthe.com/chongo/tech/comp/fnv/
		/// </summary>
		/// <param name="input">string used for hash</param>
		/// <returns>hash value of the input string</returns>
		internal static uint FNVHash32(StringSlice input) {
			if (input == StringSlice.Empty) {
				return 0;
			}
			uint hash = 2166136261;
			for (int ix = 0; ix < input.Length; ix++) {
				hash ^= input[ix];
				hash *= 16777619;
			}
			return hash;
		}

	}

}