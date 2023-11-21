using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrangeCalc.Nodes;

public sealed class AssignmentNode : INode
{
	public INode Left { get; }
	public INode Right { get; }

	public CodePosition Start => Left.Start;
	public CodePosition End => Right.End;

	public AssignmentNode(INode left, INode right)
	{
		Left = left;
		Right = right;
	}

	public override string ToString() => $"AssignmentNode({Left}, {Right})";
}
