namespace PotatoEval {

	internal class PatternIdentifier : IPattern {
		public bool Matches(ICharStream stream) {
			return char.IsLetter(stream.Peek()) || stream.Peek() == '_';
		}

		public Token ToToken(ICharStream stream, out int length) {
			length = 1;
			while (!stream.IsEndOfStream(length) && 
				(char.IsLetterOrDigit(stream.Peek(length)) || stream.Peek(length) == '_')
			) {
				length++;
			}
			return new Token(TokenType.Identifier, stream.Slice(length));
		}
	}

}