public abstract class Expression
{
    public SemanticError Semantic { get; protected set; }

    public string Value { get; protected set; }

    public ExpressionType Type { get; protected set; }

    public Expression(string value)
    {
        Semantic = new SemanticError();
        Value = value;
    }

    public abstract bool Validate(Context context);

    public abstract void Evaluate(Context context);
}

public enum ExpressionType 
{
    Void,
    Numeric,
    Bool,
    String
}

public class Print : Expression
{
    public Expression Expr;

    public Print(Expression expression) : base("")
    {
        Expr = expression;
        Type = ExpressionType.Void;
    }

    public override bool Validate(Context context)
    {
        return Expr.Validate(context);
    }

    public override void Evaluate(Context context)
    {
        Expr.Evaluate(context);
        Value = Expr.Value;
    }
}

public class FuncDef : Expression
{
    public string Identifier;

    public string[] Args;

    public Expression Body;

    public FuncDef(string identifier, List<string> args, Expression body) : base("")
    {
        Identifier = identifier;
        
        Args = new string[args.Count];
        for (int i = 0; i < args.Count; i++)
            Args[i] = args[i];

        Body = body;
        Value = "";
        Type = ExpressionType.Void;
    }

    public override bool Validate(Context context)
    {
        Context innerContext = new Context(context);

        foreach (string arg in Args)
            if (!innerContext.Define(arg))
                return false;

        if (context.Define(Identifier, Args, Body)){
            if (Body.Validate(innerContext))
                return true;
            else
                context.Undefine(Identifier, Args);
        }
        return false;
    }

    public override void Evaluate(Context context)
    {
    }
}

public class LetIn : Expression
{
    public Dictionary<string, Expression> Variables;
    
    public Expression Body;

    public Context innerContext;

    public LetIn(Dictionary<string, Expression> variables, Expression body) : base("")
    {
        Variables = new Dictionary<string, Expression>();
        foreach (string variable in variables.Keys)
            Variables[variable] = variables[variable];

        Body = body;
    }

    public override bool Validate(Context context)
    {
        innerContext = new Context(context);

        foreach (string variable in Variables.Keys){
            if (!Variables[variable].Validate(context))
                return false;
            if (!innerContext.Define(variable))
                return false;
            innerContext.SetValue(variable, Variables[variable]);
        }

        if (!Body.Validate(innerContext))
            return false;
        
        return true;
    }

    public override void Evaluate(Context context)
    {        
        Body.Evaluate(innerContext);
        Value = Body.Value;
        Type = Body.Type;
    }
}

public class IfElse : Expression
{
    public Expression Condition;

    public Expression Positive;

    public Expression Negative;

    public IfElse(Expression condition, Expression positive, Expression negative) : base("")
    {
        Condition = condition;
        Positive = positive;
        Negative = negative;
    }

    public override bool Validate(Context context)
    {
        return Condition.Validate(context) && Positive.Validate(context) && Negative.Validate(context);
    }

    public override void Evaluate(Context context)
    {
        Condition.Evaluate(context);

        if (bool.Parse(Condition.Value)){
            Positive.Evaluate(context);
            Value = Positive.Value;
            Type = Positive.Type;
        }
        else{
            Negative.Evaluate(context);
            Value = Negative.Value;
            Type = Negative.Type;
        }
    }
}