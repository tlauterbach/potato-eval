namespace PotatoEval {

	internal class ConditionalParselet : IInfixParselet {
		public int Precedence { get { return m_bindingPower.Precedence; } }

		private BindingPower m_bindingPower;

		public ConditionalParselet(BindingPower bindingPower) {
			m_bindingPower = bindingPower;
		}

		public void Parse(IParser parser, Token token) {

			int branch1 = parser.Emit(OpCode.JumpIfFalse);

			parser.ParseExpression();
			parser.Expect(TokenType.Colon);

			int branch2 = parser.Emit(OpCode.Jump);

			int label1 = parser.InstructionCount;
			parser.ChangeArgument(branch1, Instruction.Encode(label1 - branch1));

			parser.ParseExpression(m_bindingPower.Calculate());
			int label2 = parser.InstructionCount;
			parser.ChangeArgument(branch2, Instruction.Encode(label2 - branch2));
		}
	}

}