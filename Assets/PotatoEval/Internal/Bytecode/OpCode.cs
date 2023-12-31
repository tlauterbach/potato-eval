﻿namespace PotatoEval {
	internal enum OpCode : byte {

		LoadNumberConst,
		LoadBoolConst,
		LoadStringConst,
		LoadIdentifierConst,

		Access,
		ValueOf,
		Assignment,
		Invoke,
		Addition,
		Subtraction,
		Multiplication,
		Division,
		Modulo,
		LogicalAnd,
		LogicalOr,
		LogicalNot,
		Negate,
		EqualTo,
		NotEqualTo,
		LessThan,
		LessThanOrEqualTo,
		GreaterThan,
		GreaterThanOrEqualTo,
		ShiftLeft,
		ShiftRight,
		BitwiseAnd,
		BitwiseOr,
		BitwiseXor,
		BitwiseNot,
		
		Jump,
		JumpIfTrue,
		JumpIfFalse,
		Duplicate,
		PushToStorage,
		PopFromStorage
	}

}