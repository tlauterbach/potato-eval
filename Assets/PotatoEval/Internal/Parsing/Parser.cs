using System.Collections.Generic;

namespace PotatoEval {

	internal class Parser : IParser {

		public int InstructionCount { get { return m_instructions.Count; } }

		private static readonly Dictionary<TokenType, IPrefixParselet> m_prefixParselets = new Dictionary<TokenType, IPrefixParselet>() {
			{ TokenType.OpenParen,	new GroupParselet() },
			{ TokenType.Identifier, new IdentifierParselet() },
			{ TokenType.Number,		new NumberParselet() },
			{ TokenType.String,		new StringParselet() },
			{ TokenType.True,       new BooleanParselet(true) },
			{ TokenType.False,      new BooleanParselet(false) },
			{ TokenType.Bang,		new PrefixOpParselet(BindingPower.Unary, OpCode.LogicalNot) },
			{ TokenType.Minus,		new PrefixOpParselet(BindingPower.Unary, OpCode.Negate) },
			{ TokenType.Tilde,		new PrefixOpParselet(BindingPower.Unary, OpCode.BitwiseNot) },
			{ TokenType.Dollar,		new PrefixOpParselet(BindingPower.Unary, OpCode.ValueOf) },
		};
		private static readonly Dictionary<TokenType, IInfixParselet> m_infixParselets = new Dictionary<TokenType, IInfixParselet>() {
			{ TokenType.OpenParen,			new FunctionParselet() },
			{ TokenType.Question,			new ConditionalParselet(BindingPower.Conditional) },
			{ TokenType.AmpersandAmpersand,	new LogicalAndOpParselet(BindingPower.LogicalAnd) },
			{ TokenType.PipePipe,			new LogicalOrOpParselet(BindingPower.LogicalOr) },
			{ TokenType.Period,				new InfixOpParselet(BindingPower.Postfix, OpCode.Access) },
			{ TokenType.EqualEqual,			new InfixOpParselet(BindingPower.Equality, OpCode.EqualTo) },
			{ TokenType.BangEqual,			new InfixOpParselet(BindingPower.Equality, OpCode.NotEqualTo) },
			{ TokenType.Right,				new InfixOpParselet(BindingPower.Relational, OpCode.GreaterThan) },
			{ TokenType.RightEqual,			new InfixOpParselet(BindingPower.Relational, OpCode.GreaterThanOrEqualTo) },
			{ TokenType.Left,				new InfixOpParselet(BindingPower.Relational, OpCode.LessThan) },
			{ TokenType.LeftEqual,			new InfixOpParselet(BindingPower.Relational, OpCode.LessThanOrEqualTo) },
			{ TokenType.Star,				new InfixOpParselet(BindingPower.Multiplicative, OpCode.Multiplication) },
			{ TokenType.Slash,				new InfixOpParselet(BindingPower.Multiplicative, OpCode.Division) },
			{ TokenType.Percent,			new InfixOpParselet(BindingPower.Multiplicative, OpCode.Modulo) },
			{ TokenType.Plus,				new InfixOpParselet(BindingPower.Additive, OpCode.Addition) },
			{ TokenType.Minus,				new InfixOpParselet(BindingPower.Additive, OpCode.Subtraction) },
			{ TokenType.LeftLeft,			new InfixOpParselet(BindingPower.Shift, OpCode.ShiftLeft) },
			{ TokenType.RightRight,			new InfixOpParselet(BindingPower.Shift, OpCode.ShiftRight) },
			{ TokenType.Ampersand,			new InfixOpParselet(BindingPower.BitwiseAnd, OpCode.BitwiseAnd) },
			{ TokenType.Pipe,				new InfixOpParselet(BindingPower.BitwiseOr, OpCode.BitwiseOr) },
			{ TokenType.Carret,				new InfixOpParselet(BindingPower.BitwiseXor, OpCode.BitwiseXor) },
		};

		private IErrorLogger m_logger;
		private List<Token> m_stream;
		private int m_index;
		private List<Instruction> m_instructions;
		private List<string> m_strings;

		public Parser() {
			m_stream = new List<Token>();
			m_instructions = new List<Instruction>();
			m_strings = new List<string>();
			m_index = 0;
		}

		public InstructionBlock Parse(IEnumerable<Token> tokens, IErrorLogger logger) {
			m_logger = logger;
			m_stream.Clear();
			m_instructions.Clear();
			m_strings.Clear();
			m_stream.AddRange(tokens);
			m_index = 0;
			ParseExpression(int.MinValue);
			InstructionBlock result = new InstructionBlock(m_instructions, m_strings);
			return result;
		}

		public void ParseExpression() {
			// parses with lowest precedence
			ParseExpression(0);
		}
		public void ParseExpression(int precedence) {
			Token token = Consume();
			if (!m_prefixParselets.TryGetValue(token, out IPrefixParselet prefix)) {
				m_logger.LogError($"Could not parse {token}");
				return;
			}
			prefix.Parse(this,token);
			while (m_index < m_stream.Count && precedence < GetBindingPower()) {
				token = Consume();

				if (!m_infixParselets.TryGetValue(token.Type, out IInfixParselet infix)) {
					m_logger.LogError($"Could not parse {token}");
					return;
				}
				infix.Parse(this, token);
			}
		}

		public int Emit(OpCode opCode) {
			m_instructions.Add(new Instruction(opCode, 0));
			return m_instructions.Count - 1;
		}
		public int Emit(OpCode opCode, uint argument) {
			m_instructions.Add(new Instruction(opCode, argument));
			return m_instructions.Count - 1;
		}
		public void ChangeArgument(int index, uint argument) {
			Instruction instr = m_instructions[index];
			instr.Value = argument;
			m_instructions[index] = instr;
		}
		public int AddString(string value) {
			int index = m_strings.IndexOf(value);
			if (index == -1) {
				m_strings.Add(value);
				return m_strings.Count - 1;
			} else {
				return index;
			}
		}

		public Token Expect(TokenType type) {
			Token token = Lookahead(0);
			if (token != type) {
				m_logger.LogError($"Expected token {type} and found {token}");
				return Token.Empty;
			}
			return Consume();
		}
		public Token Consume() {
			Token token = Lookahead(0);
			m_index++;
			return token;
		}
		public bool Match(TokenType type) {
			Token token = Lookahead(0);
			if (token != type) {
				return false;
			}
			Consume();
			return true;
		}
		public Token Peek() {
			return Lookahead(0);
		}

		private Token Lookahead(int distance) {
			if (m_index + distance >= m_stream.Count) {
				m_logger.LogError("Unexpected end of stream");
				return Token.Empty;
			}
			return m_stream[m_index + distance];
		}
		private int GetBindingPower() {
			if (m_infixParselets.TryGetValue(Lookahead(0), out var parselet)) {
				return parselet.Precedence;
			}
			return 0;
		}

	}

}