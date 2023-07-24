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

    public List<Stmt> Parse()
    {
        List<Stmt> statements = new List<Stmt>();

        while (!IsAtEnd())
        {
            statements.Add(Declaration());
        }

        return statements;
    }

    private Expr Expr()
    {
        return Equality();
    }

    private Stmt Declaration()
    {
        try
        {
            if (Match(TokenType.VAR)) { return VarDeclaration(); }

            return Stmt();
        }
        catch (ParseError)
        {
            Synchronize();
            return null;
        }
    }

    private Stmt Stmt()
    {
        if (Match(TokenType.PRINT)) { return PrintStatement(); }

        return ExpressionStatement();
    }

    private Stmt PrintStatement()
    {
        Expr value = Expr();
        Consume(TokenType.SEMICOLON, "Expect ';' after expression.");
        return new Stmt.Print(value);
    }

    private Stmt VarDeclaration()
    {
        Token name = Consume(TokenType.IDENTIFIER, "Expect variable name.");

        Expr initializer = null;

        if (Match(TokenType.EQUAL))
        {
            initializer = Expr();
        }

        Consume(TokenType.SEMICOLON, "Expect ';' after variable declaration.");

        return new Stmt.Var(name, initializer);
    }

    private Stmt ExpressionStatement()
    {
        Expr expr = Expr();

        Consume(TokenType.SEMICOLON, "Expect ';' after expression.");
        return new Stmt.Expression(expr);
    }

    private Expr Equality()
    {
        Expr expr = Compare();

        while (Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
        {
            Token op = Previous();
            Expr right = Compare();
            expr = new Expr.Binary(expr, op, right);
        }

        return expr;
    }

    private Expr Compare()
    {
        Expr expr = Term();

        while (Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
        {
            Token op = Previous();
            Expr right = Term();
            expr = new Expr.Binary(expr, op, right);
        }

        return expr;
    }

    private Expr Term()
    {
        Expr expr = Factor();

        while (Match(TokenType.MINUS, TokenType.PLUS))
        {
            Token op = Previous();
            Expr right = Factor();
            expr = new Expr.Binary(expr, op, right);
        }

        return expr;
    }

    private Expr Factor()
    {
        Expr expr = Unary();

        while (Match(TokenType.SLASH, TokenType.STAR))
        {
            Token op = Previous();
            Expr right = Unary();
            expr = new Expr.Binary(expr, op, right);
        }

        return expr;
    }

    private Expr Unary()
    {
        if (Match(TokenType.BANG, TokenType.MINUS))
        {
            Token op = Previous();
            Expr right = Unary();
            return new Expr.Unary(op, right);
        }

        return Primary();
    }

    private Expr Primary()
    {
        if (Match(TokenType.FALSE)) { return new Expr.Literal(false); }
        if (Match(TokenType.TRUE)) { return new Expr.Literal(true); }
        if (Match(TokenType.NIL)) { return new Expr.Literal(null); }

        if (Match(TokenType.NUMBER, TokenType.STRING))
        {
            return new Expr.Literal(Previous().Literal);
        }

        if (Match(TokenType.IDENTIFIER))
        {
            return new Expr.Var(Previous());
        }

        if (Match(TokenType.LEFT_PAREN))
        {
            Expr expr = Expr();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
            return new Expr.Grouping(expr);
        }

        throw Error(Peek(), "Expect expression.");
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
