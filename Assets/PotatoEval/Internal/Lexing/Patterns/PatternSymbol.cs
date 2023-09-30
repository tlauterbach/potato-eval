namespace PotatoEval {

	internal class PatternSymbol : IPattern {

		private readonly TokenType m_type;
		private readonly string m_symbol;

		public PatternSymbol(TokenType type, string keyword) {
			m_type = type;
			m_symbol = keyword;
		}

		public bool Matches(ICharStream stream) {
			bool isEndOfStream = stream.IsEndOfStream(m_symbol.Length-1);
			if (!isEndOfStream) {
				StringSlice str = stream.Slice(m_symbol.Length);
				return str == m_symbol;
			} else {
				return false;
			}
		}
		public Token ToToken(ICharStream stream, out int length) {
			length = m_symbol.Length;
			return new Token(m_type, StringSlice.Empty);
		}
	}

}