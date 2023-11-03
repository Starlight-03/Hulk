namespace HULK;

public abstract class Expression
{
    public SemanticError Semantic { get; protected set; }

    public Type Type { get; protected set; }

    public Expression()
    {
        Semantic = new SemanticError();
        Type = Type.NotSet;
    }

    public virtual void SetType(Context context, Type type) { }

    public abstract bool Validate(Context context);

    public abstract string Evaluate(Context context);
}

public enum Type 
{
    NotSet,
    String,
    Number,
    Boolean
}