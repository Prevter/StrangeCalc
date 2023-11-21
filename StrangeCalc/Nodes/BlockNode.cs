using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrangeCalc.Nodes;

public sealed class BlockNode : INode
{
	public IEnumerable<INode> Statements { get; }

	public CodePosition Start => Statements.Any() ? Statements.First().Start : null;
	public CodePosition End => Statements.Any() ? Statements.Last().End : null;

	public BlockNode(IEnumerable<INode> statements)
	{
		Statements = statements;
	}

	public override string ToString() => $"BlockNode({string.Join(", ", Statements)})";
}
