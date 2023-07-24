using System.Text;

namespace cshlox;

public class AstPrinter : IExprVisitor<string>
{
    public string Print(Expr expr)
    {
        return expr.Accept(this);
    }

    public string VisitBinaryExpr(Expr.Binary expression)
    {
        return Parenthesize(expression.Op.Lexeme, expression.Left, expression.Right);
    }

    public string VisitGroupingExpr(Expr.Grouping expression)
    {
        return Parenthesize("group", expression.Expr);
    }

    public string VisitLiteralExpr(Expr.Literal expression)
    {
        if (expression.Value == null) { return "nil"; }

        return expression.Value.ToString();
    }

    public string VisitUnaryExpr(Expr.Unary expression)
    {
        return Parenthesize(expression.Op.Lexeme, expression.Right);
    }

    public string VisitVarExpr(Expr.Var expression)
    {
        throw new NotImplementedException();
    }

    private string Parenthesize(string name, params Expr[] expressions)
    {
        StringBuilder builder = new StringBuilder();

        builder.Append("(").Append(name);

        foreach (Expr expr in expressions)
        {
            builder.Append(" ");
            builder.Append(expr.Accept(this));
        }
        builder.Append(")");

        return builder.ToString();
    }
}
