namespace cshlox;

public interface IStmtVisitor<T>
{
	T VisitExpressionStmt(Stmt.Expression stmt);
	T VisitPrintStmt(Stmt.Print stmt);
	T VisitVarStmt(Stmt.Var stmt);
}
