namespace PotatoEval {

	/// <summary>
	/// used for groupings between open and close parens
	/// </summary>
	internal class GroupParselet : IPrefixParselet {

		public void Parse(IParser parser, Token token) {
			parser.ParseExpression();
			parser.Expect(TokenType.CloseParen);
		}

	}

}