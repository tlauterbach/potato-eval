using System;

namespace PotatoEval {

	public enum Typing {
		Weak,
		Strong
	}
	public enum Conversion {
		Unchecked,
		Checked,
	}

	public class ValueConverter : IValueConverter {
		public static readonly ValueConverter WeakUnchecked = new ValueConverter(Typing.Weak, Conversion.Unchecked);
		public static readonly ValueConverter WeakChecked = new ValueConverter(Typing.Weak, Conversion.Checked);
		public static readonly ValueConverter StrongUnchecked = new ValueConverter(Typing.Strong, Conversion.Unchecked);
		public static readonly ValueConverter StrongChecked = new ValueConverter(Typing.Strong, Conversion.Checked);

		private Typing m_typing;
		private Conversion m_conversion;
		private IErrorLogger m_logger;

		#region Constructors

		public ValueConverter() : this(Typing.Strong, Conversion.Checked, new ErrorLogger(ErrorMode.Throw)) {
		}
		public ValueConverter(IErrorLogger errorLogger) : this(Typing.Strong,Conversion.Checked, errorLogger) {
		}
		public ValueConverter(Typing typing, Conversion conversion) : this(typing,conversion,new ErrorLogger(ErrorMode.Throw)) {
		}
		public ValueConverter(Typing typing, Conversion conversion, IErrorLogger errorLogger) {
			m_typing = typing;
			m_conversion = conversion;
			m_logger = errorLogger;
		}

		#endregion

		public bool IsVoid(Value value) {
			return m_typing == Typing.Weak || value.Type == ValueKind.Void;
		}
		public bool IsString(Value value) {
			return m_typing == Typing.Weak || value.Type == ValueKind.String;
		}
		public bool IsNumber(Value value) {
			return m_typing == Typing.Weak || value.Type == ValueKind.Number;
		}
		public bool IsBoolean(Value value) {
			return m_typing == Typing.Weak || value.Type == ValueKind.Boolean;
		}
		public bool IsAddress(Value value) {
			return m_typing == Typing.Weak || value.Type == ValueKind.Address;
		}

		public string ToString(Value value) {
			return ConvertRawString(value);
		}
		public double ToDouble(Value value) {
			return ConvertRawNumber(value);
		}
		public bool ToBool(Value value) {
			return ConvertRawNumber(value) > 0;
		}
		public Address ToAddress(Value value) {
			string raw = ConvertRawString(value);
			return new Address(raw);
		}

		public short ToInt16(Value value) {
			try {
				double raw = ConvertRawNumber(value);
				return (m_conversion == Conversion.Checked) ? checked((short)raw) : unchecked((short)raw);
			} catch (Exception e) {
				m_logger.LogError(e);
				return default;
			}
		}
		public int ToInt32(Value value) {
			try {
				double raw = ConvertRawNumber(value);
				return (m_conversion == Conversion.Checked) ? checked((int)raw) : unchecked((int)raw);
			} catch (Exception e) {
				m_logger.LogError(e);
				return default;
			}
		}
		public long ToInt64(Value value) {
			try {
				double raw = ConvertRawNumber(value);
				return (m_conversion == Conversion.Checked) ? checked((long)raw) : unchecked((long)raw);
			} catch (Exception e) {
				m_logger.LogError(e);
				return default;
			}
		}

		public ushort ToUInt16(Value value) {
			try {
				double raw = ConvertRawNumber(value);
				return (m_conversion == Conversion.Checked) ? checked((ushort)raw) : unchecked((ushort)raw);
			} catch (Exception e) {
				m_logger.LogError(e);
				return default;
			}
		}
		public uint ToUInt32(Value value) {
			try {
				double raw = ConvertRawNumber(value);
				return (m_conversion == Conversion.Checked) ? checked((uint)raw) : unchecked((uint)raw);
			} catch (Exception e) {
				m_logger.LogError(e);
				return default;
			}
		}
		public ulong ToUInt64(Value value) {
			try {
				double raw = ConvertRawNumber(value);
				return (m_conversion == Conversion.Checked) ? checked((ulong)raw) : unchecked((ulong)raw);
			} catch (Exception e) {
				m_logger.LogError(e);
				return default;
			}
		}
		public byte ToByte(Value value) {
			try {
				double raw = ConvertRawNumber(value);
				return (m_conversion == Conversion.Checked) ? checked((byte)raw) : unchecked((byte)raw);
			} catch (Exception e) {
				m_logger.LogError(e);
				return default;
			}
		}
		public sbyte ToSByte(Value value) {
			try {
				double raw = ConvertRawNumber(value);
				return (m_conversion == Conversion.Checked) ? checked((sbyte)raw) : unchecked((sbyte)raw);
			} catch (Exception e) {
				m_logger.LogError(e);
				return default;
			}
		}
		public float ToSingle(Value value) {
			try {
				double raw = ConvertRawNumber(value);
				return (m_conversion == Conversion.Checked) ? checked((float)raw) : unchecked((float)raw);
			} catch (Exception e) {
				m_logger.LogError(e);
				return default;
			}
		}
		public decimal ToDecimal(Value value) {
			try {
				double raw = ConvertRawNumber(value);
				return (m_conversion == Conversion.Checked) ? checked((decimal)raw) : unchecked((decimal)raw);
			} catch (Exception e) {
				m_logger.LogError(e);
				return default;
			}
		}
		


		private double ConvertRawNumber(Value value) {
			if (m_typing == Typing.Weak || value.Type == ValueKind.Number || value.Type == ValueKind.Boolean) {
				if (value.Type == ValueKind.String || value.Type == ValueKind.Address) {
					if (double.TryParse(value.RawString, out double result)) {
						return result;
					}
				} else {
					return value.RawNumber;
				}
			}
			m_logger.LogError(new ConversionException(value.Type, ValueKind.Number));
			return default;
		}
		private string ConvertRawString(Value value) {
			if (m_typing == Typing.Weak || value.Type == ValueKind.String || value.Type == ValueKind.Address) {
				if (value.Type == ValueKind.Number) {
					return value.RawNumber.ToString();
				}
				if (value.Type == ValueKind.Boolean) {
					return value.RawNumber > 0 ? "true" : "false";
				}
				return value.RawString;
			}
			m_logger.LogError(new ConversionException(value.Type, ValueKind.String));
			return default;
		}

	}





}