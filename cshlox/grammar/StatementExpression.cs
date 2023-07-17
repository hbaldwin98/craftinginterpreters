namespace cshlox;

public class StatementExpression : Statement
{
	public Expression Expr { get; }

	public StatementExpression(Expression expr)
	{
		Expr = expr;
	}

	public override T Accept<T>(IStatementVisitor<T> visitor)
	{
		return visitor.VisitStatementExpressionStatement(this);
	}
}
