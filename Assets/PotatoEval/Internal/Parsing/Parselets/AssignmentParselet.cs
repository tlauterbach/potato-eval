namespace PotatoEval {

	internal class AssignmentParselet : IInfixParselet {

		private readonly BindingPower m_precedence;
		private readonly bool m_isCompound;
		private readonly OpCode m_opCode;

		public int Precedence { get { return m_precedence.BasePrecedence; } }

		public AssignmentParselet(BindingPower precedence) {
			m_precedence = precedence;
			m_isCompound = false;
		}

		public AssignmentParselet(BindingPower precedence, OpCode code) {
			m_precedence = precedence;
			m_isCompound = true;
			m_opCode = code;
		}

		public void Parse(IParser parser, Token token) {
			parser.ParseExpression(m_precedence);
			if (m_isCompound) {
				parser.Emit(OpCode.PushToStorage);
				parser.Emit(OpCode.Duplicate);
				parser.Emit(OpCode.ValueOf);
				parser.Emit(OpCode.PopFromStorage);
				parser.Emit(m_opCode);
			}
			parser.Emit(OpCode.Assignment);
		}
	}

}