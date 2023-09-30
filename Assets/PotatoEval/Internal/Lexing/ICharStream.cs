
namespace PotatoEval {
	internal interface ICharStream {

		int Index { get; }
		int Length { get; }
		char Peek();
		char Peek(int distance);
		StringSlice Slice(int distance);

		bool IsEndOfStream(int distance = 1);

	}
}