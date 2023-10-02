namespace PotatoEval {

	public interface IContext {
		Value GetValue(Identifier identifier);
		Value SetValue(Identifier identifier, Value value);
		Value DeleteValue(Identifier identifier);
		Value Invoke(Identifier identifier, Value[] arguments);
		IContext GetContext(Identifier identifier);
		NarrowedContext ConvertAddress(Address address);
	}

}