namespace StrangeCalc;

public enum TokenType
{
	// Operators
	Plus, Minus,
	Multiply, Divide,
	Assign, Modulo, Power,
	Increment, Decrement,

	// Brackets
	LeftParenthesis, RightParenthesis,
	LeftBrace, RightBrace,
	LeftBracket, RightBracket,

	// Comparators
	NotEquals, Equals,
	GreaterThan, LessThan,
	GreaterThanOrEqual, LessThanOrEqual,

	// Boolean operators
	And, Or, Not,

	// Bitwise operators
	BitwiseAnd, BitwiseOr, BitwiseNot,
	BitwiseXor, BitwiseLeftShift, BitwiseRightShift,

	// Other
	Number,
	Identifier,
	String,

	// Special
	Comma, Dot, Colon, Semicolon, EOF
}