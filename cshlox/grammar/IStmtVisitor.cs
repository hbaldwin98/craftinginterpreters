namespace cshlox;

public interface IStmtVisitor<T>
{
	T VisitBlockStmt(Stmt.Block stmt);
	T VisitExpressionStmt(Stmt.Expression stmt);
	T VisitIfStmt(Stmt.If stmt);
	T VisitPrintStmt(Stmt.Print stmt);
	T VisitWhileStmt(Stmt.While stmt);
	T VisitVarStmt(Stmt.Var stmt);
}
