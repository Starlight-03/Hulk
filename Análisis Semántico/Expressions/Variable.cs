namespace HULK;

public class Variable : Expression
{
    public string Identifier;

    public Variable(string identifier)
    {
        Identifier = identifier;
    }

    public override void SetType(Context context, Type type)
    {
        context.SetType(Identifier, type);
        Type = type;
    }

    public override bool Validate(Context context)
    {
        if (context.IsDefined(Identifier)){
            Type = context.GetType(Identifier);
            return true;
        }
        Semantic.Show($"Variable \'{Identifier}\' has not been defined in this context.");
        return false;
    }

    public override string Evaluate(Context context)
    {
        return context.GetValue(Identifier);
    }
}