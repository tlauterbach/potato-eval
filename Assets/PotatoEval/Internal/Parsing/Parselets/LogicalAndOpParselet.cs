namespace PotatoEval {

	internal class LogicalAndOpParselet : IInfixParselet {

		public int Precedence { get { return m_precedence; } }

		private readonly BindingPower m_precedence;

		public LogicalAndOpParselet(BindingPower precedence) {
			m_precedence = precedence;
		}

		public void Parse(IParser parser, Token token) {
			parser.Emit(OpCode.Duplicate);
			int branch = parser.Emit(OpCode.JumpIfFalse);

			parser.ParseExpression(m_precedence);
			parser.Emit(OpCode.LogicalAnd);

			int label = parser.InstructionCount;
			parser.ChangeArgument(branch, Instruction.Encode(label - branch));
		}

	}

}