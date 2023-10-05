using System;

namespace PotatoEval {

	public class ConversionException : InvalidCastException {
		public ConversionException(string src, string dest) : base(
			$"Cannot cast `{src}' to `{dest}'"
		) { }
		public ConversionException(ValueKind src, ValueKind dest) : base(
			$"Cannot cast `{src}' to `{dest}'"
		) { }
	}

	public class UndefinedMemberException : Exception {
		public UndefinedMemberException(string type, Identifier identifier) : base(
			$"{type} with identifier `{identifier}' is not defined"
		) { }
	}
	public class ContextOperationException : Exception {
		public ContextOperationException(string operation, string type, Identifier identifier) : base(
			$"Cannot {operation} with identifier {identifier} because it is a {type}"
		) { }
	}
	public class FunctionException : Exception {
		public FunctionException(string message) : base(message) {
		}
	}
	public class PrexistingMemberException : Exception {
		public PrexistingMemberException(Identifier id) : base(
			$"Member with identifier `{id}' is already defined"
		) { }
	}

}