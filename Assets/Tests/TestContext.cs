
using PotatoEval;
using System;

public class TestContext : BaseContext {

	public TestContext() : base() {
		
	}


	[Function(alias = "num5", returns = ValueKind.Number)]
	private Value Num5(Value[] parameters) {
		return 5;
	}
	[Function(alias = "concat", returns = ValueKind.String)]
	private Value Concact(Value[] parameters) {
		if (parameters.Length != 2 || !parameters[0].IsString || !parameters[1].IsString) {
			throw new FunctionException("Concat requires two string arguments");
		}
		return string.Concat(parameters[0].AsString, parameters[1].AsString);
	}

}