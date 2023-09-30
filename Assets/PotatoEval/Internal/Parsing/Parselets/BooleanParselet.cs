namespace PotatoEval {

	internal class BooleanParselet : IPrefixParselet {

		private readonly bool m_value;

		public BooleanParselet(bool value) {
			m_value = value;
		}
		public void Parse(IParser parser, Token token) {
			parser.Emit(OpCode.LoadBoolConst, Instruction.Encode(m_value));
		}
	}

}