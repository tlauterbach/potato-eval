using System;

namespace PotatoEval {

	/// <summary>
	/// Narrowed version of an IContext that has an identifier
	/// supplied for easier access to context functions. Can also
	/// be used to provide custom NarrowedContexts for the 
	/// <see cref="IContext.ConvertAddress(Address)"/> interface 
	/// implementations
	/// </summary>
	public sealed class NarrowedContext {

		private readonly IContext m_context;
		private readonly Identifier m_identifier;

		/// <summary>
		/// If you have a custom lookup for additional contexts
		/// via Address, do them then call this constructor to 
		/// provide your own IContext and Identifier
		/// </summary>
		public NarrowedContext(IContext context, Identifier identifier) {
			if (context == null) {
				throw new ArgumentNullException(nameof(context));
			}
			if (identifier == Identifier.Empty) {
				throw new ArgumentException("identifier argument cannot be Identifier.Empty");
			}
			m_context = context;
			m_identifier = identifier;
		}
		/// <summary>
		/// Used to convert an Address struct into an IContext
		/// and Identifier found within the given base IContext
		/// </summary>
		public NarrowedContext(IContext baseContext, Address address) {
			if (baseContext == null) {
				throw new ArgumentNullException(nameof(baseContext));
			}
			if (address == Address.Empty) {
				throw new ArgumentException("address argument cannot be Address.Empty");
			}
			IContext context = baseContext;
			Identifier identifier = address.Last;
			int index = 0;
			while (index < address.Count - 1) {
				context = context.GetContext(address[index++]);
			}
			m_context = context;
			m_identifier = identifier;
		}

		public Value GetValue() {
			return m_context.GetValue(m_identifier);
		}
		public Value SetValue(Value value) {
			return m_context.SetValue(m_identifier, value);
		}
		public Value DeleteValue() {
			return m_context.DeleteValue(m_identifier);
		}
		public IContext GetContext() {
			return m_context.GetContext(m_identifier);
		}
		public Value Invoke(Value[] arguments) {
			return m_context.Invoke(m_identifier, arguments);
		}
	}

}