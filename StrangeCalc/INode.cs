namespace StrangeCalc;

public interface INode
{
	public CodePosition Start { get; }
	public CodePosition End { get; }
}
