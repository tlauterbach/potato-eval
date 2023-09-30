namespace PotatoEval {

	internal interface IPattern {
		bool Matches(ICharStream stream);
		Token ToToken(ICharStream stream, out int length);
	}

}