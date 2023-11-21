using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrangeCalc.Nodes;

public sealed class CallNode : INode
{
	public INode Node { get; }
	public IEnumerable<INode> Arguments { get; }

	public CodePosition Start => Node.Start;
	public CodePosition End => Arguments.Any() ? Arguments.Last().End : Node.End;

	public CallNode(INode node, IEnumerable<INode> arguments)
	{
		Node = node;
		Arguments = arguments;
	}

	public override string ToString() => $"CallNode({Node}{(Arguments.Any() ? (", " + string.Join(", ", Arguments)) : "")})";
}
