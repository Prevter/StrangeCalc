using StrangeCalc.Nodes;

namespace StrangeCalc;

public sealed class Parser
{
    public Lexer Lexer { get; }
    private Token _currentToken;

    public Parser(Lexer lexer)
    {
        Lexer = lexer;
        _currentToken = Lexer.NextToken();
    }

    public IEnumerable<INode> Parse()
    {
        var result = new List<INode>();

        while (_currentToken.Type != TokenType.EOF)
        {
            result.Add(ParseIfStatement());
        }

        return result;
    }

    private INode ParseForLoop()
    {
        if (_currentToken.Type == TokenType.Identifier && _currentToken.Value == "for")
        {
            Eat(TokenType.Identifier);
            Eat(TokenType.LeftParenthesis);
            var init = ParseExpression();
            Eat(TokenType.Semicolon);
            var condition = ParseExpression();
            Eat(TokenType.Semicolon);
            var increment = ParseExpression();
            Eat(TokenType.RightParenthesis);
            var body = ParseCodeBlock();

            return new ForNode(init, condition, increment, body);
        }

        return ParseIfStatement();
    }

    private INode ParseIfStatement()
    {
        if (_currentToken.Type == TokenType.Identifier && _currentToken.Value == "if")
        {
            Eat(TokenType.Identifier);
            Eat(TokenType.LeftParenthesis);
            var condition = ParseExpression();
            Eat(TokenType.RightParenthesis);
            var body = ParseCodeBlock();

            List<(INode condition, INode body)> conditions = new()
            {
                (condition, body)
            };
            INode? elseBody = null;

            while (_currentToken.Type == TokenType.Identifier && _currentToken.Value == "else")
            {
                Eat(TokenType.Identifier);

                if (_currentToken.Type == TokenType.Identifier && _currentToken.Value == "if")
                {
                    Eat(TokenType.Identifier);
                    Eat(TokenType.LeftParenthesis);
                    var elifCondition = ParseExpression();
                    Eat(TokenType.RightParenthesis);
                    var elifBody = ParseCodeBlock();
                    conditions.Add((elifCondition, elifBody));
                }
                else
                {
                    elseBody = ParseCodeBlock();
                    break;
                }
            }

            return new IfNode(conditions, elseBody);
        }

        return ParseCodeBlock();
    }

    private INode ParseCodeBlock()
    {
        if (_currentToken.Type == TokenType.LeftBrace)
        {
            Eat(TokenType.LeftBrace);
            var result = new BlockNode(ParseStatements());
            Eat(TokenType.RightBrace);
            return result;
        }

        return ParseStatement();
    }

    private IEnumerable<INode> ParseStatements()
    {
        var result = new List<INode>();

        while (_currentToken.Type != TokenType.RightBrace)
        {
            result.Add(ParseStatement());
        }

        return result;
    }

    private INode ParseStatement()
    {
        INode result = null;
        var endTokens = new TokenType[] { TokenType.Semicolon, TokenType.EOF };

        if (_currentToken.Type == TokenType.Identifier && _currentToken.Value == "let")
        {
            Eat(TokenType.Identifier);
            var name = ParseIdentifier();
            Eat(TokenType.Assign);
            var value = ParseExpression();
            result = new AssignmentNode(name, value);
            // result = new VarNode(name, value);
            Eat(endTokens);
        }
        else if (_currentToken.Type == TokenType.Identifier && _currentToken.Value == "return")
        {
            Eat(TokenType.Identifier);
            var value = ParseExpression();
            // result = new ReturnNode(value);
            Eat(endTokens);
        }
        else if (_currentToken.Type == TokenType.Identifier && _currentToken.Value == "while")
        {
            Eat(TokenType.Identifier);
            Eat(TokenType.LeftParenthesis);
            var condition = ParseExpression();
            Eat(TokenType.RightParenthesis);
            var body = ParseCodeBlock();
            // result = new WhileNode(condition, body);
            Eat(endTokens);
        }
        else if (_currentToken.Type == TokenType.Identifier && _currentToken.Value == "break")
        {
            Eat(TokenType.Identifier);
            // result = new BreakNode();
            Eat(endTokens);
        }
        else if (_currentToken.Type == TokenType.Identifier && _currentToken.Value == "continue")
        {
            Eat(TokenType.Identifier);
            // result = new ContinueNode();
            Eat(endTokens);
        }
        else if (_currentToken.Type == TokenType.Identifier && _currentToken.Value == "func")
        {
            Eat(TokenType.Identifier);
            var name = ParseIdentifier();
            Eat(TokenType.LeftParenthesis);
            var args = ParseArguments();
            Eat(TokenType.RightParenthesis);
            var body = ParseCodeBlock();
            // result = new FunctionNode(name, args, body);
        }
        else if (_currentToken.Type == TokenType.Identifier && _currentToken.Value == "class")
        {
            Eat(TokenType.Identifier);
            var name = ParseIdentifier();
            Eat(TokenType.LeftBrace);
            var body = ParseStatements();
            Eat(TokenType.RightBrace);
            // result = new ClassNode(name, body);
        }
        else if (_currentToken.Type == TokenType.Identifier && _currentToken.Value == "new")
        {
            Eat(TokenType.Identifier);
            var name = ParseIdentifier();
            Eat(TokenType.LeftParenthesis);
            var args = ParseArguments();
            Eat(TokenType.RightParenthesis);
            // result = new NewNode(name, args);
            Eat(endTokens);
        }
        else if (_currentToken.Type == TokenType.Identifier && _currentToken.Value == "import")
        {
            Eat(TokenType.Identifier);
            var name = ParseIdentifier();
            // result = new ImportNode(name);
            Eat(endTokens);
        }
        else if (_currentToken.Type == TokenType.Identifier && _currentToken.Value == "for")
        {
            result = ParseForLoop();
        }
        else if (_currentToken.Type == TokenType.Identifier && _currentToken.Value == "if")
        {
            result = ParseIfStatement();
        }
        else
        {
            result = ParseExpression();
            Eat(endTokens);
        }

        return result;
    }

    private INode ParseExpression()
    {
        return ParseAssignment();
    }

    private INode ParseAssignment()
    {
        INode result = ParseBooleanMath();

        if (_currentToken.Type == TokenType.Assign)
        {
            Eat(TokenType.Assign);
            result = new AssignmentNode(result, ParseAssignment());
        }

        return result;
    }

    private INode ParseBooleanMath()
    {
        INode result = ParseComparison();

        while (CheckTokenTypes(_currentToken, new[] { TokenType.And, TokenType.Or }))
        {
            var token = _currentToken;
            Eat(_currentToken.Type);
            result = new BinOpNode(result, token, ParseComparison());
        }

        return result;
    }

    private INode ParseComparison()
    {
        INode result = ParseTerm();

        while (CheckTokenTypes(_currentToken, new[] { TokenType.Equals, TokenType.NotEquals, TokenType.GreaterThan, TokenType.LessThan, TokenType.GreaterThanOrEqual, TokenType.LessThanOrEqual }))
        {
            var token = _currentToken;
            Eat(_currentToken.Type);
            result = new BinOpNode(result, token, ParseTerm());
        }

        return result;
    }

    private INode ParseTerm()
    {
        INode result = ParseFactor();

        while (CheckTokenTypes(_currentToken, new[] {
            TokenType.Plus, TokenType.Minus,
            TokenType.BitwiseAnd, TokenType.BitwiseLeftShift,
            TokenType.BitwiseNot, TokenType.BitwiseOr,
            TokenType.BitwiseRightShift, TokenType.BitwiseXor
        }))
        {
            var token = _currentToken;
            Eat(_currentToken.Type);
            result = new BinOpNode(result, token, ParseFactor());
        }

        return result;
    }

    private INode ParseFactor()
    {
        INode result = ParsePower();

        while (CheckTokenTypes(_currentToken, new[] { TokenType.Multiply, TokenType.Divide, TokenType.Modulo }))
        {
            var token = _currentToken;
            Eat(_currentToken.Type);
            result = new BinOpNode(result, token, ParsePower());
        }

        return result;
    }

    private INode ParsePower()
    {
        INode result = ParseUnary();

        while (_currentToken.Type == TokenType.Power)
        {
            var token = _currentToken;
            Eat(_currentToken.Type);
            result = new BinOpNode(result, token, ParseUnary());
        }

        return result;
    }

    private INode ParseUnary()
    {
        if (CheckTokenTypes(_currentToken, new[] { TokenType.Plus, TokenType.Minus }))
        {
            var token = _currentToken;
            Eat(_currentToken.Type);
            return new UnaryOpNode(token, ParseUnary());
        }

        return ParseCall();
    }

    private INode ParseCall()
    {
        INode result = ParsePrimary();

        while (true)
        {
            if (_currentToken.Type == TokenType.LeftParenthesis)
            {
                Eat(TokenType.LeftParenthesis);
                result = new CallNode(result, ParseArguments());
                Eat(TokenType.RightParenthesis);
            }
            else
            {
                break;
            }
        }

        return result;
    }

    private IEnumerable<INode> ParseArguments()
    {
        var result = new List<INode>();

        if (_currentToken.Type != TokenType.RightParenthesis)
        {
            result.Add(ParseExpression());

            while (_currentToken.Type == TokenType.Comma)
            {
                Eat(TokenType.Comma);
                result.Add(ParseExpression());
            }
        }

        return result;
    }

    private INode ParsePrimary()
    {
        INode result;

        switch (_currentToken.Type)
        {
            case TokenType.String:
                result = ParseString();
                break;

            case TokenType.Number:
                result = ParseNumber();
                break;

            case TokenType.Identifier:
                result = ParseIdentifier();
                break;

            case TokenType.LeftParenthesis:
                Eat(TokenType.LeftParenthesis);
                result = ParseExpression();
                Eat(TokenType.RightParenthesis);
                break;

            default:
                throw UnexpectedToken(_currentToken);
        }

        return result;
    }

    private INode ParseString()
    {
        var result = new StringNode(_currentToken);
        Eat(TokenType.String);
        return result;
    }

    private INode ParseNumber()
    {
        var result = new NumberNode(_currentToken);
        Eat(TokenType.Number);
        return result;
    }

    private INode ParseIdentifier()
    {
        INode result = new IdentifierNode(_currentToken);
        Eat(TokenType.Identifier);

        if (CheckTokenTypes(_currentToken, new[] { TokenType.Increment, TokenType.Decrement }))
        {
            result = new AssignmentNode(result, new UnaryOpNode(_currentToken, result));
            Eat(_currentToken.Type);
        }

        return result;
    }

    private void Eat(IEnumerable<TokenType> types)
    {
        if (types.Contains(_currentToken.Type))
        {
            _currentToken = Lexer.NextToken();
        }
        else
        {
            throw UnexpectedToken(_currentToken);
        }
    }

    private void Eat(TokenType type)
    {
        if (_currentToken.Type == type)
        {
            _currentToken = Lexer.NextToken();
        }
        else
        {
            throw UnexpectedToken(_currentToken);
        }
    }

    public static bool CheckTokenTypes(Token token, IEnumerable<TokenType> types)
    {
        return types.Contains(token.Type);
    }

    private static Exception UnexpectedToken(Token token)
    {
        return new Exception($"Unexpected token: {token.Type} '{token.Value}' at position {token.Start}");
    }
}
