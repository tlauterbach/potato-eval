namespace PotatoEval {

	internal class InfixOpParselet : IInfixParselet {

		public int Precedence { get { return m_opPrecedence; } }

		private readonly BindingPower m_opPrecedence;
		private readonly OpCode m_opCode;

		public InfixOpParselet(BindingPower precedence, OpCode opCode) {
			m_opPrecedence = precedence;
			m_opCode = opCode;
		}

		public void Parse(IParser parser, Token token) {
			parser.ParseExpression(m_opPrecedence);
			parser.Emit(m_opCode);
		}

	}

}