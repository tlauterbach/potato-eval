namespace PotatoEval {

	public interface IContext {
		Value GetValue(Identifier identifier);
		Value Invoke(Identifier identifier, Value[] arguments);
		IContext GetContext(Identifier identifier);
	}

}