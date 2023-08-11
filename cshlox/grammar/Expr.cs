namespace cshlox;

public abstract class Expr
{
	public abstract T Accept<T>(IExprVisitor<T> visitor);

	public class Assign : Expr
	{
		public Token Name { get; }
		public Expr Value { get; }

		public Assign(Token name, Expr value)
		{
			Name = name;
			Value = value;
		}

		public override T Accept<T>(IExprVisitor<T> visitor)
		{
			return visitor.VisitAssignExpr(this);
		}
	}

	public class Binary : Expr
	{
		public Expr Left { get; }
		public Token Op { get; }
		public Expr Right { get; }

		public Binary(Expr left, Token op, Expr right)
		{
			Left = left;
			Op = op;
			Right = right;
		}

		public override T Accept<T>(IExprVisitor<T> visitor)
		{
			return visitor.VisitBinaryExpr(this);
		}
	}

	public class Grouping : Expr
	{
		public Expr Expr { get; }

		public Grouping(Expr expr)
		{
			Expr = expr;
		}

		public override T Accept<T>(IExprVisitor<T> visitor)
		{
			return visitor.VisitGroupingExpr(this);
		}
	}

	public class Literal : Expr
	{
		public object Value { get; }

		public Literal(object value)
		{
			Value = value;
		}

		public override T Accept<T>(IExprVisitor<T> visitor)
		{
			return visitor.VisitLiteralExpr(this);
		}
	}

	public class Logical : Expr
	{
		public Expr Left { get; }
		public Token Op { get; }
		public Expr Right { get; }

		public Logical(Expr left, Token op, Expr right)
		{
			Left = left;
			Op = op;
			Right = right;
		}

		public override T Accept<T>(IExprVisitor<T> visitor)
		{
			return visitor.VisitLogicalExpr(this);
		}
	}

	public class Unary : Expr
	{
		public Token Op { get; }
		public Expr Right { get; }

		public Unary(Token op, Expr right)
		{
			Op = op;
			Right = right;
		}

		public override T Accept<T>(IExprVisitor<T> visitor)
		{
			return visitor.VisitUnaryExpr(this);
		}
	}

	public class Var : Expr
	{
		public Token Name { get; }

		public Var(Token name)
		{
			Name = name;
		}

		public override T Accept<T>(IExprVisitor<T> visitor)
		{
			return visitor.VisitVarExpr(this);
		}
	}
}
