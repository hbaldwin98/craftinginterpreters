using cshlox;

public class Parser
{
    private class ParseError : Exception { }

    private List<Token> _tokens { get; }
    private int _current { get; set; } = 0;

    public Parser(List<Token> tokens)
    {
        _tokens = tokens;
    }

    public List<Statement> Parse()
    {
        List<Statement> statements = new List<Statement>();
        
        while (!IsAtEnd())
        {
            statements.Add(Statement());
        }

        return statements;
    }

    private Expression Expression()
    {
        return Equality();
    }

    private Statement Statement()
    {
        if (Match(TokenType.PRINT)) { return PrintStatement(); }

        return ExpressionStatement();
    }

    private Statement PrintStatement()
    {
        Expression value = Expression();
        Consume(TokenType.SEMICOLON, "Expect ';' after expression.");
        return new Print(value);
    }

    private Statement ExpressionStatement()
    {
        Expression expr = Expression();
        Consume(TokenType.SEMICOLON, "Expect ';' after expression.");
        return new StatementExpression(expr);
    }

    private Expression Equality()
    {
        Expression expr = Compare();

        while (Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
        {
            Token op = Previous();
            Expression right = Compare();
            expr = new Binary(expr, op, right);
        }

        return expr;
    }

    private Expression Compare()
    {
        Expression expr = Term();

        while (Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
        {
            Token op = Previous();
            Expression right = Term();
            expr = new Binary(expr, op, right);
        }

        return expr;
    }

    private Expression Term()
    {
        Expression expr = Factor();

        while (Match(TokenType.MINUS, TokenType.PLUS))
        {
            Token op = Previous();
            Expression right = Factor();
            expr = new Binary(expr, op, right);
        }

        return expr;
    }

    private Expression Factor()
    {
        Expression expr = Unary();

        while (Match(TokenType.SLASH, TokenType.STAR))
        {
            Token op = Previous();
            Expression right = Unary();
            expr = new Binary(expr, op, right);
        }

        return expr;
    }

    private Expression Unary()
    {
        if (Match(TokenType.BANG, TokenType.MINUS))
        {
            Token op = Previous();
            Expression right = Unary();
            return new Unary(op, right);
        }

        return Primary();
    }

    private Expression Primary()
    {
        if (Match(TokenType.FALSE)) { return new Literal(false); }
        if (Match(TokenType.TRUE)) { return new Literal(true); }
        if (Match(TokenType.NIL)) { return new Literal(null); }

        if (Match(TokenType.NUMBER, TokenType.STRING))
        {
            return new Literal(Previous().Literal);
        }

        if (Match(TokenType.LEFT_PAREN))
        {
            Expression expr = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
            return new Grouping(expr);
        }

        throw Error(Peek(), "Expect Expression.");
    }

    private bool Match(params TokenType[] types)
    {
        foreach (var type in types)
        {
            if (Check(type))
            {
                Advance();
                return true;
            }
        }

        return false;
    }

    private Token Consume(TokenType type, string message)
    {
        if (Check(type)) { return Advance(); }

        throw Error(Peek(), message);
    }

    private bool Check(TokenType type)
    {
        if (IsAtEnd()) { return false; }

        return Peek().Type == type;
    }

    private Token Advance()
    {
        if (!IsAtEnd()) { _current++; }

        return Previous();
    }

    private bool IsAtEnd()
    {
        return Peek().Type == TokenType.EOF;
    }

    private Token Peek()
    {
        return _tokens.ElementAt(_current);
    }

    private Token Previous()
    {
        return _tokens.ElementAt(_current - 1);
    }

    private ParseError Error(Token token, string message)
    {
        Cshlox.Error(token, message);

        return new ParseError();
    }

    private void Synchronize()
    {
        Advance();

        while (!IsAtEnd())
        {
            if (Previous().Type == TokenType.SEMICOLON) { return; }

            switch (Peek().Type)
            {
                case TokenType.CLASS:
                case TokenType.FOR:
                case TokenType.FUN:
                case TokenType.IF:
                case TokenType.PRINT:
                case TokenType.RETURN:
                case TokenType.VAR:
                case TokenType.WHILE:
                    return;
            }

            Advance();
        }
    }
}
