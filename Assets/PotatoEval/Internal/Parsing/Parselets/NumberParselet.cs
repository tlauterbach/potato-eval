namespace PotatoEval {

	internal class NumberParselet : IPrefixParselet {

		public void Parse(IParser parser, Token token) {
			parser.Emit(OpCode.LoadNumberConst, Instruction.Encode(double.Parse(token)));
		}

	}

}