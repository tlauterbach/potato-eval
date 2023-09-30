namespace PotatoEval {

	internal class AccessOpParselet : IInfixParselet {
		public int Precedence { get { return m_opPrecedence.Precedence; } }

		private readonly BindingPower m_opPrecedence;

		public AccessOpParselet(BindingPower precedence) {
			m_opPrecedence = precedence;
		}

		public void Parse(IParser parser, Token token) {
			parser.Emit(OpCode.PushContext);
			parser.ParseExpression(m_opPrecedence.Calculate());
			parser.Emit(OpCode.PopContext);
		}
	}

}