namespace PotatoEval {

	internal class InfixOpParselet : IInfixParselet {

		public int Precedence { get { return m_bindingPower.BasePrecedence; } }

		private readonly BindingPower m_bindingPower;
		private readonly OpCode m_opCode;

		public InfixOpParselet(BindingPower precedence, OpCode opCode) {
			m_bindingPower = precedence;
			m_opCode = opCode;
		}

		public void Parse(IParser parser, Token token) {
			parser.ParseExpression(m_bindingPower);
			parser.Emit(m_opCode);
		}

	}

}