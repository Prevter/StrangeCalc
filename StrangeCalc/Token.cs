using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrangeCalc;

public sealed class Token
{
	public TokenType Type { get; }
	public string Value { get; }
	public CodePosition? Start { get; }
	public CodePosition? End { get; }

	public Token(TokenType type, string value = "", CodePosition? start = null, CodePosition? end = null)
	{
		Type = type;
		Value = value;
		if (start != null)
		{
			Start = start.Copy();
			End = start.Copy();
			End.Advance();
		}
		if (end != null) End = end.Copy();
	}

	public override string ToString() => $"Token({Type}, {Value})";
}