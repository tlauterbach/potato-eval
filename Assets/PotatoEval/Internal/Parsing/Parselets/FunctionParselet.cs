namespace PotatoEval {

	internal class FunctionParselet : IInfixParselet {

		public int Precedence { get { return m_bindingPower.Precedence; } }

		private BindingPower m_bindingPower;

		public FunctionParselet() {
			m_bindingPower = BindingPower.Postfix;
		}

		public void Parse(IParser parser, Token token) {
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

	}

}