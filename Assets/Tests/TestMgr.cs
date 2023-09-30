using PotatoEval;
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
	};
	private static readonly List<(string, object)> m_testFunctions = new List<(string, object)>() {
		("num5()", 5),
		("num5() == 5", true),
		("num5() == 3", false),
		("concat(\"a\",\"b\")", "ab"),
		("concat(\"abc\",\"def\")", "abcdef"),
		("set(foo, 5)", Value.Void),
		("foo", 5),
		("get(foo)", 5),
		("get(foo) + 5", 10),
		("set(id, foo)", Value.Void),
		("get(get(id))", 5),
		("set(bar, true)", Value.Void),
		("set(foo, true)", Value.Void),
		("bar == true", true),
		("bar != false", true),
		("bar && bar", true),
		("bar == foo", true),
	};
	private static readonly List<(string, object)> m_testContexts = new List<(string, object)>() {
		("context.num5()", 5),
		("context.set(foo, 3)", Value.Void),
		("context.get(foo)", 3),
		("context.foo", 3),
		("num5() + context.get(foo)", 8),
		("num5() + context.foo", 8),
		("get(context.foo) + (get(foo) ? 3 : 0)", 6),
		("context.set(fizz, foo)", Value.Void),
		("context.fizz == foo", true)
	};



	private void Start() {
		m_evaluator = new Evaluator();
		m_context1 = new TestContext();
		m_context2 = new TestContext();
		m_context1.AddContext("context", m_context2);

		foreach ((string,object) tuple in m_testMathematic) {
			AssertIsTrue(InputEquals(tuple.Item1, tuple.Item2), tuple.Item1);
		}
		foreach ((string, object) tuple in m_testLogic) {
			AssertIsTrue(InputEquals(tuple.Item1, tuple.Item2), tuple.Item1);
		}
		foreach ((string, object) tuple in m_testFunctions) {
			AssertIsTrue(InputEquals(tuple.Item1, tuple.Item2), tuple.Item1);
		}
		foreach ((string, object) tuple in m_testContexts) {
			AssertIsTrue(InputEquals(tuple.Item1, tuple.Item2), tuple.Item1);
		}
	}

	private void AssertIsTrue(bool condition, string message) {
		if (condition) {
			Debug.Log(message);
		} else {
			Debug.LogError(message);
		}
	}

	private bool InputEquals(string input, object obj) {
		if (!Evaluate(input, out Value value)) {
			return false;
		}
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
		} else { 
			throw new NotImplementedException();
		}
	}

	private bool Evaluate(string input, out Value value) {
		ExpressionResult result = m_evaluator.ToExpression(input);
		if (result.HasError) {
			LogErrors(result);	
			value = Value.Void;
			return false;
		}
		ValueResult valueResult = m_evaluator.Evaluate(result, m_context1);
		if (valueResult.HasError) {
			LogErrors(valueResult);
			value = Value.Void;
			return false;
		}
		value = valueResult;
		return true;
	}
	private Value LogErrors(IErrorLogger result) {
		foreach (Exception e in result.Errors) {
			Debug.LogException(e);
		}
		return Value.Void;
	}


}