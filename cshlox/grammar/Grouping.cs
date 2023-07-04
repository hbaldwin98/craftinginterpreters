namespace cshlox;

public class Grouping : Expression
{
	public Expression Expr { get; }

	public Grouping(Expression expr)
	{
		Expr = expr;
	}

	public override T Accept<T>(IVisitor<T> visitor)
	{
		return visitor.VisitGroupingExpression(this);
	}
}