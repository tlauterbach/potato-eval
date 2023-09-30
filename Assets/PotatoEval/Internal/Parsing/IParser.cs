namespace PotatoEval {


	internal interface IParser {

		int InstructionCount { get; }

		void ParseExpression();
		void ParseExpression(int precedence);

		int Emit(OpCode opCode);
		int Emit(OpCode opCode, uint argument);
		void ChangeArgument(int index, uint argument);
		int AddString(string value);

		Token Expect(TokenType type);
		Token Consume();
		bool Match(TokenType type);
		Token Peek();


	}

}