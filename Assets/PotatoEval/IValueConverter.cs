namespace PotatoEval {
	public interface IValueConverter {
		bool IsVoid(Value value);
		bool IsNumber(Value value);
		bool IsString(Value value);
		bool IsBoolean(Value value);
		bool IsAddress(Value value);

		string ToString(Value value);
		short ToInt16(Value value);
		int ToInt32(Value value);
		long ToInt64(Value value);
		ushort ToUInt16(Value value);
		uint ToUInt32(Value value);
		ulong ToUInt64(Value value);
		double ToDouble(Value value);
		float ToSingle(Value value);
		decimal ToDecimal(Value value);
		byte ToByte(Value value);
		sbyte ToSByte(Value value);
		bool ToBool(Value value);
		Address ToAddress(Value value);
	}





}