namespace PotatoEval {

	internal interface IInfixParselet {
		void Parse(IParser parser, Token token);
		int Precedence { get; }
	}

}