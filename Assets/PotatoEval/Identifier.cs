using System;

namespace PotatoEval {

	public struct Identifier : IEquatable<Identifier> {

		public static readonly Identifier Empty = new Identifier();

		private string m_name;
		private uint m_hash;

		public Identifier(string path) {
			m_name = path;
			if (string.IsNullOrEmpty(m_name)) {
				m_hash = 0;
			} else {
				m_hash = Util.FNVHash32(m_name);
			}
		}

		public static bool operator ==(Identifier lhs, Identifier rhs) {
			return lhs.Equals(rhs);
		}
		public static bool operator !=(Identifier lhs, Identifier rhs) {
			return !lhs.Equals(rhs);
		}
		public static implicit operator Identifier(string value) {
			return new Identifier(value);
		}

		public bool Equals(Identifier other) {
			return other.m_hash == m_hash;
		}
		public override bool Equals(object obj) {
			if (obj is Identifier identifier) {
				return Equals(identifier);
			} else {
				return false;
			}
		}
		public override int GetHashCode() {
			return unchecked((int)m_hash);
		}
		public override string ToString() {
			return m_name;
		}

	}

}