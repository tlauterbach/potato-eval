namespace PotatoEval {

	internal class LogicalOrOpParselet : IInfixParselet {

		public int Precedence { get { return m_bindingPower.BasePrecedence; } }

		private readonly BindingPower m_bindingPower;

		public LogicalOrOpParselet(BindingPower precedence) {
			m_bindingPower = precedence;
		}
		public void Parse(IParser parser, Token token) {
			parser.Emit(OpCode.Duplicate);
			int branch = parser.Emit(OpCode.JumpIfTrue);

			parser.ParseExpression(m_bindingPower);
			parser.Emit(OpCode.LogicalOr);

			int label = parser.InstructionCount;
			parser.ChangeArgument(branch, Instruction.Encode(label - branch));
		}

	}

}