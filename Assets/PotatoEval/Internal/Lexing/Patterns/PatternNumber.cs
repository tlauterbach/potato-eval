namespace PotatoEval {

	internal class PatternNumber : IPattern {
		public bool Matches(ICharStream stream) {
			return char.IsDigit(stream.Peek()) || (stream.Peek() == '.' && char.IsDigit(stream.Peek(1)));
		}

		public Token ToToken(ICharStream stream, out int length) {
			length = 1;
			bool hadDot = stream.Peek() == '.';
			while (
				!stream.IsEndOfStream(length) &&
				(char.IsDigit(stream.Peek(length)) ||
				(stream.Peek(length) == '.' && !hadDot))
			) {
				hadDot |= stream.Peek(length) == '.';
				length++;
			}
			return new Token(TokenType.Number, stream.Slice(length));
		}
	}

}