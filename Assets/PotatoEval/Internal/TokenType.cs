using System;

namespace PotatoEval {

	internal enum TokenType {
		Void,
		False,
		True,
		Identifier,
		Number,
		String,
		Period,
		Dollar,
		EqualEqual,
		BangEqual,
		RightEqual,
		LeftEqual,
		Right,
		Left,
		AmpersandAmpersand,
		PipePipe,
		Bang,
		Plus,
		Minus,
		Star,
		Slash,
		Percent,
		Ampersand,
		Pipe,
		Carret,
		Tilde,
		LeftLeft,
		RightRight,
		Comma,
		OpenParen,
		CloseParen,
		Question,
		Colon,

		Equal,
		PlusEqual,
		MinusEqual,
		StarEqual,
		SlashEqual,
		PercentEqual,
		AmpersandEqual,
		PipeEqual,
		CarretEqual,
		LeftLeftEqual,
		RightRightEqual,
	}

	internal static class TokenTypeExtensions {

		public static string ToSymbol(this TokenType type) {
			switch (type) {
				case TokenType.Void: return "void";
				case TokenType.False: return "false";
				case TokenType.True: return "true";
				case TokenType.Identifier: return "[identifier]";
				case TokenType.Number: return "[number]";
				case TokenType.String: return "[string]";
				case TokenType.Period: return ".";
				case TokenType.EqualEqual: return "==";
				case TokenType.BangEqual: return "!=";
				case TokenType.RightEqual: return ">=";
				case TokenType.LeftEqual: return "<=";
				case TokenType.Right: return ">";
				case TokenType.Left: return "<";
				case TokenType.AmpersandAmpersand: return "&&";
				case TokenType.PipePipe: return "||";
				case TokenType.Bang: return "!";
				case TokenType.Plus: return "+";
				case TokenType.Minus: return "-";
				case TokenType.Star: return "*";
				case TokenType.Slash: return "/";
				case TokenType.Percent: return "%";
				case TokenType.Ampersand: return "&";
				case TokenType.Pipe: return "|";
				case TokenType.Carret: return "^";
				case TokenType.Tilde: return "~";
				case TokenType.LeftLeft: return "<<";
				case TokenType.RightRight: return ">>";
				case TokenType.Comma: return ",";
				case TokenType.OpenParen: return "(";
				case TokenType.CloseParen: return ")";
				case TokenType.Question: return "?";
				case TokenType.Colon: return ":";

				case TokenType.Equal: return "=";
				case TokenType.PlusEqual: return "+=";
				case TokenType.MinusEqual: return "-=";
				case TokenType.StarEqual: return "*=";
				case TokenType.SlashEqual: return "/=";
				case TokenType.PercentEqual: return "%=";
				case TokenType.AmpersandEqual: return "&=";
				case TokenType.PipeEqual: return "|=";
				case TokenType.CarretEqual: return "^=";
				case TokenType.LeftLeftEqual: return "<<=";
				case TokenType.RightRightEqual: return ">>=";

				default: throw new NotImplementedException();
			}
		}

	}

}