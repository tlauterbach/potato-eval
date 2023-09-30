using System.Collections.Generic;

namespace PotatoEval {

	public class Evaluator {

		public static readonly Evaluator Instance = new Evaluator();

		private Lexer m_lexer;
		private Parser m_parser;
		private EvaluatorInternal m_evaluator;

		public Evaluator() {
			m_lexer = new Lexer();
			m_parser = new Parser();
			m_evaluator = new EvaluatorInternal();
		}

		public ExpressionResult ToExpression(string input) {
			ExpressionResult result = new ExpressionResult();

			IEnumerable<Token> tokens = m_lexer.Tokenize(input, result);
			if (result.HasError) {
				return result;
			}
			InstructionBlock expression = m_parser.Parse(tokens, result);
			if (result.HasError) {
				return result;
			}
			result.AddResult(expression);
			return result;
		}

		public ValueResult Evaluate(InstructionBlock instructions, IContext context) {
			ValueResult result = new ValueResult();
			Value value = m_evaluator.Evaluate(instructions, context, result);
			if (result.HasError) {
				return result;
			}
			result.AddResult(value);
			return result;
		}

	}

}