using System;

namespace PotatoEval {

	public class CastException : InvalidCastException {
		internal CastException(string type) : base(
			$"Cannot cast string to `{type}'"
		) { }
		internal CastException(string typeA, string typeB) : base(
			$"Cannot cast `{typeA}' to `{typeB}'"
		) { }
	}

	public class UndefinedValueException : Exception {
		internal UndefinedValueException(string type, Identifier identifier) : base(
			$"{type} with identifier `{identifier}' is not defined"
		) { }
	}
	public class FunctionException : Exception {
		internal FunctionException(string message) : base(message) {
		}
	}
	public class AddIdentifierException : Exception {
		internal AddIdentifierException(string type, Identifier id) : base(
			$"{type} with identifier `{id}' is already defined. Did you forget to remove an old {type}?"
		) { }
	}

}