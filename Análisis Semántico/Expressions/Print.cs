namespace HULK;

public class Print : Expression
{
    private readonly Expression Expr;

    public Print(Expression expression)
    {
        Expr = expression;
        Type = Type.String;
    }

    public override bool Validate(Context context)
    {
        return Expr.Validate(context);
    }

    public override string Evaluate(Context context)
    {
        return Expr.Evaluate(context);
    }
}