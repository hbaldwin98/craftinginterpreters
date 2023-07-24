namespace cshlox;

public interface IExprVisitor<T>
{
	T VisitBinaryExpr(Expr.Binary expr);
	T VisitGroupingExpr(Expr.Grouping expr);
	T VisitLiteralExpr(Expr.Literal expr);
	T VisitUnaryExpr(Expr.Unary expr);
	T VisitVarExpr(Expr.Var expr);
}
