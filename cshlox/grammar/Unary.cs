namespace cshlox;

public class Unary : Expression
{
	public Token Op { get; }
	public Expression Right { get; }

	public Unary(Token op, Expression right)
	{
		Op = op;
		Right = right;
	}

	public override T Accept<T>(IVisitor<T> visitor)
	{
		return visitor.VisitUnaryExpression(this);
	}
}