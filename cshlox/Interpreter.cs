namespace cshlox;

public class Interpreter : IExpressionVisitor<object>, IStatementVisitor<object>
{

    public void Interpret(List<Statement> statements)
    {
        try
        {
            foreach (Statement statement in statements)
            {
                Execute(statement);
            }
        }
        catch (RuntimeError error)
        {
            Cshlox.RuntimeError(error);
        }
    }

    public object VisitBinaryExpression(Binary expression)
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

    public object VisitGroupingExpression(Grouping expression)
    {
        return Evaluate(expression.Expr);
    }

    public object VisitLiteralExpression(Literal expression)
    {
        return expression.Value;
    }

    public object VisitUnaryExpression(Unary expression)
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

    public object VisitStatementExpressionStatement(StatementExpression statement)
    {
        Evaluate(statement.Expr);

        return null;
    }

    public object VisitPrintStatement(Print statement)
    {
        object value = Evaluate(statement.Expr);
        Console.WriteLine(Stringify(value));

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

    public object Evaluate(Expression expr)
    {
        return expr.Accept(this);
    }

    public void Execute(Statement statement)
    {
        statement.Accept(this);
    }
}
