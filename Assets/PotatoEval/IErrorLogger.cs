using System;
using System.Collections.Generic;

namespace PotatoEval {

	public interface IErrorReport {
		IEnumerable<Exception> Errors { get; }
		bool HasError { get; }
	}

	public interface IErrorLogger : IErrorReport {
		void LogError(Exception exception);
		void LogError(string error);
	}

	public enum ErrorMode {
		Throw,
		Log,
		Silent,
	}


	public class ErrorLogger : IErrorLogger {
		public IEnumerable<Exception> Errors { get { return m_errors; } }
		public bool HasError { get { return m_errors.Count > 0; } }

		private ErrorMode m_mode;
		private List<Exception> m_errors;

		public ErrorLogger(ErrorMode mode) {
			m_mode = mode;
			m_errors = new List<Exception>();
		}

		public void LogError(Exception exception) {
			if (m_mode == ErrorMode.Throw) {
				throw exception;
			} else if (m_mode == ErrorMode.Log) {
				m_errors.Add(exception);
			}
		}

		public void LogError(string message) {
			if (m_mode == ErrorMode.Throw) {
				throw new Exception(message);
			} else if (m_mode == ErrorMode.Log) {
				m_errors.Add(new Exception(message));
			}
		}



	}

}