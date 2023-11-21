namespace StrangeCalc;

public sealed class Lexer
{
    private readonly string _filename;
    private readonly string _source;
    private CodePosition _position;
    private char? _currentChar;

    public Lexer(string source, string filename)
    {
        _filename = filename;
        _source = source;
        _position = new CodePosition(-1, 0, 0, _filename, _source);

        Advance();
    }

    public void Advance()
    {
        _position.Advance(_currentChar);
        _currentChar = _position.Index < _source.Length ? _source[_position.Index] : null;
    }

    public Token NextToken()
    {
        Token token = NextTokenInternal();
#if DEBUG
        Console.WriteLine(token);
#endif
        return token;
    }

    public Token NextTokenInternal()
    {
        if (_currentChar is null) return new Token(TokenType.EOF, string.Empty, _position);
        char c = _currentChar.Value;

        Token? result;

        while (char.IsWhiteSpace(c))
        {
            Advance();
            if (_currentChar is null) return new Token(TokenType.EOF, string.Empty, _position);
            c = _currentChar.Value;
        }

        if (char.IsDigit(c))
            return ParseNumber();

        else if (char.IsLetter(c) || c == '_')
            return ParseIdentifier();

        else if (c == '"')
            return ParseString();

        else if (c == '+')
            return ParsePlus();

        else if (c == '-')
            return ParseMinus();

        else if (c == '*')
            result = new Token(TokenType.Multiply, "*", _position);

        else if (c == '/')
        {
            var tmp = ParseComment();
            if (tmp != null) return tmp;
            return NextTokenInternal();
        }

        else if (c == '(')
            result = new Token(TokenType.LeftParenthesis, "(", _position);

        else if (c == ')')
            result = new Token(TokenType.RightParenthesis, ")", _position);

        else if (c == ';')
            result = new Token(TokenType.Semicolon, ";", _position);

        else if (c == '=')
            result = ParseEquals();

        else if (c == '!')
            result = ParseNotEquals();

        else if (c == '>')
            result = ParseGreaterThan();

        else if (c == '<')
            result = ParseLessThan();

        else if (c == ',')
            result = new Token(TokenType.Comma, ",", _position);

        else if (c == '&')
            result = ParseAnd();

        else if (c == '|')
            result = ParseOr();

        else if (c == '^')
            result = new Token(TokenType.BitwiseXor, "^", _position);

        else if (c == '~')
            result = new Token(TokenType.BitwiseNot, "~", _position);

        else if (c == '%')
            result = new Token(TokenType.Modulo, "%", _position);

        else if (c == '{')
            result = new Token(TokenType.LeftBrace, "{", _position);

        else if (c == '}')
            result = new Token(TokenType.RightBrace, "}", _position);

        else if (c == '[')
            result = new Token(TokenType.LeftBracket, "[", _position);

        else if (c == ']')
            result = new Token(TokenType.RightBracket, "]", _position);

        else if (c == '.')
            result = new Token(TokenType.Dot, ".", _position);

        else if (c == ':')
            result = new Token(TokenType.Colon, ":", _position);

        else throw new Exception($"Unexpected character: {c}");

        if (result != null)
        {
            Advance();
            return result;
        }
        else
        {
            throw new Exception($"Unexpected character: {c}");
        }
    }

    private Token ParsePlus()
    {
        var posStart = _position.Copy();
        Advance();

        if (_currentChar == '+')
        {
            Advance();
            return new Token(TokenType.Increment, "++", posStart, _position);
        }

        return new Token(TokenType.Plus, "+", posStart);
    }

    private Token ParseMinus()
    {
        var posStart = _position.Copy();
        Advance();

        if (_currentChar == '-')
        {
            Advance();
            return new Token(TokenType.Decrement, "--", posStart, _position);
        }

        return new Token(TokenType.Minus, "-", posStart);
    }

    private Token ParseIdentifier()
    {
        string id = "";
        var posStart = _position.Copy();

        while (_currentChar != null && (char.IsLetterOrDigit(_currentChar.Value) || _currentChar.Value == '_'))
        {
            id += _currentChar;
            Advance();
        }

        return new Token(TokenType.Identifier, id, posStart, _position);
    }

    private Token ParseNumber()
    {
        string number = "";
        int dot_count = 0;
        var posStart = _position.Copy();

        while (_currentChar != null && (char.IsDigit(_currentChar.Value) || _currentChar.Value == '.' || _currentChar.Value == '_'))
        {
            if (_currentChar == '.')
            {
                if (dot_count++ == 1) break;
                number += '.';
            }
            else if (_currentChar != '_')
            {
                number += _currentChar;
            }
            Advance();
        }

        return new Token(TokenType.Number, number, posStart, _position);
    }

    private Token ParseString()
    {
        string str = "";
        var posStart = _position.Copy();
        bool escape = false;
        Advance();

        Dictionary<char, char> escapeDict = new()
        {
            { 'n', '\n' },
            { 'r', '\r' },
            { 't', '\t' },
            { 'b', '\b' },
            { 'a', '\a' },
            { 'f', '\f' },
            { 'v', '\v' },
            { '\\', '\\' },
            { '"', '"' },
            { '\'', '\'' },
            { '0', '\0' },
        };

        while (_currentChar != null && (_currentChar != '"' || escape))
        {
            if (escape)
            {
                if (escapeDict.ContainsKey(_currentChar.Value))
                {
                    str += escapeDict[_currentChar.Value];
                }
                else if (_currentChar.Value == 'x' || _currentChar.Value == 'u')
                {
                    int max = _currentChar.Value == 'x' ? 2 : 4;
                    int count = 0;
                    string hex = "";
                    while (count++ < max)
                    {
                        Advance();
                        if (_currentChar is null || !char.IsLetterOrDigit(_currentChar.Value)) break;
                        hex += _currentChar;
                    }
                    if (hex.Length == 0) throw new Exception($"Invalid escape character: {_currentChar}");
                    str += (char)Convert.ToInt32(hex, 16);
                }
                else
                {
                    throw new Exception($"Invalid escape character: {_currentChar}");
                }
            }
            else
            {
                if (_currentChar == '\\')
                {
                    escape = true;
                    Advance();
                    continue;
                }
                else
                {
                    str += _currentChar;
                }
            }
            Advance();
            escape = false;
        }

        Advance();
        return new Token(TokenType.String, str, posStart, _position);
    }

    private Token? ParseComment()
    {
        var posStart = _position.Copy();
        Advance();

        if (_currentChar == '/')
        {
            do
            {
                Advance();
            } while (_currentChar != '\n' && _currentChar != null);
            return null;
        }
        else if (_currentChar == '*')
        {
            Advance();
            while (_currentChar != null)
            {
                if (_currentChar == '*')
                {
                    Advance();
                    if (_currentChar == '/')
                    {
                        Advance();
                        return null;
                    }
                }
                else Advance();
            }
            throw new Exception("Unclosed comment");
        }

        return new Token(TokenType.Divide, "/", posStart);
    }

    private Token ParseEquals()
    {
        var posStart = _position.Copy();
        Advance();

        if (_currentChar == '=')
        {
            Advance();
            return new Token(TokenType.Equals, "==", posStart, _position);
        }
        else
        {
            // go back
            _position = posStart.Copy();
        }

        return new Token(TokenType.Assign, "=", posStart);
    }

    private Token ParseNotEquals()
    {
        var posStart = _position.Copy();
        Advance();

        if (_currentChar == '=')
        {
            Advance();
            return new Token(TokenType.NotEquals, "!=", posStart, _position);
        }
        else
        {
            // go back
            _position = posStart.Copy();
        }

        return new Token(TokenType.Not, "!", posStart);
    }

    private Token ParseGreaterThan()
    {
        var posStart = _position.Copy();
        Advance();

        if (_currentChar == '=')
        {
            Advance();
            return new Token(TokenType.GreaterThanOrEqual, ">=", posStart, _position);
        }
        else
        {
            // go back
            _position = posStart.Copy();
        }

        return new Token(TokenType.GreaterThan, ">", posStart);
    }

    private Token ParseLessThan()
    {
        var posStart = _position.Copy();
        Advance();

        if (_currentChar == '=')
        {
            Advance();
            return new Token(TokenType.LessThanOrEqual, "<=", posStart, _position);
        }
        else
        {
            // go back
            _position = posStart.Copy();
        }

        return new Token(TokenType.LessThan, "<", posStart);
    }

    private Token ParseAnd()
    {
        var posStart = _position.Copy();
        Advance();

        if (_currentChar == '&')
        {
            Advance();
            return new Token(TokenType.And, "&&", posStart, _position);
        }
        else
        {
            // go back
            _position = posStart.Copy();
        }

        return new Token(TokenType.BitwiseAnd, "&", posStart);
    }

    private Token ParseOr()
    {
        var posStart = _position.Copy();
        Advance();

        if (_currentChar == '|')
        {
            Advance();
            return new Token(TokenType.Or, "||", posStart, _position);
        }
        else
        {
            // go back
            _position = posStart.Copy();
        }

        return new Token(TokenType.BitwiseOr, "|", posStart);
    }

}