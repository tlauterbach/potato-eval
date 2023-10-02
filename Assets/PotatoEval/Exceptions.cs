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

	public class UndefinedMemberException : Exception {
		internal UndefinedMemberException(string type, Identifier identifier) : base(
			$"{type} with identifier `{identifier}' is not defined"
		) { }
	}
	public class ContextOperationException : Exception {
		internal ContextOperationException(string operation, string type, Identifier identifier) : base(
			$"Cannot {operation} with identifier {identifier} because it is a {type}"
		) { }
	}
	public class FunctionException : Exception {
		internal FunctionException(string message) : base(message) {
		}
	}
	public class PrexistingMemberException : Exception {
		internal PrexistingMemberException(Identifier id) : base(
			$"Member with identifier `{id}' is already defined"
		) { }
	}

}