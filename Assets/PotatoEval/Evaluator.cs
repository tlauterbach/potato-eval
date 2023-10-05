using System;
using System.Collections.Generic;

namespace PotatoEval {

	public class Evaluator : IErrorReport {

		public IEnumerable<Exception> Errors { get { return m_logger.Errors; } }
		public bool HasError { get { return m_logger.HasError; } }

		private Lexer m_lexer;
		private Parser m_parser;
		private EvaluatorInternal m_evaluator;
		private IErrorLogger m_logger;
		private IValueConverter m_converter;

		public Evaluator() : this(Typing.Strong, Conversion.Checked, ErrorMode.Throw) {
		}
		public Evaluator(ErrorMode errorMode) : this(Typing.Strong, Conversion.Checked, errorMode) {
		}
		public Evaluator(Typing typing, Conversion conversion) : this(typing, conversion, ErrorMode.Throw) {
		}
		public Evaluator(Typing typing, Conversion conversion, ErrorMode errorMode) {
			m_lexer = new Lexer();
			m_parser = new Parser();
			m_evaluator = new EvaluatorInternal();
			m_logger = new ErrorLogger(errorMode);
			m_converter = new ValueConverter(typing, conversion, m_logger);
		}

		public Evaluator(IValueConverter converter) {
			m_lexer = new Lexer();
			m_parser = new Parser();
			m_evaluator = new EvaluatorInternal();
			m_converter = converter;
			m_logger = new ErrorLogger(ErrorMode.Throw);
		}
		public Evaluator(IErrorLogger errorLogger) {
			m_lexer = new Lexer();
			m_parser = new Parser();
			m_evaluator = new EvaluatorInternal();
			m_logger = errorLogger;
			m_converter = new ValueConverter(Typing.Strong, Conversion.Checked, errorLogger);
		}
		public Evaluator(IValueConverter converter, IErrorLogger errorLogger) {
			m_lexer = new Lexer();
			m_parser = new Parser();
			m_evaluator = new EvaluatorInternal();
			m_converter = converter;
			m_logger = errorLogger;
		}

		public InstructionBlock ToExpression(string input) {
			return m_parser.Parse(m_lexer.Tokenize(input, m_logger), m_logger);
		}

		public Value Evaluate(InstructionBlock instructions, IContext context) {
			return m_evaluator.Evaluate(instructions, context, m_converter, m_logger);
		}

	}

}