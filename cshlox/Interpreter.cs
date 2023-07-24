namespace cshlox;

public class Interpreter : IExprVisitor<object>, IStmtVisitor<object>
{
    private Env _environment = new Env();

    public void Interpret(List<Stmt> statements)
    {
        try
        {
            foreach (Stmt statement in statements)
            {
                Execute(statement);
            }
        }
        catch (RuntimeError error)
        {
            Cshlox.RuntimeError(error);
        }
    }

    public object VisitBinaryExpr(Expr.Binary expression)
    {
        object left = Evaluate(expression.Left);
        object right = Evaluate(expression.Right);

        switch (expression.Op.Type)
        {
            case TokenType.BANG_EQUAL:
                return !IsEqual(left, right);
            case TokenType.EQUAL_EQUAL:
                return IsEqual(left, right);
            case TokenType.GREATER:
                CheckNumberOperand(expression.Op, left, right);
                return (double)left > (double)right;
            case TokenType.GREATER_EQUAL:
                CheckNumberOperand(expression.Op, left, right);
                return (double)left >= (double)right;
            case TokenType.LESS:
                CheckNumberOperand(expression.Op, left, right);
                return (double)left < (double)right;
            case TokenType.LESS_EQUAL:
                CheckNumberOperand(expression.Op, left, right);
                return (double)left <= (double)right;
            case TokenType.MINUS:
                CheckNumberOperand(expression.Op, left, right);
                return (double)left - (double)right;
            case TokenType.PLUS:
                if (left is double && right is double)
                {
                    return (double)left + (double)right;
                }

                if (left is string && right is string)
                {
                    return (string)left + (string)right;
                }

                if (left is string && right is double || left is double && right is string)
                {
                    return Stringify(left) + Stringify(right);
                }

                throw new RuntimeError(expression.Op, "Operands must be numbers or two strings.");
            case TokenType.SLASH:
                CheckNumberOperand(expression.Op, left, right);
                return (double)left / (double)right;
            case TokenType.STAR:
                CheckNumberOperand(expression.Op, left, right);
                return (double)left * (double)right;
        }

        return null;
    }

    public object VisitGroupingExpr(Expr.Grouping expression)
    {
        return Evaluate(expression.Expr);
    }

    public object VisitLiteralExpr(Expr.Literal expression)
    {
        return expression.Value;
    }

    public object VisitUnaryExpr(Expr.Unary expression)
    {
        object right = Evaluate(expression.Right);

        switch (expression.Op.Type)
        {
            case TokenType.BANG:
                return !IsTruthy(right);
            case TokenType.MINUS:
                CheckNumberOperand(expression.Op, right);
                return -(double)right;
        }

        return null;
    }

    public object VisitVarExpr(Expr.Var expr)
    {
        return _environment.Get(expr.Name);
    }

    public object VisitExpressionStmt(Stmt.Expression statement)
    {
        Evaluate(statement.Expr);

        return null;
    }

    public object VisitPrintStmt(Stmt.Print statement)
    {
        object value = Evaluate(statement.Expr);
        Console.WriteLine(Stringify(value));

        return null;
    }

    public object VisitVarStmt(Stmt.Var statement)
    {
        object value = null;

        if (statement.Initializer != null)
        {
            value = Evaluate(statement.Initializer);
        }

        _environment.Define(statement.Name.Lexeme, value);
        return null;
    }

    public object VisitAssignExpr(Expr.Assign expr)
    {
        object value = Evaluate(expr.Value);

        _environment.Assign(expr.Name, value);
        return null;
    }

    private void CheckNumberOperand(Token op, object operand)
    {
        if (operand is double) { return; }

        throw new RuntimeError(op, "Operand must be a number.");
    }

    private void CheckNumberOperand(Token op, object left, object right)
    {
        if (left is double && right is double) { return; }

        throw new RuntimeError(op, "Operands must be numbers.");
    }

    public bool IsTruthy(object obj)
    {
        if (obj == null) { return false; }
        if (obj is bool) { return (bool)obj; }

        return true;
    }

    public bool IsEqual(object a, object b)
    {
        if (a == null && b == null) { return true; }
        if (a == null) { return false; }

        return a.Equals(b);
    }

    private string Stringify(object obj)
    {
        if (obj == null) { return "nil"; }

        if (obj is double)
        {
            string text = obj.ToString();
            if (text.EndsWith(".0"))
            {
                text = text.Substring(0, text.Length - 2);
            }

            return text;
        }

        return obj.ToString();
    }

    public object Evaluate(Expr expr)
    {
        return expr.Accept(this);
    }

    public void Execute(Stmt statement)
    {
        statement.Accept(this);
    }

    public object VisitBlockStmt(Stmt.Block stmt)
    {
        ExecuteBlock(stmt.Statements, new Env(_environment));
        return null;
    }

    private void ExecuteBlock(List<Stmt> statements, Env environment)
    {
        Env previous = _environment;

        try
        {
            _environment = environment;

            foreach (Stmt statement in statements)
            {
                Execute(statement);
            }
        }
        finally
        {
            _environment = previous;
        }
    }
}
