using System;
using System.Collections.Generic;

namespace PotatoEval {


	internal class Lexer : ICharStream {

		public int Index {
			get { return m_index; }
		}
		public int Length {
			get { return m_input.Length; }
		}
		public char Current {
			get { return m_input[m_index]; }
		}

		private readonly List<IPattern> m_patterns = new List<IPattern>() {
			new PatternKeyword(TokenType.Undefined, "undefined"),
			new PatternKeyword(TokenType.False, "false"),
			new PatternKeyword(TokenType.True, "true"),
			new PatternIdentifier(),
			new PatternNumber(),
			new PatternString(),
			new PatternSymbol(TokenType.OpenParen,			"("),
			new PatternSymbol(TokenType.CloseParen,			")"),
			new PatternSymbol(TokenType.Comma,				","),
			new PatternSymbol(TokenType.Period,				"."),
			new PatternSymbol(TokenType.Dollar,				"$"),
			new PatternSymbol(TokenType.EqualEqual,         "=="),
			new PatternSymbol(TokenType.BangEqual,			"!="),
			new PatternSymbol(TokenType.RightEqual,			">="),
			new PatternSymbol(TokenType.LeftEqual,			"<="),
			new PatternSymbol(TokenType.LeftLeft,			"<<"),
			new PatternSymbol(TokenType.RightRight,			">>"),
			new PatternSymbol(TokenType.Right,				">"),
			new PatternSymbol(TokenType.Left,				"<"),
			new PatternSymbol(TokenType.AmpersandAmpersand,	"&&"),
			new PatternSymbol(TokenType.PipePipe,			"||"),
			new PatternSymbol(TokenType.Bang,				"!"),
			new PatternSymbol(TokenType.Plus,				"+"),
			new PatternSymbol(TokenType.Minus,				"-"),
			new PatternSymbol(TokenType.Star,				"*"),
			new PatternSymbol(TokenType.Slash,				"/"),
			new PatternSymbol(TokenType.Percent,			"%"),
			new PatternSymbol(TokenType.Ampersand,			"&"),
			new PatternSymbol(TokenType.Pipe,				"|"),
			new PatternSymbol(TokenType.Carret,				"^"),
			new PatternSymbol(TokenType.Tilde,				"~"),
			new PatternSymbol(TokenType.Question,			"?"),
			new PatternSymbol(TokenType.Colon,				":")
		};

		private IErrorLogger m_logger;
		private string m_input;
		private int m_index;
		private List<Token> m_output;

		public Lexer() {
			m_output = new List<Token>();
		}

		public IEnumerable<Token> Tokenize(string input, IErrorLogger logger) {
			m_logger = logger;
			m_output.Clear();
			m_input = input;
			m_index = 0;
			while (!IsEndOfStream(0)) {
				// skip whitespace
				if (char.IsWhiteSpace(Peek())) {
					Advance();
					continue;
				}
				bool addedToken = false;
				foreach (IPattern pattern in m_patterns) {
					if (pattern.Matches(this)) {
						m_output.Add(pattern.ToToken(this, out int length));
						Advance(length);
						addedToken = true;
						break;
					}
				}
				if (!addedToken) {
					m_logger.LogError($"Unrecognized character `{Peek()}' in expression.");
					yield break;
				}
			}
			foreach (Token token in m_output) {
				yield return token;
			}
			m_input = null;
			m_index = 0;
			m_output.Clear();
		}
		public char Peek() {
			return m_input[m_index];
		}
		public char Peek(int distance) {
			if (m_index + distance >= m_input.Length) {
				m_logger.LogError($"Peek of {distance} exceeds the length of the stream");
			}
			return m_input[m_index + distance];
		}
		public StringSlice Slice(int length) {
			if (IsEndOfStream(Math.Max(0,length-1))) {
				m_logger.LogError($"Slice of {length} exceeds the length of the stream");
			}
			return new StringSlice(m_input, m_index, length);
		}
		public void Advance(int distance = 1) {
			m_index = Math.Min(m_input.Length, m_index + distance);
		}
		public bool IsEndOfStream(int distance = 1) {
			return m_index + distance >= m_input.Length;
		}
		

	}

}