namespace HULK;

public class NoBool : Expression
{
    private readonly Expression Boolean;

    public NoBool(Expression boolean)
    {
        Boolean = boolean;
        Type = Type.Boolean;
    }

    public override bool Validate(Context context)
    {
        if (Boolean.Validate(context) && Boolean.Type == Type.Boolean){
            return true;
        }
        Semantic.Show($"Operator \'!\' cannot be used with \'{Boolean.Type}\'");
        return false;
    }

    public override string Evaluate(Context context)
    {
        return Boolean.Evaluate(context);
    }
}