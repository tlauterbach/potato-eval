namespace PotatoEval {

	internal class PatternString : IPattern {

		public bool Matches(ICharStream stream) {
			return stream.Peek() == '"';
		}

		public Token ToToken(ICharStream stream, out int length) {
			length = 1;
			while (!stream.IsEndOfStream(length)) {
				if (stream.Peek(length) == '"' && stream.Peek(length-1) != '\\') {
					break;
				} else {
					length++;
				}
			}
			length += 1;
			return new Token(TokenType.String, stream.Slice(length));
		}
	}

}