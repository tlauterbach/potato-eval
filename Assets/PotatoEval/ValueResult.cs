using System;
using System.Collections.Generic;

namespace PotatoEval {

	public struct ValueResult : IErrorLogger {
		
		public Value Value { get; private set; }
		public bool HasError { get { return m_errors != null && m_errors.Count > 0; } }
		public IEnumerable<Exception> Errors { get { return m_errors; } }

		private List<Exception> m_errors;

		public static implicit operator Value(ValueResult result) {
			return result.Value;
		}
		public void AddResult(Value value) {
			Value = value;
		}
		public void LogError(string error) {
			if (m_errors == null) {
				m_errors = new List<Exception>();
			}
			m_errors.Add(new Exception(error));
		}
		public void LogError(Exception error) {
			if (m_errors == null) {
				m_errors = new List<Exception>();
			}
			m_errors.Add(error);
		}

	}
}