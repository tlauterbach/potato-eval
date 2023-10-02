using System;
using System.Collections.Generic;
using System.Reflection;

namespace PotatoEval {

	public enum ContextErrorMode {
		/// <summary>
		/// When errors occur in the Context, they will be
		/// thrown using the throw keyword.
		/// </summary>
		Throw,
		/// <summary>
		/// When errors occur in the Context, they will be
		/// broadcast using the OnError event. Side effects
		/// may occur as the program will continue to execute
		/// using <see cref="Value.Void"/> or not running
		/// </summary>
		Event,
		/// <summary>
		/// No errors will be thrown and you may experience
		/// strange side effects as the Context will do its
		/// best to handle your mistakes.
		/// </summary>
		Silent
	}

	/// <summary>
	/// Implemenation of IContext that allows for Constants, 
	/// Getters, and Variables. Identifiers can only be used
	/// once in a Context
	/// </summary>
	public class Context : IContext {

		#region Classes

		/// <summary>
		/// Place above a private or public instance function to have
		/// it automatically registered as a function by the BaseContext
		/// </summary>
		protected class FunctionAttribute : Attribute {
			public string alias { get; set; }
			//public ValueKind returns { get; set; } = ValueKind.Void;
		}

		private interface IMember {
			Identifier Identifier { get; }
			Value SetValue(Value value);
			Value GetValue();
			Value DeleteValue();
			IContext GetContext();
			Value Invoke(Value[] arguments);
		}

		private class Getter : IMember {
			public Identifier Identifier { get; }	

			private readonly Context m_owner;
			private readonly Func<Value> m_getter;

			public Getter(Context owner, Identifier id, Func<Value> getter) {
				Identifier = id;	
				m_owner = owner;
				m_getter = getter;
			}
			public Value GetValue() {
				return m_getter();
			}
			public Value SetValue(Value value) {
				m_owner.LogError(new ContextOperationException("SetValue", "Getter", Identifier));
				return Value.Void;
			}
			public Value DeleteValue() {
				m_owner.LogError(new ContextOperationException("DeleteValue", "Getter", Identifier));
				return Value.False;
			}
			public IContext GetContext() {
				m_owner.LogError(new ContextOperationException("GetContext", "Getter", Identifier));
				return m_owner;
			}
			public Value Invoke(Value[] arguments) {
				m_owner.LogError(new ContextOperationException("Invoke", "Getter", Identifier));
				return Value.Void;
			}
		}
		private class Function : IMember {

			public Identifier Identifier { get; }

			private readonly Context m_owner;
			private readonly Func<Value[], Value> m_func;

			public Function(Context owner, Identifier id, MethodInfo member) {
				if (member == null) {
					throw new ArgumentNullException(nameof(member));
				}
				Identifier = id;
				m_owner = owner;
				m_func = (args) => {
					return (Value)member.Invoke(owner, new object[] { args });
				};
			}
			public Function(Context owner, Identifier id, Func<Value[],Value> function) {
				Identifier = id;
				m_owner = owner;
				m_func = function;
			}
			public Value Invoke(Value[] args) {
				return m_func(args);
			}
			public Value GetValue() {
				m_owner.LogError(new ContextOperationException("GetValue", "Function", Identifier));
				return Value.Void;
			}
			public Value SetValue(Value value) {
				m_owner.LogError(new ContextOperationException("SetValue", "Function", Identifier));
				return Value.Void;
			}
			public Value DeleteValue() {
				m_owner.LogError(new ContextOperationException("DeleteValue", "Function", Identifier));
				return Value.False;
			}
			public IContext GetContext() {
				m_owner.LogError(new ContextOperationException("GetContext", "Function", Identifier));
				return m_owner;
			}
		}
		private class SubContext : IMember {

			public Identifier Identifier { get; }
			private readonly Context m_owner;
			private readonly IContext m_context;

			public SubContext(Context owner, Identifier id, IContext context) {
				m_owner = owner;
				Identifier = id;
				m_context = context;
			}
			public Value GetValue() {
				m_owner.LogError(new ContextOperationException("GetValue", "Context", Identifier));
				return Value.Void;
			}
			public Value SetValue(Value value) {
				m_owner.LogError(new ContextOperationException("SetValue", "Context", Identifier));
				return Value.Void;
			}
			public Value DeleteValue() {
				m_owner.LogError(new ContextOperationException("DeleteValue", "Context", Identifier));
				return Value.False;
			}
			public IContext GetContext() {
				return m_context;
			}
			public Value Invoke(Value[] arguments) {
				m_owner.LogError(new ContextOperationException("Invoke", "Context", Identifier));
				return Value.Void;
			}
		}
		private class Constant : IMember {

			public Identifier Identifier { get; }
			private readonly Context m_owner;
			private readonly Value m_value;

			public Constant(Context owner, Identifier id, Value value) {
				m_owner = owner;
				Identifier = id;
				m_value = value;
			}
			public Value GetValue() {
				return m_value;
			}
			public Value SetValue(Value value) {
				m_owner.LogError(new ContextOperationException("SetValue", "Constant", Identifier));
				return Value.Void;
			}
			public IContext GetContext() {
				m_owner.LogError(new ContextOperationException("GetContext", "Constant", Identifier));
				return m_owner;
			}
			public Value DeleteValue() {
				m_owner.LogError(new ContextOperationException("DeleteValue", "Constant", Identifier));
				return Value.False;
			}
			public Value Invoke(Value[] arguments) {
				m_owner.LogError(new ContextOperationException("Invoke", "Constant", Identifier));
				return Value.Void;
			}
		}
		private class Variable : IMember {

			public Identifier Identifier { get; }
			private readonly Context m_owner;
			private Value m_value;

			public Variable(Context owner, Identifier id, Value value) {
				m_owner = owner;
				Identifier = id;
				m_value = value;
			}
			public Value GetValue() {
				return m_value;
			}
			public Value SetValue(Value value) {
				m_value = value;
				return Value.Void;
			}
			public Value DeleteValue() {
				m_value = Value.Void;
				return Value.True;
			}
			public IContext GetContext() {
				m_owner.LogError(new ContextOperationException("GetContext", "Variable", Identifier));
				return m_owner;
			}
			public Value Invoke(Value[] arguments) {
				m_owner.LogError(new ContextOperationException("Invoke", "Variable", Identifier));
				return Value.Void;
			}
		}


		#endregion

		public event Action<Exception> OnException;

		private ContextErrorMode m_errorMode;

		private Dictionary<Identifier, IMember> m_members;

		public Context(ContextErrorMode errorMode) {

			m_members = new Dictionary<Identifier, IMember>();

			Type type = GetType();
			// todo: do signature checks to confirm that the function is
			// actually usable before adding it to the member map
			while (type != null) {
				MethodInfo[] methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
				foreach (MethodInfo method in methods) {
					FunctionAttribute attr = method.GetCustomAttribute<FunctionAttribute>();
					if (attr != null) {
						Identifier id = new Identifier(string.IsNullOrEmpty(attr.alias) ? method.Name : attr.alias);
						m_members.Add(id, new Function(this,id, method));
					}
				}
				type = type.BaseType;
			}

		}

		#region IContext

		public Value GetValue(Identifier id) {
			if (m_members.TryGetValue(id, out IMember member)) {
				return member.GetValue();
			} else {
				LogError(new UndefinedMemberException("Constant, Getter, or Variable", id));
				return Value.Void;
			}
		}
		public Value SetValue(Identifier id, Value value) {
			if (m_members.TryGetValue(id, out IMember member)) {
				return member.SetValue(value);
			} else {
				m_members.Add(id, new Variable(this, id, value));
				return Value.Void;
			}
		}
		public Value DeleteValue(Identifier id) {
			if (m_members.TryGetValue(id, out IMember member)) {
				if (member.DeleteValue() == true) {
					m_members.Remove(id);
					return true;
				}
			}
			return false;
		}

		public Value Invoke(Identifier identifier, Value[] arguments) {
			if (m_members.TryGetValue(identifier, out IMember member)) {
				return member.Invoke(arguments);
			} else {
				LogError(new UndefinedMemberException("Function", identifier));
				return Value.Void;
			}
		}

		public IContext GetContext(Identifier identifier) {
			if (m_members.TryGetValue(identifier, out IMember member)) {
				return member.GetContext();
			} else {
				LogError(new UndefinedMemberException("Context", identifier));
				return this;
			}
		}

		public NarrowedContext ConvertAddress(Address address) {
			if (address == Address.Empty) {
				LogError(new Exception("Address to be converted cannot be Address.Empty"));
				return null;
			}
			return new NarrowedContext(this, address);
		}


		#endregion

		public bool RemoveMember(Identifier id) {
			return m_members.Remove(id);
		}
		public void ClearMembers() {
			m_members.Clear();
		}

		// .. Constants
		public void AddConstant(Identifier id, Value value) {
			if (m_members.ContainsKey(id)) {
				LogError(new PrexistingMemberException(id));
				return;
			}
			m_members.Add(id, new Constant(this, id, value));
		}

		// .. Getters
		public void AddGetter(Identifier id, Func<Value> getter) {
			if (m_members.ContainsKey(id)) {
				LogError(new PrexistingMemberException(id));
				return;
			}
			m_members.Add(id, new Getter(this, id, getter));
		}

		// .. Functions
		public void AddFunction(Identifier id, Func<Value[],Value> function) {
			if (m_members.ContainsKey(id)) {
				LogError(new PrexistingMemberException(id));
				return;
			}
			m_members.Add(id, new Function(this, id, function));
		}

		// .. Variables
		public void AddVariable(Identifier id, Value value) {
			if (m_members.ContainsKey(id)) {
				LogError(new PrexistingMemberException(id));
				return;
			}
			m_members.Add(id, new Variable(this, id, value));
		}

		// .. Context
		public void AddContext(Identifier id, IContext context) {
			if (m_members.ContainsKey(id)) {
				LogError(new PrexistingMemberException(id));
				return;
			}
			m_members.Add(id, new SubContext(this, id, context));
		}


		// .. Helpers

		protected bool DoArgsMatch(Value[] args, int exactLength, params ValueKind[] types) {
			if (args == null || args.Length != exactLength) {
				return false;
			}
			for (int ix = 0; ix < exactLength && ix < types.Length; ix++) {
				if (!args[ix].IsType(types[ix])) {
					return false;
				}
			}
			return true;
		}

		protected void LogError(Exception exception) {
			if (m_errorMode == ContextErrorMode.Event) {
				OnException?.Invoke(exception);
			} else if (m_errorMode == ContextErrorMode.Throw) {
				throw exception;
			}
		}


		// .. common functions

		[Function(alias = "set")]
		private Value SetFunc(Value[] args) {
			if (!DoArgsMatch(args, 2, ValueKind.Address, ValueKind.Any)) {
				throw new FunctionException("set requires 2 arguments: address, any");
			}
			return ConvertAddress(args[0].AsAddress).SetValue(args[1]);
		}

		[Function(alias = "get")]
		private Value GetFunc(Value[] args) {
			if (!DoArgsMatch(args, 1, ValueKind.Address)) {
				throw new FunctionException("get requires 1 argument: address");
			}
			return ConvertAddress(args[0].AsAddress).GetValue();
		}
		
		[Function(alias = "random")]
		private Value RandomFunc(Value[] args) {
			if (DoArgsMatch(args, 2, ValueKind.Number, ValueKind.Number)) {
				return UnityEngine.Random.Range(args[0].AsSingle, args[1].AsSingle);
			} else if (DoArgsMatch(args, 1, ValueKind.Number)) {
				return UnityEngine.Random.Range(0, args[0].AsSingle);
			} else {
				throw new FunctionException("random requires 1 or 2 arguments: number (,number)");
			}
		}
		[Function(alias = "delete")]
		private Value DeleteFunc(Value[] args) {
			if (!DoArgsMatch(args, 1, ValueKind.Address)) {
				throw new FunctionException("delete requires 1 argument: address");
			}
			return ConvertAddress(args[0].AsAddress).DeleteValue();
		}

	}

}