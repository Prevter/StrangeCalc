using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrangeCalc.Nodes;

public sealed class BinOpNode : INode
{
	public INode Left { get; }
	public Token Token { get; }
	public INode Right { get; }

	public CodePosition Start => Left.Start;
	public CodePosition End => Right.End;

	public BinOpNode(INode left, Token token, INode right)
	{
		Left = left;
		Token = token;
		Right = right;
	}

	public override string ToString() => $"BinOpNode({Left}, {Token}, {Right})";
}
