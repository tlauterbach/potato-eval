namespace PotatoEval {
	internal class PatternKeyword : IPattern {

		private readonly TokenType m_type;
		private readonly string m_keyword;

		public PatternKeyword(TokenType type, string keyword) {
			m_type = type;
			m_keyword = keyword;
		}

		public bool Matches(ICharStream stream) {
			if (!stream.IsEndOfStream(m_keyword.Length-1) && stream.Slice(m_keyword.Length) == m_keyword) {
				// the token must also terminate
				if (stream.IsEndOfStream(m_keyword.Length)) {
					return true;
				}
				char next = stream.Peek(m_keyword.Length);
				return !char.IsLetterOrDigit(next) && next != '_';
			} else {
				return false;
			}
		}
		public Token ToToken(ICharStream stream, out int length) {
			length = m_keyword.Length;
			return new Token(m_type, StringSlice.Empty);
		}
	}

}