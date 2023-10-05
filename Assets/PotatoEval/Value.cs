using System;
using System.Globalization;

namespace PotatoEval {

	[Flags]
	public enum ValueKind : byte {
		Void = 0,
		Number = 1,
		Boolean = 2,
		String = 4,
		Address = 8,
		Any = 255
	}

	public struct Value : IEquatable<Value> {

		public static readonly Value Void = new Value();
		public static readonly Value True = true;
		public static readonly Value False = false;

		public ValueKind Type {
			get { return m_type; }
		}
		public string RawString { 
			get { return m_string; } 
		}
		public double RawNumber { 
			get { return m_number; } 
		}

		private ValueKind m_type;
		private uint m_hash;
		private double m_number;
		private string m_string;

		#region Constructors

		private Value(ValueKind type) {
			m_type = type;
			m_string = string.Empty;
			m_number = 0;
			if (type == ValueKind.Void) {
				m_hash = 0;
			} else {
				m_hash = Hash(m_type, m_string, m_number);
			}
		}
		private Value(ValueKind type, string value) {
			m_type = type;
			m_string = value;
			m_number = 0;
			m_hash = Hash(m_type, m_string, m_number);
		}
		private Value(string value) {
			m_type = ValueKind.String;
			m_string = value;
			m_number = 0;
			m_hash = Hash(m_type, m_string, m_number);
		}
		private Value(double value) {
			m_type = ValueKind.Number;
			m_string = string.Empty;
			m_number = value;
			m_hash = Hash(m_type, m_string, m_number);
		}
		private Value(bool value) {
			m_type = ValueKind.Boolean;
			m_string = string.Empty;
			m_number = value ? 1 : 0;
			m_hash = Hash(m_type, m_string, m_number);
		}
		private static uint Hash(ValueKind type, string str, double num) {
			int hash = 117;
			hash = (hash << 5) ^ type.GetHashCode();
			hash = (hash >> 3) ^ str.GetHashCode();
			hash = (hash << 7) ^ num.GetHashCode();
			return unchecked((uint)hash);
		}

		#endregion

		#region Operators

		public static bool operator ==(Value lhs, Value rhs) {
			return lhs.Equals(rhs);
		}
		public static bool operator !=(Value lhs, Value rhs) {
			return !lhs.Equals(rhs);
		}
		public static Value operator +(Value lhs, Value rhs) {
			return lhs.AsDouble + rhs.AsDouble;
		}
		public static Value operator -(Value lhs, Value rhs) {
			return lhs.AsDouble - rhs.AsDouble;
		}
		public static Value operator *(Value lhs, Value rhs) {
			return lhs.AsDouble * rhs.AsDouble;
		}
		public static Value operator /(Value lhs, Value rhs) {
			return lhs.AsDouble / rhs.AsDouble;
		}
		public static Value operator >(Value lhs, Value rhs) {
			return lhs.AsDouble > rhs.AsDouble;
		}
		public static Value operator >=(Value lhs, Value rhs) {
			return lhs.AsDouble >= rhs.AsDouble;
		}
		public static Value operator <(Value lhs, Value rhs) {
			return lhs.AsDouble < rhs.AsDouble;
		}
		public static Value operator <=(Value lhs, Value rhs) {
			return lhs.AsDouble <= rhs.AsDouble;
		}

		#endregion

		#region Implicit Conversions

		public static implicit operator Value(Address value) {
			return new Value(ValueKind.Address, value.ToString());
		}
		public static implicit operator Value(bool value) {
			return new Value(value);
		}
		public static implicit operator Value(float value) {
			return new Value(value);
		}
		public static implicit operator Value(double value) {
			return new Value(value);
		}
		public static implicit operator Value(int value) {
			return new Value(value);
		}
		public static implicit operator Value(string value) {
			return new Value(value);
		}
		public static implicit operator Value(byte value) {
			return new Value(value);
		}
		public static implicit operator Value(long value) {
			return new Value(value);
		}
		public static implicit operator Value(short value) {
			return new Value(value);
		}
		public static implicit operator Value(ushort value) {
			return new Value(value);
		}
		public static implicit operator Value(ulong value) {
			return new Value(value);
		}
		public static implicit operator Value(sbyte value) {
			return new Value(value);
		}
		public static implicit operator Value(uint value) {
			return new Value(value);
		}

		#endregion

		#region Type Checks

		public bool IsBool { get { return IsType(ValueKind.Boolean); } }
		public bool IsNumber { get { return IsType(ValueKind.Number); } }
		public bool IsString { get { return IsType(ValueKind.String); } }
		public bool IsVoid { get { return IsType(ValueKind.Void); } }
		public bool IsAddress { get { return IsType(ValueKind.Address); } }

		#endregion

		#region Type Conversions

		public Address AsAddress {
			get { return ValueConverter.StrongChecked.ToAddress(this); }
		}
		public bool AsBool {
			get { return ValueConverter.StrongChecked.ToBool(this); }
		}
		public string AsString {
			get { return ValueConverter.StrongChecked.ToString(this); }
		}
		public double AsDouble {
			get { return ValueConverter.StrongChecked.ToDouble(this); }
		}
		public float AsSingle {
			get { return ValueConverter.StrongChecked.ToSingle(this); }
		}
		public decimal AsDecimal {
			get { return ValueConverter.StrongChecked.ToDecimal(this); }
		}
		public int AsInt32 {
			get { return ValueConverter.StrongChecked.ToInt32(this); }
		}
		public short AsInt16 {
			get { return ValueConverter.StrongChecked.ToInt16(this); }
		}
		public long AsInt64 {
			get { return ValueConverter.StrongChecked.ToInt64(this); }
		}
		public uint AsUInt32 {
			get { return ValueConverter.StrongChecked.ToUInt32(this); }
		}
		public ushort AsUInt16 {
			get { return ValueConverter.StrongChecked.ToUInt16(this); }
		}
		public ulong AsUInt64 {
			get { return ValueConverter.StrongChecked.ToUInt64(this); }
		}
		public byte AsByte {
			get { return ValueConverter.StrongChecked.ToByte(this); }
		}
		public sbyte AsSByte {
			get { return ValueConverter.StrongChecked.ToSByte(this); }
		}

		#endregion

		public bool IsType(ValueKind type) {
			if (m_type == ValueKind.Void) {
				if (type == ValueKind.Void) {
					return true;
				}
				return false;
			}
			return (type & m_type) == m_type;
		}

		public bool Equals(Value other) {
			return other.m_hash == m_hash;
		}
		public override bool Equals(object obj) {
			if (obj is Value value) {
				return Equals(value);
			} else {
				return false;
			}
		}
		public override int GetHashCode() {
			return unchecked((int)m_hash);
		}
		public override string ToString() {
			switch (Type) {
				case ValueKind.Boolean: return m_number == 0 ? "false" : "true";
				case ValueKind.String:
				case ValueKind.Address: return m_string;
				case ValueKind.Number: return m_number.ToString(CultureInfo.InvariantCulture);
				case ValueKind.Void: return "void";
				default: throw new NotImplementedException();
			}
		}
	}


}