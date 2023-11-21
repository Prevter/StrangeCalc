using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrangeCalc.Nodes;

public sealed class NumberNode : INode
{
	public Token Token { get; }

	public CodePosition Start => Token.Start;
	public CodePosition End => Token.End;

	public NumberNode(Token token)
	{
		Token = token;
	}

	public override string ToString() => $"NumberNode({Token})";
}
