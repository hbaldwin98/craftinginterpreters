using System.Text;

namespace cshlox;

public class AstPrinter : IExpressionVisitor<string>
{
    public string Print(Expression expr)
    {
        return expr.Accept(this);
    }

    public string VisitBinaryExpression(Binary expression)
    {
        return Parenthesize(expression.Op.Lexeme, expression.Left, expression.Right);
    }

    public string VisitGroupingExpression(Grouping expression)
    {
        return Parenthesize("group", expression.Expr);
    }

    public string VisitLiteralExpression(Literal expression)
    {
        if (expression.Value == null) { return "nil"; }

        return expression.Value.ToString();
    }

    public string VisitUnaryExpression(Unary expression)
    {
        return Parenthesize(expression.Op.Lexeme, expression.Right);
    }

    private string Parenthesize(string name, params Expression[] expressions)
    {
        StringBuilder builder = new StringBuilder();

        builder.Append("(").Append(name);

        foreach (Expression expr in expressions)
        {
            builder.Append(" ");
            builder.Append(expr.Accept(this));
        }
        builder.Append(")");

        return builder.ToString();
    }
}
