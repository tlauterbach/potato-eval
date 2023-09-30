namespace PotatoEval {

	internal class StringParselet : IPrefixParselet {

		public void Parse(IParser parser, Token token) {
			string withQuotes = token.ToString();
			string noQuotes = withQuotes.Substring(1, withQuotes.Length - 2).Replace("\\\"", "\"");
			parser.Emit(OpCode.LoadStringConst, Instruction.Encode(parser.AddString(noQuotes)));
		}

	}

}