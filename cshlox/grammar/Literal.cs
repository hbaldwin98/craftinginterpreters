namespace cshlox;

public class Literal : Expression
{
	public object Value { get; }

	public Literal(object value)
	{
		Value = value;
	}

	public override T Accept<T>(IVisitor<T> visitor)
	{
		return visitor.VisitLiteralExpression(this);
	}
}