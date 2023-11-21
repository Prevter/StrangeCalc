using StrangeCalc.Nodes;
using System.Text;

namespace StrangeCalc;

public static class DebugUtils
{
    public static string PrintAST(INode node, int indent = 0)
    {
        return (node switch
        {
            BinOpNode binOpNode => VisitBinOpNode(binOpNode, indent),
            UnaryOpNode unaryOpNode => VisitUnaryOpNode(unaryOpNode, indent),
            NumberNode numberNode => VisitNumberNode(numberNode, indent),
            StringNode stringNode => VisitStringNode(stringNode, indent),
            IdentifierNode identifierNode => VisitIdentifierNode(identifierNode, indent),
            AssignmentNode assignmentNode => VisitAssignmentNode(assignmentNode, indent),
            CallNode callNode => VisitCallNode(callNode, indent),
            BlockNode blockNode => VisitBlockNode(blockNode, indent),
            IfNode ifNode => VisitIfNode(ifNode, indent),
            ForNode forNode => VisitForNode(forNode, indent),
            IndexNode indexNode => VisitIndexNode(indexNode, indent),
            _ => throw new Exception($"Unexpected node type: {node.GetType().Name}")
        }).TrimEnd();
    }

    private static string VisitBinOpNode(BinOpNode node, int indent)
    {
        var result = new StringBuilder();
        result.AppendLine($"{new string(' ', indent)}BinOpNode({node.Token}):");
        result.AppendLine(PrintAST(node.Left, indent + 1));
        result.AppendLine(PrintAST(node.Right, indent + 1));
        return result.ToString();
    }

    private static string VisitUnaryOpNode(UnaryOpNode node, int indent)
    {
        var result = new StringBuilder();
        result.AppendLine($"{new string(' ', indent)}UnaryOpNode({node.Token}):");
        result.AppendLine(PrintAST(node.Expr, indent + 1));
        return result.ToString();
    }

    private static string VisitNumberNode(NumberNode node, int indent)
    {
        return $"{new string(' ', indent)}NumberNode({node.Token})";
    }

    private static string VisitStringNode(StringNode node, int indent)
    {
        return $"{new string(' ', indent)}StringNode({node.Token})";
    }

    private static string VisitIdentifierNode(IdentifierNode node, int indent)
    {
        return $"{new string(' ', indent)}IdentifierNode({node.Token})";
    }

    private static string VisitAssignmentNode(AssignmentNode node, int indent)
    {
        var result = new StringBuilder();
        result.AppendLine($"{new string(' ', indent)}AssignmentNode:");
        result.AppendLine(PrintAST(node.Left, indent + 1));
        result.AppendLine(PrintAST(node.Right, indent + 1));
        return result.ToString();
    }

    private static string VisitCallNode(CallNode node, int indent)
    {
        var result = new StringBuilder();
        result.AppendLine($"{new string(' ', indent)}CallNode:");
        result.AppendLine(PrintAST(node.Node, indent + 1));
        foreach (var argument in node.Arguments)
        {
            result.AppendLine(PrintAST(argument, indent + 1));
        }
        return result.ToString();
    }

    private static string VisitBlockNode(BlockNode node, int indent)
    {
        var result = new StringBuilder();
        result.AppendLine($"{new string(' ', indent)}BlockNode:");
        foreach (var statement in node.Statements)
        {
            result.AppendLine(PrintAST(statement, indent + 1));
        }
        return result.ToString();
    }

    private static string VisitIfNode(IfNode node, int indent)
    {
        var result = new StringBuilder();
        result.AppendLine($"{new string(' ', indent)}IfNode:");
        foreach (var branch in node.Cases)
        {
            result.AppendLine(PrintAST(branch.Condition, indent + 1));
            result.AppendLine(PrintAST(branch.Body, indent + 1));
        }
        if (node.ElseBody is not null)
        {
            result.AppendLine(PrintAST(node.ElseBody, indent + 1));
        }
        return result.ToString();
    }

    private static string VisitForNode(ForNode node, int indent)
    {
        var result = new StringBuilder();
        result.AppendLine($"{new string(' ', indent)}ForNode:");
        result.AppendLine(PrintAST(node.Init, indent + 1));
        result.AppendLine(PrintAST(node.Condition, indent + 1));
        result.AppendLine(PrintAST(node.Increment, indent + 1));
        result.AppendLine(PrintAST(node.Body, indent + 1));
        return result.ToString();
    }

    private static string VisitIndexNode(IndexNode node, int indent)
    {
        var result = new StringBuilder();
        result.AppendLine($"{new string(' ', indent)}IndexNode:");
        result.AppendLine(PrintAST(node.Value, indent + 1));
        result.AppendLine(PrintAST(node.Indexer, indent + 1));
        return result.ToString();
    }
}
