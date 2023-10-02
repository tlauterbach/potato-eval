namespace PotatoEval {

	internal class LogicalOrOpParselet : IInfixParselet {

		public int Precedence { get { return m_precedence; } }

		private readonly BindingPower m_precedence;

		public LogicalOrOpParselet(BindingPower precedence) {
			m_precedence = precedence;
		}
		public void Parse(IParser parser, Token token) {
			parser.Emit(OpCode.Duplicate);
			int branch = parser.Emit(OpCode.JumpIfTrue);

			parser.ParseExpression(m_precedence);
			parser.Emit(OpCode.LogicalOr);

			int label = parser.InstructionCount;
			parser.ChangeArgument(branch, Instruction.Encode(label - branch));
		}

	}

}