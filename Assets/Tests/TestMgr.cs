﻿using PotatoEval;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TestMgr : MonoBehaviour {

	private Evaluator m_evaluator;
	private TestContext m_context1;
	private TestContext m_context2;

	private static readonly List<(string, object)> m_testMathematic = new List<(string,object)>() {
		("2 + 2", 2 + 2),
		("3 - 2", 3 - 2),
		("3 * 3", 3 * 3),
		("8 / 4", 8 / 4),
		("6 % 4", 6 % 4),
		("1 << 5", 1 << 5),
		("1 >> 3", 1 >> 3),
		("4 | 1 | 16", 4 | 1 | 16),
		("4 & 5", 4 & 5),
		("32 ^ 28", 32 ^ 28),
		("~0", ~0),
		("-1", -1),
		("2 + 4 * 8 / 16 % 32", 2 + 4 * 8 / 16 % 32),
	};
	private static readonly List<(string, object)> m_testLogic = new List<(string, object)>() {
		("true == true", true == true),
		("true != false", true != false),
		("true && true", true && true),
		("true && false", true && false),
		("false && true", false && true),
		("true || false", true || false),
		("false || false", false || false),
		("false || true", false || true),
		("true || true", true || true),
		("true && (false || true && true)", true && (false || true && true)),
		("true ? 1 : 0", 1),
		("false ? 1 : 0", 0),
	};
	private static readonly List<(string, object)> m_testFunctions = new List<(string, object)>() {
		("num5()", 5),
		("num5() == 5", true),
		("num5() == 3", false),
		("concat(\"a\",\"b\")", "ab"),
		("concat(\"abc\",\"def\")", "abcdef"),
		("set(foo, 5)", 5),
		("get(foo)", 5),
		("get(foo) + 5", 10),
		("set(id, foo)", new Address("foo")),
		("get(get(id))", 5),
	};
	private static readonly List<(string, object)> m_testContexts = new List<(string, object)>() {
		("set(bar, true)", true),
		("set(foo, true)", true),
		("$foo", true),
		("$bar == true", true),
		("$bar != false", true),
		("$bar && $bar", true),
		("$bar == $foo", true),
		("delete(bar)", true),
		("delete(foo)", true),
		("context.num5()", 5),
		("context.set(foo, 3)", 3),
		("$context.foo", 3),
		("num5() + context.get(foo)", 8),
		("num5() + $context.foo", 8),
		("get(context.foo)", 3),
		("context.set(fizz, context.foo)", new Address("context.foo")),
		("$context.fizz == context.foo", true),
		("get($context.fizz)", 3),
		("$$context.fizz", 3 ),
		("delete(context.foo)", true),
		("delete(context.fizz)", true)
	};
	private static readonly List<(string, object)> m_testAssignments = new List<(string, object)>() {
		("foo = 5", 5),
		("foo += 5", 10),
		("foo -= 5", 5),
		("foo *= 5", 25),
		("foo /= 25", 1),
		("foo <<= 1", 1 << 1),
		("foo >>= 1", 1),
		("foo |= 1", 1),
		("foo &= 2", 0),
		("foo ^= 3", 0 ^ 3),
		("foo = bar = 2", 2),
		("$foo", 2),
		("foo = bar += 1", 3),
		("bar = fizz", new Address("fizz")),
		("fizz = 10", 10),
		("$foo", 3),
		("$foo + 7 == $fizz", true),
		("foo += $fizz", 13),
		("foo -= $fizz", 3),
		("foo = $bar", new Address("fizz")),
		("foo = $$bar", 10),
	};



	private void Start() {
		m_evaluator = new Evaluator();
		m_context1 = new TestContext();
		m_context2 = new TestContext();
		m_context1.AddContext("context", m_context2);

		foreach ((string,object) tuple in m_testMathematic) {
			AssertEquals(tuple, tuple.Item1);
		}
		foreach ((string, object) tuple in m_testLogic) {
			AssertEquals(tuple, tuple.Item1);
		}
		foreach ((string, object) tuple in m_testFunctions) {
			AssertEquals(tuple, tuple.Item1);
		}
		foreach ((string, object) tuple in m_testContexts) {
			AssertEquals(tuple, tuple.Item1);
		}
		foreach ((string, object) tuple in m_testAssignments) {
			AssertEquals(tuple, tuple.Item1);
		}
	}

	private void AssertEquals((string,object) tuple, string message) {
		if (InputEquals(tuple.Item1, tuple.Item2, out Value value)) {
			Debug.Log(message);
		} else {
			Debug.LogError($"{message}\nReceived: {value}\nExpected: {tuple.Item2}");
		}
	}

	private bool InputEquals(string input, object obj, out Value value) {
		value = Evaluate(input);
		if (obj is Value objValue) {
			return value == objValue;
		} else if (obj is string str) {
			return value.IsString && value.AsString == str;
		} else if (obj is int integer) {
			return value.IsNumber && value.AsInt32 == integer;
		} else if (obj is double number) {
			return value.IsNumber && value.AsDouble == number;
		} else if (obj is bool boolean) {
			return value.IsBool && value.AsBool == boolean;
		} else if (obj is Address address) {
			return value.IsAddress && value.AsAddress == address;
		} else {
			throw new NotImplementedException();
		}
	}

	private Value Evaluate(string input) {
		return m_evaluator.Evaluate(m_evaluator.ToExpression(input), m_context1);
	}


}