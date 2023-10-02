
using PotatoEval;
using System;

public class TestContext : Context {

	public TestContext() : base(ContextErrorMode.Throw) {
		
	}


	[Function(alias = "num5")]
	private Value Num5(Value[] parameters) {
		return 5;
	}
	[Function(alias = "concat")]
	private Value Concact(Value[] parameters) {
		if (parameters.Length != 2 || !parameters[0].IsString || !parameters[1].IsString) {
			throw new FunctionException("Concat requires two string arguments");
		}
		return string.Concat(parameters[0].AsString, parameters[1].AsString);
	}

}