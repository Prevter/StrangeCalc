using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrangeCalc.Nodes;

public sealed class IfNode : INode
{
	public IEnumerable<(INode Condition, INode Body)> Cases { get; }
	public INode? ElseBody { get; }

	public CodePosition Start { get; }
	public CodePosition End { get; }

	public IfNode(IEnumerable<(INode Condition, INode Body)> cases, INode? elseBody = null)
	{
		Cases = cases;
		ElseBody = elseBody;

		Start = Cases.First().Condition.Start;
		End = ElseBody?.End ?? Cases.Last().Body.End;
	}
}
