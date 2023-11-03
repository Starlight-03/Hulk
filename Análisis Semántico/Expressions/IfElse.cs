namespace HULK;

public class IfElse : Expression
{
    private readonly Expression Condition;

    private readonly Expression Positive;

    private readonly Expression Negative;

    public IfElse(Expression condition, Expression positive, Expression negative)
    {
        Condition = condition;
        Positive = positive;
        Negative = negative;
    }

    public override bool Validate(Context context)
    {
        return Condition.Validate(context) && Positive.Validate(context) && Negative.Validate(context);
    }

    public override string Evaluate(Context context)
    {
        string condition = Condition.Evaluate(context);

        if (bool.Parse(condition)){
            return Positive.Evaluate(context);
        }
        else{
            return Negative.Evaluate(context);
        }
    }
}