namespace cshlox;

public class Binary : Expression
{
	public Expression Left { get; }
	public Token Op { get; }
	public Expression Right { get; }

	public Binary(Expression left, Token op, Expression right)
	{
		Left = left;
		Op = op;
		Right = right;
	}

	public override T Accept<T>(IExpressionVisitor<T> visitor)
	{
		return visitor.VisitBinaryExpression(this);
	}
}