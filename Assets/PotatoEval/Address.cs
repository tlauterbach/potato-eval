using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace PotatoEval {

	public struct Address : IEquatable<Address>, IReadOnlyList<Identifier> {

		public static readonly Address Empty = new Address();

		public int Count {
			get { return m_split?.Length ?? 0; }
		}
		public Identifier this[int index] {
			get {
				if (index < 0 && index >= Count) {
					throw new IndexOutOfRangeException();
				}
				return new Identifier(m_split[index]);
			}
		}
		public Identifier Last { 
			get { return new Identifier(m_split?[Count - 1] ?? string.Empty); }
		}

		private static readonly StringBuilder m_builder = new StringBuilder();

		private string[] m_split;
		private uint m_hash;

		public Address(string path) {
			if (string.IsNullOrEmpty(path)) {
				m_split = null;
				m_hash = 0;
			} else {
				m_split = path.Replace(" ", "").Split('.');
				m_hash = Hash(m_split);
			}
		}
		public Address(List<Identifier> path) {
			if (path == null || path.Count == 0) {
				m_split = null;
				m_hash = 0;
			} else {
				m_split = new string[path.Count];
				for (int ix = 0; ix < m_split.Length; ix++) {
					m_split[ix] = path[ix].ToString();
				}
				m_hash = Hash(m_split);
			}
		}
		public Address(params string[] path) {
			if (path == null || path.Length == 0) {
				m_split = null;
				m_hash = 0;
			} else {
				m_split = path;
				m_hash = Hash(m_split);
			}
		}
		private Address(string[] basePath, string newContext) {
			if ((basePath == null || basePath.Length == 0) && string.IsNullOrEmpty(newContext)) {
				m_split = null;
				m_hash = 0;
			} else {
				m_split = new string[(basePath?.Length ?? 0) + 1];
				if ((basePath?.Length ?? 0) > 0) {
					basePath.CopyTo(m_split, 0);
				}
				m_split[m_split.Length - 1] = newContext;
				m_hash = Hash(m_split);
			}
		}
		private Address(string[] basePath, Address newContext) {
			if ((basePath == null || basePath.Length == 0) && newContext == Empty) {
				m_split = null;
				m_hash = 0;
			} else {
				int baseSize = basePath?.Length ?? 0;
				m_split = new string[baseSize + newContext.Count];
				if (baseSize > 0) {
					basePath.CopyTo(m_split, 0);
				}
				for (int ix = 0; ix < newContext.Count; ix++) {
					m_split[baseSize + ix] = newContext[ix].ToString();
				}
				m_hash = Hash(m_split);
			}
		}

		public static bool operator ==(Address lhs, Address rhs) {
			return lhs.Equals(rhs);
		}
		public static bool operator !=(Address lhs, Address rhs) {
			return !lhs.Equals(rhs);
		}

		public Address Enqueue(string context) {
			return new Address(m_split, context);
		}
		public Address Enqueue(Identifier context) {
			return new Address(m_split, context.ToString());
		}
		public Address Enqueue(Address address) {
			return new Address(m_split, address);
		}

		public bool Equals(Address other) {
			return other.m_hash == m_hash;
		}
		public override bool Equals(object obj) {
			if (obj is Address address) {
				return Equals(address);
			} else {
				return false;
			}
		}
		public override int GetHashCode() {
			return unchecked((int)m_hash);
		}
		public override string ToString() {
			m_builder.Clear();
			for (int ix = 0; ix < m_split.Length; ix++) {
				m_builder.Append(m_split[ix]).Append('.');
			}
			m_builder.Length--;
			return m_builder.ToString();
		}
		public IEnumerator<Identifier> GetEnumerator() {
			return new Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return new Enumerator(this);
		}

		private static uint Hash(string[] split) {
			uint hash = 4679;
			for (int ix = 0; ix < split.Length; ix++) {
				hash = (hash >> ix) ^ Util.FNVHash32(split[ix]);
			}
			return hash;
		}

		private class Enumerator : IEnumerator<Identifier> {
			public Identifier Current {
				get { return new Identifier(m_array[m_index]); }
			}
			object IEnumerator.Current { get { return Current; } }

			private string[] m_array;
			private int m_index = -1;

			public Enumerator(Address baseAddress) {
				m_array = baseAddress.m_split;
				m_index = -1;
			}

			public void Dispose() {
				m_array = null;
				m_index = -1;
			}

			public bool MoveNext() {
				return ++m_index < (m_array?.Length ?? 0);
			}

			public void Reset() {
				m_index = -1;
			}
		}

	}

}