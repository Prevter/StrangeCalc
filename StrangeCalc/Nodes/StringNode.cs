using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrangeCalc.Nodes;

public sealed class StringNode : INode
{
	public Token Token { get; }

	public CodePosition Start => Token.Start;
	public CodePosition End => Token.End;

	public StringNode(Token token)
	{
		Token = token;
	}

	public override string ToString() => $"StringNode({Token})";
}
