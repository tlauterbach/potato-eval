﻿using System;
using System.Collections.Generic;

namespace PotatoEval {

	internal class EvaluatorInternal {

		private static readonly Dictionary<OpCode, string> m_opCodeNames = new Dictionary<OpCode, string> {
			{ OpCode.Addition, "Addition (+)" },
			{ OpCode.Subtraction, "Subtraction (-)" },
			{ OpCode.Multiplication, "Multiplication (*)" },
			{ OpCode.Division, "Division (/)" },
			{ OpCode.Modulo, "Modulo (%)" },
			{ OpCode.ShiftLeft, "Shift Left (<<)" },
			{ OpCode.ShiftRight, "Shift Right (>>)" },
			{ OpCode.BitwiseAnd, "Bitwise And (&)" },
			{ OpCode.BitwiseOr, "Bitwise Or (|)" },
			{ OpCode.BitwiseXor, "Bitwise Xor (^)" },
			{ OpCode.BitwiseNot, "Bitwise Not (~)" },
			{ OpCode.LogicalAnd, "Logical And (&&)" },
			{ OpCode.LogicalOr, "Logical Or (||)" },
			{ OpCode.LogicalNot, "Logical Not (!)" },
			{ OpCode.EqualTo, "Equal To (==)" },
			{ OpCode.NotEqualTo, "Not Equal To (!=)" },
			{ OpCode.GreaterThan, "Greater Than (>)" },
			{ OpCode.GreaterThanOrEqualTo, "Greater Than or Equal To (>=)" },
			{ OpCode.LessThan, "Less Than (<)" },
			{ OpCode.LessThanOrEqualTo, "Less Than or Equal To (<=)" },
			{ OpCode.Negate, "Negate (-)" },


		};

		private Stack<Value> m_valueStack;
		private Stack<IContext> m_contextStack;
		private IErrorLogger m_logger;

		public EvaluatorInternal() {
			m_valueStack = new Stack<Value>();
			m_contextStack = new Stack<IContext>();
		}

		public Value Evaluate(InstructionBlock instructions, IContext context, IErrorLogger logger) {
			m_logger = logger;
			m_valueStack.Clear();
			m_contextStack.Clear();
			m_contextStack.Push(context);
			Instruction[] instructionArray = instructions.Instructions;
			string[] stringTable = instructions.StringTable;
			int programCounter = 0;
			int instructionsLength = instructionArray.Length;
			while (programCounter < instructionsLength && !m_logger.HasError) {
				Instruction instr = instructionArray[programCounter++];
				if (!m_opCodeNames.TryGetValue(instr.OpCode, out string name)) {
					name = instr.OpCode.ToString();
				}
				switch (instr.OpCode) {

					// Load

					case OpCode.LoadNumberConst: {
						PushValue(Instruction.DecodeDouble(instr.Value));
						break;
					}
					case OpCode.LoadBoolConst: {
						PushValue(Instruction.DecodeBool(instr.Value));
						break;
					}
					case OpCode.LoadStringConst: {
						PushValue(stringTable[Instruction.DecodeInt(instr.Value)]);
						break;
					}
					case OpCode.LoadIdentifierConst: {
						PushValue(new Identifier(stringTable[Instruction.DecodeInt(instr.Value)]));
						break;
					}
					/*
					case OpCode.LoadValue: {
						Identifier identifier = new Identifier(stringTable[Instruction.DecodeInt(instr.Value)]);
						Value value = m_contextStack.Peek().GetValue(identifier);
						PushValue(value);
						break;
					}
					*/

					// invoke

					case OpCode.Invoke: {
						int argCount = Instruction.DecodeInt(instr.Value);
						Value[] arguments = new Value[argCount];
						while (--argCount >= 0) {
							arguments[argCount] = PopValue();
						}
						Identifier function = PopValue().AsIdentifier;
						Value result = m_contextStack.Peek().Invoke(function, arguments);
						PushValue(result);
						break;
					}

					// context

					case OpCode.PushContext: {
						//PushContext(PopValue().AsContext);
						Value value = PopValue();
						if (value.IsIdentifier) {
							PushContext(m_contextStack.Peek().GetContext(value.AsIdentifier));
						} else {
							m_logger.LogError($"Identifier is required for Pushing Context");
						}
						break;
					}
					case OpCode.PopContext: {
						PopContext();
						break;
					}

					// Jumps and Duplicate

					case OpCode.Jump: {
						programCounter += Instruction.DecodeInt(instr.Value) - 1;
						break;
					}
					case OpCode.JumpIfFalse: {
						if (PopAndLoadValue() == false) {
							programCounter += Instruction.DecodeInt(instr.Value) - 1;
						}
						break;
					}
					case OpCode.JumpIfTrue: {
						if (PopAndLoadValue() == true) {
							programCounter += Instruction.DecodeInt(instr.Value) - 1;
						}
						break;
					}
					case OpCode.Duplicate: {
						if (m_valueStack.Count > 0) {
							PushValue(m_valueStack.Peek());
						} else {
							m_logger.LogError("No Value on Value Stack to Duplicate");
						}
						break;
					}

					// Operators

					case OpCode.EqualTo: {
						Value rhs = PopAndLoadValue();
						Value lhs = PopAndLoadValue();
						PushValue(lhs == rhs);
						break;
					}
					case OpCode.NotEqualTo: {
						Value rhs = PopAndLoadValue();
						Value lhs = PopAndLoadValue();
						PushValue(lhs != rhs);
						break;
					}
					case OpCode.GreaterThan: {
						PushValue(BinaryDouble(name, GreaterThan));
						break;
					}
					case OpCode.GreaterThanOrEqualTo: {
						PushValue(BinaryDouble(name, GreaterThanOrEqualTo));
						break;
					}
					case OpCode.LessThan: {
						PushValue(BinaryDouble(name, LessThan));
						break;
					}
					case OpCode.LessThanOrEqualTo: {
						PushValue(BinaryDouble(name, LessThanOrEqualTo));
						break;
					}
					case OpCode.Addition: {
						PushValue(BinaryDouble(name, Addition));
						break;
					}
					case OpCode.Subtraction: {
						PushValue(BinaryDouble(name, Subtraction));
						break;
					}
					case OpCode.Multiplication: {
						PushValue(BinaryDouble(name, Multiplication));
						break;
					}
					case OpCode.Division: {
						PushValue(BinaryDouble(name, Division));
						break;
					}
					case OpCode.Modulo: {
						PushValue(BinaryDouble(name, Modulo));
						break;
					}
					case OpCode.LogicalNot: {
						PushValue(UnaryBool(name, LogicalNot));
						break;
					}
					case OpCode.Negate: {
						PushValue(UnaryDouble(name, Negate));
						break;
					}
					case OpCode.LogicalAnd: {
						PushValue(BinaryBool(name, LogicalAnd));
						break;
					}
					case OpCode.LogicalOr: {
						PushValue(BinaryBool(name, LogicalOr));
						break;
					}
					case OpCode.ShiftLeft: {
						PushValue(BinaryInt32(name, ShiftLeft));
						break;
					}
					case OpCode.ShiftRight: {
						PushValue(BinaryInt32(name, ShiftRight));
						break;
					}
					case OpCode.BitwiseAnd: {
						PushValue(BinaryInt32(name, BitwiseAnd));
						break;
					}
					case OpCode.BitwiseOr: {
						PushValue(BinaryInt32(name, BitwiseOr));
						break;
					}
					case OpCode.BitwiseXor: {
						PushValue(BinaryInt32(name, BitwiseXor));
						break;
					}
					case OpCode.BitwiseNot: {
						PushValue(UnaryInt32(name, BitwiseNot));
						break;
					}

				}
			}
			if (m_logger.HasError || m_valueStack.Count == 0) {
				return Value.Void;
			}
			if (m_valueStack.Count > 1) {
				m_logger.LogError($"Missing operators in expression ({m_valueStack.Count} values)");
				return Value.Void;
			}
			return PopAndLoadValue();
		}

		private Value PopAndLoadValue() {
			Value value = PopValue();
			if (value.IsIdentifier) {
				if (m_contextStack.Count >= 1) {
					return m_contextStack.Peek().GetValue(value.AsIdentifier);
				} else {
					m_logger.LogError($"Context Stack is empty");
					return Value.Void;
				}
			} else {
				return value;
			}
		}

		private void PushValue(Value value) {
			m_valueStack.Push(value);
		}
		private Value PopValue() {
			if (m_valueStack.Count > 0) {
				return m_valueStack.Pop();
			} else {
				m_logger.LogError("Value Stack is empty");
				return Value.Void;
			}
		}
		private void PushContext(IContext context) {
			m_contextStack.Push(context);
		}
		private IContext PopContext() {
			if (m_contextStack.Count > 0) {
				return m_contextStack.Pop();
			} else {
				m_logger.LogError("Context Stack is empty");
				return null;
			}
		}

		private T UnaryInt32<T>(string name, Func<int,T> op) {
			Value operand = PopAndLoadValue();
			if (operand.IsNumber) {
				try {
					return op(operand.AsInt32);
				} catch {
					m_logger.LogError($"{name} requires value be a valid Int32");
					return default;
				}
			}
			m_logger.LogError($"{name} requires value to be a Number");
			return default;
		}
		private T UnaryDouble<T>(string name, Func<double,T> op) {
			Value operand = PopAndLoadValue();
			if (operand.IsNumber) {
				return op(operand.AsDouble);
			}
			m_logger.LogError($"{name} requires value to be a Number");
			return default;
		}

		private T UnaryBool<T>(string name, Func<bool, T> op) {
			Value operand = PopAndLoadValue();
			if (operand.IsBool) {
				return op(operand.AsBool);
			}
			m_logger.LogError($"{name} requires value to be a Boolean");
			return default;
		}

		private T BinaryDouble<T>(string name, Func<double, double, T> op) {
			Value rhs = PopAndLoadValue();
			Value lhs = PopAndLoadValue();
			if (rhs.IsNumber && lhs.IsNumber) {
				return op(rhs.AsDouble, lhs.AsDouble);
			}
			m_logger.LogError($"{name} requires both values be Numbers");
			return default;
		}
		private T BinaryInt32<T>(string name, Func<int, int, T> op) {
			Value rhs = PopAndLoadValue();
			Value lhs = PopAndLoadValue();
			if (rhs.IsNumber && lhs.IsNumber) {
				try {
					return op(rhs.AsInt32, lhs.AsInt32);
				} catch {
					m_logger.LogError($"{name} requires both values be valid Int32s");
					return default;
				}
			}
			m_logger.LogError($"{name} requires both values be Numbers");
			return default;
		}
		private T BinaryBool<T>(string name, Func<bool, bool,T> op) {
			Value rhs = PopAndLoadValue();
			Value lhs = PopAndLoadValue();
			if (rhs.IsBool && lhs.IsBool) {
				return op(rhs.AsBool, lhs.AsBool);
			}
			m_logger.LogError($"{name} requires both values be Booleans");
			return default;
		}

		private static bool LogicalAnd(bool rhs, bool lhs) {
			return lhs && rhs;
		}
		private static bool LogicalOr(bool rhs, bool lhs) {
			return lhs || rhs;
		}
		private static bool LogicalNot(bool operand) {
			return !operand;
		}

		private static bool GreaterThan(double rhs, double lhs) {
			return lhs > rhs;
		}
		private static bool LessThan(double rhs, double lhs) {
			return lhs < rhs;
		}
		private static bool GreaterThanOrEqualTo(double rhs, double lhs) {
			return lhs >= rhs;
		}
		private static bool LessThanOrEqualTo(double rhs, double lhs) {
			return lhs <= rhs;
		}

		private static double Addition(double rhs, double lhs) {
			return lhs + rhs;
		}
		private static double Multiplication(double rhs, double lhs) {
			return lhs * rhs;
		}
		private static double Subtraction(double rhs, double lhs) {
			return lhs - rhs;
		}
		private double Division(double rhs, double lhs) {
			if (rhs == 0) {
				m_logger.LogError("Division by 0");
				return 0;
			} else {
				return lhs / rhs;
			}
		}
		private double Modulo(double rhs, double lhs) {
			if (rhs == 0) {
				m_logger.LogError("Modulo by 0");
				return lhs;
			} else {
				return lhs % rhs;
			}
		}
		private static double Negate(double rhs) {
			return -rhs;
		}


		private static int ShiftLeft(int rhs, int lhs) {
			return lhs << rhs;
		}
		private static int ShiftRight(int rhs, int lhs) {
			return lhs >> rhs;
		}
		private static int BitwiseAnd(int rhs, int lhs) {
			return lhs & rhs;
		}
		private static int BitwiseOr(int rhs, int lhs) {
			return lhs | rhs;
		}
		private static int BitwiseXor(int rhs, int lhs) {
			return lhs ^ rhs;
		}
		private static int BitwiseNot(int operand) {
			return ~operand;
		}

	}

}