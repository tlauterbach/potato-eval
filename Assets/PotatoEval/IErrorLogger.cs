using System;
using System.Collections.Generic;

namespace PotatoEval {

	public interface IErrorLogger {
		IEnumerable<Exception> Errors { get; }
		bool HasError { get; }
		void LogError(Exception exception);
		void LogError(string error);
	}

}