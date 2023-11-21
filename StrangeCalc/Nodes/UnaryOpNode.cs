using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrangeCalc.Nodes;

public sealed class UnaryOpNode : INode
{
	public Token Token { get; }
	public INode Expr { get; }

	public CodePosition Start => Token.Start;
	public CodePosition End => Expr.End;

	public UnaryOpNode(Token token, INode expr)
	{
		Token = token;
		Expr = expr;
	}

	public override string ToString() => $"UnaryOpNode({Token}, {Expr})";
}