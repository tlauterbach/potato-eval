namespace PotatoEval {

	internal class LogicalAndOpParselet : IInfixParselet {

		public int Precedence { get { return m_bindingPower.BasePrecedence; } }

		private readonly BindingPower m_bindingPower;

		public LogicalAndOpParselet(BindingPower precedence) {
			m_bindingPower = precedence;
		}

		public void Parse(IParser parser, Token token) {
			parser.Emit(OpCode.Duplicate);
			int branch = parser.Emit(OpCode.JumpIfFalse);

			parser.ParseExpression(m_bindingPower);
			parser.Emit(OpCode.LogicalAnd);

			int label = parser.InstructionCount;
			parser.ChangeArgument(branch, Instruction.Encode(label - branch));
		}

	}

}