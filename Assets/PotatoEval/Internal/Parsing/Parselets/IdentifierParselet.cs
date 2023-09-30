
namespace PotatoEval {

	internal class IdentifierParselet : IPrefixParselet {

		public void Parse(IParser parser, Token token) {
			parser.Emit(OpCode.LoadIdentifierConst, Instruction.Encode(parser.AddString(token)));
			//parser.Emit(OpCode.LoadValue, Instruction.Encode(parser.AddString(token)));
		}

	}

}