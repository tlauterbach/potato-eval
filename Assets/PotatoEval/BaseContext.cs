using System;
using System.Collections.Generic;
using System.Reflection;

namespace PotatoEval {

	/// <summary>
	/// Implemenation of IContext that allows for Constants, 
	/// Getters, and Variables. If a duplicate name is used, 
	/// Constants preceed Getters, and Getters preceed Variables.
	/// </summary>
	public abstract class BaseContext : IContext {

		#region Classes

		/// <summary>
		/// Place above a private or public instance function to have
		/// it automatically registered as a function by the BaseContext
		/// </summary>
		protected class FunctionAttribute : Attribute {
			public string alias { get; set; }
			public ValueKind returns { get; set; } = ValueKind.Void;
		}
		private class GetterWrapper {
			public ValueKind ReturnType {
				get { return m_returnType; }
			}

			private readonly ValueKind m_returnType;
			private readonly Func<Value> m_getter;

			public GetterWrapper(Func<Value> getter, ValueKind returnType) {
				m_returnType = returnType;
				m_getter = getter;
			}
			public Value GetValue() {
				return m_getter();
			}
		}
		private class FunctionWrapper {

			private readonly Func<Value[], Value> m_func;

			public FunctionWrapper(BaseContext owner, MethodInfo member) {
				if (member == null) {
					throw new ArgumentNullException(nameof(member));
				}
				m_func = (args) => {
					return (Value)member.Invoke(owner, new object[] { args });
				};
			}
			public FunctionWrapper(Func<Value[],Value> function) {
				m_func = function;
			}
			public Value Invoke(Value[] args) {
				return m_func(args);
			}
		}

		#endregion

		private Dictionary<Identifier, Value> m_constants;
		private Dictionary<Identifier, GetterWrapper> m_getters;
		private Dictionary<Identifier, Value> m_variables;
		private Dictionary<Identifier, FunctionWrapper> m_functions;
		private Dictionary<Identifier, IContext> m_objects;

		public BaseContext() {
			m_constants = new Dictionary<Identifier, Value>();
			m_getters = new Dictionary<Identifier, GetterWrapper>();
			m_variables = new Dictionary<Identifier, Value>();
			m_functions = new Dictionary<Identifier, FunctionWrapper>();
			m_objects = new Dictionary<Identifier, IContext>();

			Type type = GetType();
			while (type != null) {
				MethodInfo[] methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
				foreach (MethodInfo method in methods) {
					FunctionAttribute attr = method.GetCustomAttribute<FunctionAttribute>();
					if (attr != null) {
						m_functions.Add(new Identifier(attr.alias), new FunctionWrapper(this, method));
					}
				}
				type = type.BaseType;
			}

		}

		#region IContext

		public Value GetValue(Identifier id) {
			if (m_constants.ContainsKey(id)) {
				return m_constants[id];
			} else if (m_getters.ContainsKey(id)) {
				return m_getters[id].GetValue();
			} else if (m_variables.ContainsKey(id)) {
				return m_variables[id];
			} else {
				throw new UndefinedValueException("Value", id);
			}
		}

		public Value Invoke(Identifier identifier, Value[] arguments) {
			if (m_functions.TryGetValue(identifier, out FunctionWrapper function)) {
				return function.Invoke(arguments);
			} else {
				throw new UndefinedValueException("Function", identifier);
			}
		}

		public IContext GetContext(Identifier identifier) {
			if (m_objects.TryGetValue(identifier, out IContext child)) {
				return child;
			} else {
				throw new UndefinedValueException("Context", identifier);
			}
		}


		#endregion

		// .. Constants
		public void AddConstant(Identifier id, Value value) {
			if (m_constants.ContainsKey(id)) {
				throw new AddIdentifierException("Constant", id);
			}
			m_constants.Add(id, value);
		}
		public bool RemoveConstant(Identifier id) {
			return m_constants.Remove(id);
		}
		public void ClearConstants() {
			m_constants.Clear();
		}

		// .. Getters
		public void AddGetter(Identifier id, Func<Value> getter, ValueKind returnType) {
			if (m_getters.ContainsKey(id)) {
				throw new AddIdentifierException("Getter", id);
			}
			m_getters.Add(id, new GetterWrapper(getter, returnType));
		}
		public bool RemoveGetter(Identifier id) {
			return m_getters.Remove(id);
		}
		public void ClearGetters() {
			m_getters.Clear();
		}

		// .. Functions
		public void AddFunction(Identifier id, Func<Value[],Value> function) {
			if (m_functions.ContainsKey(id)) {
				throw new AddIdentifierException("Function", id);
			}
			m_functions.Add(id, new FunctionWrapper(function));
		}
		public bool RemoveFunction(Identifier id) {
			return m_functions.Remove(id);
		}

		// .. Variables
		public void AddVariable(Identifier id, Value value) {
			if (IsVariableDefined(id)) {
				throw new AddIdentifierException("Variable", id);
			}
			m_variables.Add(id, value);
		}
		public bool RemoveVariable(Identifier id) {
			return m_variables.Remove(id);
		}
		public void SetVariable(Identifier id, Value value) {
			if (IsVariableDefined(id)) {
				m_variables[id] = value;
			} else {
				m_variables.Add(id, value);
			}
		}
		public void ClearVariables() {
			m_variables.Clear();
		}

		// .. Context
		public void AddContext(Identifier id, IContext context) {
			if (m_objects.ContainsKey(id)) {
				throw new AddIdentifierException("Context", id);
			}
			m_objects.Add(id, context);
		}
		public bool RemoveContext(Identifier id) {
			return m_objects.Remove(id);
		}
		public void ClearContexts() {
			m_objects.Clear();
		}


		// .. Helpers
		protected bool IsVariableDefined(Identifier id) {
			return m_variables.ContainsKey(id);
		}

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


		// .. common functions

		[Function(alias = "set")]
		private Value SetFunc(Value[] args) {
			if (!DoArgsMatch(args, 2, ValueKind.Identifier, ValueKind.Any)) {
				throw new FunctionException("set requires 2 arguments: identifier, any");
			}
			SetVariable(args[0].AsIdentifier, args[1]);
			return Value.Void;
		}

		[Function(alias = "get", returns = ValueKind.Any)]
		private Value GetFunc(Value[] args) {
			if (!DoArgsMatch(args, 1, ValueKind.Identifier)) {
				throw new FunctionException("get requires 1 argument: identifier");
			}
			return GetValue(args[0].AsIdentifier);
		}
		
		[Function(alias = "random", returns = ValueKind.Number)]
		private Value Random(Value[] args) {
			if (DoArgsMatch(args, 2, ValueKind.Number, ValueKind.Number)) {
				return UnityEngine.Random.Range(args[0].AsSingle, args[1].AsSingle);
			} else if (DoArgsMatch(args, 1, ValueKind.Number)) {
				return UnityEngine.Random.Range(0, args[0].AsSingle);
			} else {
				throw new FunctionException("random requires 1 or 2 arguments: number (,number)");
			}
		}

	}

}