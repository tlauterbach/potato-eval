
namespace PotatoEval {

	internal class IdentifierParselet : IPrefixParselet {

		public void Parse(IParser parser, Token token) {
			parser.Emit(OpCode.LoadIdentifierConst, Instruction.Encode(parser.AddString(token)));
			/*
			if (parser.Match(TokenType.OpenParen)) {
				int argCount = 0;
				if (!parser.Match(TokenType.CloseParen)) {
					do {
						parser.ParseExpression();
						argCount++;
					} while (parser.Match(TokenType.Comma));
					parser.Expect(TokenType.CloseParen);
				}
				parser.Emit(OpCode.Invoke, Instruction.Encode(argCount));
			}
			*/
		}

	}

}