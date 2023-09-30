using System.Collections.Generic;

namespace PotatoEval {

	public class InstructionBlock {

		internal Instruction[] Instructions { get; }
		internal string[] StringTable { get; }

		internal InstructionBlock(List<Instruction> instructions, List<string> strings) {
			Instructions = new Instruction[instructions.Count];
			for (int ix = 0; ix < instructions.Count; ix++) {
				Instructions[ix] = instructions[ix];
			}
			StringTable = new string[strings.Count];
			for (int ix = 0; ix < strings.Count; ix++) {
				StringTable[ix] = strings[ix];
			}
		}

	}

}