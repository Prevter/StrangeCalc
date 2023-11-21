namespace StrangeCalc.Nodes;

public sealed class IndexNode : INode
{
    public INode Value { get; set; }
    public INode Indexer { get; set; }

    public CodePosition Start => Value.Start;
    public CodePosition End => Indexer.End;

    public IndexNode(INode value, INode indexer)
    {
        Value = value;
        Indexer = indexer;
    }

    public override string ToString() => $"IndexNode({Value}, {Indexer})";
}
