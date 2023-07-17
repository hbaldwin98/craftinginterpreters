namespace cshlox;

public interface IStatementVisitor<T>
{
	T VisitStatementExpressionStatement(StatementExpression statement);
	T VisitPrintStatement(Print statement);
}
