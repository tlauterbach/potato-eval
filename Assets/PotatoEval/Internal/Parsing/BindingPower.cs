namespace PotatoEval {

	internal enum Associativity {
		LeftToRight,
		RightToLeft,
	}

	internal struct BindingPower {

		// Precedence modeled after the C# standard
		// (some are not used)

		public static readonly BindingPower Postfix			= new BindingPower(150, Associativity.LeftToRight);
		public static readonly BindingPower Unary			= new BindingPower(140, Associativity.RightToLeft);
		public static readonly BindingPower Multiplicative	= new BindingPower(130, Associativity.LeftToRight);
		public static readonly BindingPower Additive		= new BindingPower(120, Associativity.LeftToRight);
		public static readonly BindingPower Shift			= new BindingPower(110, Associativity.LeftToRight);
		public static readonly BindingPower Relational		= new BindingPower(100, Associativity.LeftToRight);
		public static readonly BindingPower Equality		= new BindingPower( 90, Associativity.LeftToRight);
		public static readonly BindingPower BitwiseAnd		= new BindingPower( 80, Associativity.LeftToRight);
		public static readonly BindingPower BitwiseXor		= new BindingPower( 70, Associativity.LeftToRight);
		public static readonly BindingPower BitwiseOr		= new BindingPower( 60, Associativity.LeftToRight);
		public static readonly BindingPower LogicalAnd		= new BindingPower( 50, Associativity.LeftToRight);
		public static readonly BindingPower LogicalOr		= new BindingPower( 40, Associativity.LeftToRight);
		public static readonly BindingPower Conditional		= new BindingPower( 30, Associativity.RightToLeft);
		public static readonly BindingPower Assignment		= new BindingPower( 20, Associativity.RightToLeft);
		public static readonly BindingPower Comma			= new BindingPower( 10, Associativity.LeftToRight);

		private int m_precedence;
		
		private BindingPower(int precedence, Associativity associativity) {
			m_precedence = precedence - (associativity == Associativity.RightToLeft ? 1 : 0);
		}
		public static implicit operator int(BindingPower value) {
			return value.m_precedence;
		}

	}

}