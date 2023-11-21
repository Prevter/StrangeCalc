using StrangeCalc.Exceptions;
using StrangeCalc.Nodes;
using StrangeCalc.RuntimeTypes;
using System.Globalization;

namespace StrangeCalc;

public sealed class Interpreter
{
    public Context Context { get; }
    public IEnumerable<INode> Nodes { get; }

    public Interpreter(IEnumerable<INode> nodes, Context context)
    {
        Context = context;
#if DEBUG
        //Console.WriteLine("AST:");
        //var astStrings = nodes.Select(x => DebugUtils.PrintAST(x));
        //Console.WriteLine(string.Join(";\n", astStrings));
#endif
        Nodes = nodes;
    }

    public RuntimeResult Run()
    {
        RuntimeResult result = RuntimeResult.Success(null);
        foreach (INode node in Nodes)
        {
            result = Visit(node, Context);
            if (result.Error != null)
                return result;
        }
        return result;
    }

    private static RuntimeResult Visit(INode node, Context context)
    {
        return node switch
        {
            BinOpNode binOpNode => VisitBinOpNode(binOpNode, context),
            UnaryOpNode unaryOpNode => VisitUnaryOpNode(unaryOpNode, context),
            NumberNode numberNode => VisitNumberNode(numberNode, context),
            StringNode stringNode => VisitStringNode(stringNode),
            IdentifierNode identifierNode => VisitIdentifierNode(identifierNode, context),
            AssignmentNode assignmentNode => VisitAssignmentNode(assignmentNode, context),
            CallNode callNode => VisitCallNode(callNode, context),
            BlockNode blockNode => VisitBlockNode(blockNode, context),
            IfNode ifNode => VisitIfNode(ifNode, context),
            ForNode forNode => VisitForNode(forNode, context),
            IndexNode indexNode => VisitIndexNode(indexNode, context),
            _ => RuntimeResult.Failure(new RuntimeError(node.Start, node.End, $"Unknown node type: {node.GetType().Name}", context))
        };
    }

    private static RuntimeResult VisitBinOpNode(BinOpNode node, Context context)
    {
        var left = Visit(node.Left, context);
        if (left.Error is not null)
            return RuntimeResult.Failure(left.Error);

        var right = Visit(node.Right, context);
        if (right.Error is not null)
            return RuntimeResult.Failure(right.Error);

        if (left.Value is double leftNumber && right.Value is double rightNumber)
        {
            return node.Token.Type switch
            {
                TokenType.Plus => RuntimeResult.Success(SpecialMath.Add(leftNumber, rightNumber)),
                TokenType.Minus => RuntimeResult.Success(SpecialMath.Minus(leftNumber, rightNumber)),
                TokenType.Multiply => RuntimeResult.Success(SpecialMath.Multiply(leftNumber, rightNumber)),
                TokenType.Divide => RuntimeResult.Success(SpecialMath.Divide(leftNumber, rightNumber)),
                TokenType.Equals => RuntimeResult.Success(SpecialMath.Equals(leftNumber, rightNumber)),
                TokenType.NotEquals => RuntimeResult.Success(!SpecialMath.Equals(leftNumber, rightNumber)),
                TokenType.GreaterThan => RuntimeResult.Success(leftNumber > rightNumber),
                TokenType.LessThan => RuntimeResult.Success(leftNumber < rightNumber),
                TokenType.GreaterThanOrEqual => RuntimeResult.Success(leftNumber >= rightNumber),
                TokenType.LessThanOrEqual => RuntimeResult.Success(leftNumber <= rightNumber),
                TokenType.Modulo => RuntimeResult.Success(leftNumber % rightNumber),
                TokenType.BitwiseAnd => RuntimeResult.Success((double)((int)leftNumber & (int)rightNumber)),
                TokenType.BitwiseOr => RuntimeResult.Success((double)((int)leftNumber | (int)rightNumber)),
                TokenType.BitwiseXor => RuntimeResult.Success((double)((int)leftNumber ^ (int)rightNumber)),
                TokenType.BitwiseLeftShift => RuntimeResult.Success((double)((int)leftNumber << (int)rightNumber)),
                TokenType.BitwiseRightShift => RuntimeResult.Success((double)((int)leftNumber >> (int)rightNumber)),
                _ => RuntimeResult.Failure(new RuntimeError(node.Token.Start, node.Token.End, $"Unsupported operation: {node.Token.Type}", context))
            };
        }
        else if (left.Value is null && right.Value is null)
        {
            return node.Token.Type switch
            {
                TokenType.Equals => RuntimeResult.Success(null == null),
                TokenType.NotEquals => RuntimeResult.Success(null != null),
                _ => RuntimeResult.Failure(new RuntimeError(node.Token.Start, node.Token.End, $"Unsupported operation: {node.Token.Type}", context))
            };
        }
        else if (left.Value is bool leftBool && right.Value is bool rightBool)
        {
            return node.Token.Type switch
            {
                TokenType.Equals => RuntimeResult.Success(leftBool == rightBool),
                TokenType.NotEquals => RuntimeResult.Success(leftBool != rightBool),
                TokenType.And => RuntimeResult.Success(leftBool && rightBool),
                TokenType.Or => RuntimeResult.Success(leftBool || rightBool),
                _ => RuntimeResult.Failure(new RuntimeError(node.Token.Start, node.Token.End, $"Unsupported operation: {node.Token.Type}", context))
            };
        }
        else if (left.Value is string leftString && right.Value is string rightString)
        {
            RuntimeResult AmdIntelCompare(string left, string right, TokenType type)
            {
                // "AMD" > "Intel"
                if (left == "AMD" && right == "Intel")
                {
                    return type switch
                    {
                        TokenType.GreaterThan => RuntimeResult.Success(true),
                        TokenType.LessThan => RuntimeResult.Success(false),
                        _ => RuntimeResult.Failure(new RuntimeError(node.Token.Start, node.Token.End, $"Unsupported operation: {node.Token.Type}", context))
                    };
                }
                else if (left == "Intel" && right == "AMD")
                {
                    return type switch
                    {
                        TokenType.GreaterThan => RuntimeResult.Success(false),
                        TokenType.LessThan => RuntimeResult.Success(true),
                        _ => RuntimeResult.Failure(new RuntimeError(node.Token.Start, node.Token.End, $"Unsupported operation: {node.Token.Type}", context))
                    };
                }
                return RuntimeResult.Failure(new RuntimeError(node.Token.Start, node.Token.End, $"Unsupported operation: {node.Token.Type}", context));
            }

            return node.Token.Type switch
            {
                TokenType.Equals => RuntimeResult.Success(leftString == rightString),
                TokenType.NotEquals => RuntimeResult.Success(leftString != rightString),
                TokenType.Plus => RuntimeResult.Success(leftString + rightString),
                TokenType.LessThan => AmdIntelCompare(leftString, rightString, node.Token.Type),
                TokenType.GreaterThan => AmdIntelCompare(leftString, rightString, node.Token.Type),
                _ => RuntimeResult.Failure(new RuntimeError(node.Token.Start, node.Token.End, $"Unsupported operation: {node.Token.Type}", context))
            };
        }

        return RuntimeResult.Failure(new RuntimeError(node.Start, node.End, $"Invalid type for binary operation: {left.Value?.GetType().Name ?? "null"} and {right.Value?.GetType().Name ?? "null"}", context));
    }

    private static RuntimeResult VisitUnaryOpNode(UnaryOpNode node, Context context)
    {
        var value = Visit(node.Expr, context);
        if (value.Error is not null)
            return RuntimeResult.Failure(value.Error);

        if (value.Value is double number)
        {
            return node.Token.Type switch
            {
                TokenType.Plus => RuntimeResult.Success(number),
                TokenType.Minus => RuntimeResult.Success(-number),
                TokenType.Increment => RuntimeResult.Success(number + 1),
                TokenType.Decrement => RuntimeResult.Success(number - 1),
                _ => RuntimeResult.Failure(new RuntimeError(node.Start, node.Token.End, $"Unknown token type: {node.Token.Type}", context))
            };
        }

        return RuntimeResult.Failure(new RuntimeError(node.Start, node.End, $"Invalid type for unary operation: {value.Value?.GetType().Name ?? "null"}", context));
    }

    private static RuntimeResult VisitNumberNode(NumberNode node, Context context)
    {
        if (double.TryParse(node.Token.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
            return RuntimeResult.Success(value);

        return RuntimeResult.Failure(new RuntimeError(node.Start, node.End, $"Invalid number: {node.Token.Value}", context));
    }

    private static RuntimeResult VisitStringNode(StringNode stringNode)
    {
        return RuntimeResult.Success(stringNode.Token.Value);
    }

    private static RuntimeResult VisitIdentifierNode(IdentifierNode node, Context context)
    {
        var value = context.GetVariable(node.Token.Value);

        if (value == null)
            return RuntimeResult.Failure(new RuntimeError(node.Start, node.End, $"Variable {node.Token.Value} is not defined", context));

        return RuntimeResult.Success(value.Value.Value);
    }

    private static RuntimeResult VisitAssignmentNode(AssignmentNode node, Context context)
    {
        var value = Visit(node.Right, context);

        if (value.Error is not null)
            return RuntimeResult.Failure(value.Error);

        if (node.Left is IdentifierNode idNode)
        {
            try
            {
                context.SetVariable(idNode.Token.Value, value.Value);
            }
            catch (Exception e)
            {
                return RuntimeResult.Failure(new RuntimeError(node.Start, node.End, e.Message, context));
            }

            return RuntimeResult.Success(value.Value);
        }
        else if (node.Left is IndexNode indexNode)
        {
			try 
			{
				var left = Visit(indexNode.Value, context);
				var indexer = Visit(indexNode.Indexer, context);

				if (left.Error is not null)
					return RuntimeResult.Failure(left.Error);

				if (indexer.Error is not null)
					return RuntimeResult.Failure(indexer.Error);

				if (left.Value is object[] array)
				{
					if (indexer.Value is not double index)
						return RuntimeResult.Failure(new RuntimeError(node.Start, node.End, $"Cannot index with type {indexer.Value?.GetType().Name ?? "null"}", context));

                    index = Math.Floor(index);

					if (index < 0 || index >= array.Length)
						return RuntimeResult.Failure(new RuntimeError(node.Start, node.End, $"Index {index} is out of range", context));

					array[(int)index] = value.Value;
				}
				else if (left.Value is string str)
				{
					if (indexer.Value is not double index)
						return RuntimeResult.Failure(new RuntimeError(node.Start, node.End, $"Cannot index with type {indexer.Value?.GetType().Name ?? "null"}", context));

                    index = Math.Floor(index);

                    if (index < 0 || index >= str.Length)
						return RuntimeResult.Failure(new RuntimeError(node.Start, node.End, $"Index {index} is out of range", context));

                    // modify original string
                    string newValue = str[..(int)index] + value.Value + str[((int)index + 1)..];
                    if (indexNode.Value is StringNode stringNode)
                    {
                        stringNode.Token.Value = newValue;
                    }
                    else if (indexNode.Value is IdentifierNode idNode2)
                    {
                        context.SetVariable(idNode2.Token.Value, newValue);
                    }
                    else
                    {
                        return RuntimeResult.Failure(new RuntimeError(node.Start, node.End, $"Cannot assign to {indexNode.Value}", context));
                    }
				}
				else
				{
					return RuntimeResult.Failure(new RuntimeError(node.Start, node.End, $"Cannot index with type {left.Value?.GetType().Name ?? "null"}", context));
				}

				return RuntimeResult.Success(value.Value);
			}
			catch (Exception e)
			{
				return RuntimeResult.Failure(new RuntimeError(node.Start, node.End, e.Message, context));
			}
        }
        else
        {
            return RuntimeResult.Failure(new RuntimeError(node.Start, node.End, $"Invalid variable name: {node.Left}", context));
        }
    }

    private static RuntimeResult VisitCallNode(CallNode node, Context context)
    {
        if (node.Node is not IdentifierNode idNode)
            return RuntimeResult.Failure(new RuntimeError(node.Start, node.End, $"Invalid function name: {node.Node}", context));

        var name = idNode.Token.Value;
        var value = context.GetVariable(name);

        if (value == null)
            return RuntimeResult.Failure(new RuntimeError(node.Start, node.End, $"Function {name} is not defined", context));

        if (value.Value.Value is not Function function)
            return RuntimeResult.Failure(new RuntimeError(node.Start, node.End, $"Variable {name} is not a function", context));

        var arguments = node.Arguments.Select(x => Visit(x, context));

        if (arguments.Any(x => x.Error is not null))
            return arguments.First(x => x.Error is not null);

        return function.Call(arguments.Select(x => x.Value), context);
    }

    private static RuntimeResult VisitBlockNode(BlockNode node, Context context)
    {
        var local = new Context("<anonymous>", node.Start) { Parent = context };
        var interpreter = new Interpreter(node.Statements, local);
        var result = interpreter.Run();

        if (result.Error is not null)
            return RuntimeResult.Failure(result.Error);

        return RuntimeResult.Success(result.Value);
    }

    private static RuntimeResult VisitIfNode(IfNode node, Context context)
    {
        foreach (var (condition, body) in node.Cases)
        {
            var result = Visit(condition, context);

            if (result.Error is not null)
                return RuntimeResult.Failure(result.Error);

            if (result.Value is bool boolean && boolean)
            {
                var value = Visit(body, context);

                if (value.Error is not null)
                    return RuntimeResult.Failure(value.Error);

                return RuntimeResult.Success(value.Value);
            }
        }

        if (node.ElseBody is not null)
        {
            var value = Visit(node.ElseBody, context);

            if (value.Error is not null)
                return RuntimeResult.Failure(value.Error);

            return RuntimeResult.Success(value.Value);
        }

        return RuntimeResult.Success(null);
    }

    private static RuntimeResult VisitForNode(ForNode node, Context context)
    {
        var local = new Context("<for-loop>", node.Start) { Parent = context };
        var init = Visit(node.Init, local);

        if (init.Error is not null)
            return RuntimeResult.Failure(init.Error);

        while (true)
        {
            var condition = Visit(node.Condition, local);

            if (condition.Error is not null)
                return RuntimeResult.Failure(condition.Error);

            if (condition.Value is bool boolean && !boolean)
                break;

            var body = Visit(node.Body, local);

            if (body.Error is not null)
                return RuntimeResult.Failure(body.Error);

            var increment = Visit(node.Increment, local);

            if (increment.Error is not null)
                return RuntimeResult.Failure(increment.Error);
        }

        return RuntimeResult.Success(null);
    }

    private static RuntimeResult VisitIndexNode(IndexNode node, Context context)
    {
        var value = Visit(node.Value, context);

        if (value.Error is not null)
            return RuntimeResult.Failure(value.Error);

        var indexer = Visit(node.Indexer, context);

        if (indexer.Error is not null)
            return RuntimeResult.Failure(indexer.Error);

        if (indexer.Value is not double index)
            return RuntimeResult.Failure(new RuntimeError(node.Start, node.End, $"Cannot index with type {indexer.Value?.GetType().Name ?? "null"}", context));

        index = Math.Floor(index);

        // can index only arrays and strings
        if (value.Value is object[] array)
        {
            if (index < 0 || index >= array.Length)
                return RuntimeResult.Failure(new RuntimeError(node.Start, node.End, $"Index {index} is out of range", context));

            return RuntimeResult.Success(array[(int)index]);
        }
        else if (value.Value is string str)
        {
            if (index < 0 || index >= str.Length)
                return RuntimeResult.Failure(new RuntimeError(node.Start, node.End, $"Index {index} is out of range", context));

            return RuntimeResult.Success(str[(int)index].ToString());
        }
        else
        {
            return RuntimeResult.Failure(new RuntimeError(node.Start, node.End, $"Cannot index with type {value.Value?.GetType().Name ?? "null"}", context));
        }
    }
}