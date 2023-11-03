namespace HULK;

public abstract class LiteralExpression : Expression
{
    private readonly string value;

    protected LiteralExpression(string value)
    {
        this.value = value;
    }

    public override bool Validate(Context context)
    {
        return true;
    }

    public override string Evaluate(Context context)
    {
        return value;
    }
}

public class StringLiteral : LiteralExpression
{
    public StringLiteral(string value) : base(value)
    {
        Type = Type.String;
    }
}

public class NumericLiteral : LiteralExpression
{
    public NumericLiteral(string value) : base(value)
    {
        Type = Type.Number;
    }
}

public class BooleanLiteral : LiteralExpression
{
    public BooleanLiteral(string value) : base(value)
    {
        Type = Type.Boolean;
    }
}