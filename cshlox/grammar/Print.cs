namespace cshlox;

public class Print : Statement
{
	public Expression Expr { get; }

	public Print(Expression expr)
	{
		Expr = expr;
	}

	public override T Accept<T>(IStatementVisitor<T> visitor)
	{
		return visitor.VisitPrintStatement(this);
	}
}
