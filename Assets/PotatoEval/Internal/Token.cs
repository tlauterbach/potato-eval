using System;

namespace PotatoEval {


	internal struct Token : IEquatable<Token>, IEquatable<TokenType> {

		public static readonly Token Empty = new Token();

		public TokenType Type { get { return m_type; } }

		private TokenType m_type;
		private StringSlice m_value;
		private uint m_hash;

		public Token(TokenType type, StringSlice value) {
			m_type = type;
			m_value = value;
			m_hash = Util.FNVHash32(m_type.ToString()) ^ Util.FNVHash32(m_value);
		}
		public static implicit operator TokenType(Token token) {
			return token.m_type;
		}
		public static implicit operator string(Token token) {
			return token.m_value.ToString();
		}
		public static bool operator ==(Token lhs, Token rhs) {
			return lhs.Equals(rhs);
		}
		public static bool operator !=(Token lhs, Token rhs) {
			return !lhs.Equals(rhs);
		}
		public static bool operator ==(Token lhs, TokenType rhs) {
			return lhs.Equals(rhs);
		}
		public static bool operator !=(Token lhs, TokenType rhs) {
			return !lhs.Equals(rhs);
		}

		public bool Equals(Token other) {
			return m_hash == other.m_hash;
		}
		public bool Equals(TokenType other) {
			return other == m_type;
		}
		public override bool Equals(object obj) {
			if (obj is Token token) {
				return Equals(token);
			} else if (obj is TokenType type) {
				return Equals(type);
			} else {
				return false;
			}
		}
		public override int GetHashCode() {
			return unchecked((int)m_hash);
		}
		public override string ToString() {
			string str = m_value.ToString();
			if (string.IsNullOrEmpty(str)) {
				return Type.ToString();
			} else {
				return str;
			}
		}
	}

}