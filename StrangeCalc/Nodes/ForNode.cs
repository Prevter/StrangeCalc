using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrangeCalc.Nodes;

public sealed class ForNode : INode
{
	public INode Init { get; }
	public INode Condition { get; }
	public INode Increment { get; }
	public INode Body { get; }

	public CodePosition Start => Init.Start;
	public CodePosition End => Body.End;

	public ForNode(INode init, INode condition, INode increment, INode body)
	{
		Init = init;
		Condition = condition;
		Increment = increment;
		Body = body;
	}

	public override string ToString()
	{
		return $"ForNode({Init}, {Condition}, {Increment}, {Body})";
	}
}
