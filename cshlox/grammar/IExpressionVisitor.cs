namespace cshlox;

public interface IExpressionVisitor<T>
{
	T VisitBinaryExpression(Binary expression);
	T VisitGroupingExpression(Grouping expression);
	T VisitLiteralExpression(Literal expression);
	T VisitUnaryExpression(Unary expression);
}
