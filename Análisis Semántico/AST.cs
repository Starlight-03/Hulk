public class Context
{
    private Context parent;

    private Dictionary<string, Expression> variables;

    private Dictionary<string, HashSet<string[]>> functions;

    public Context(Context parent)
    {
        this.parent = parent;
        variables = new Dictionary<string, Expression>();
        functions = new Dictionary<string, HashSet<string[]>>();
    }
    
    public bool Define(string variable)
    {
        if (!variables.ContainsKey(variable))
        {
            variables.Add(variable, null);
            return true;
        }
        
        return false;
    }

    public bool Define(string function, string[] args)
    {
        if (!functions.ContainsKey(function) || functions[function].Contains(args))
            return functions[function].Add(args);
        else
            return false;
    }

    public bool IsDefined(string variable)
    {
        return variables.ContainsKey(variable) 
            || parent != null && parent.IsDefined(variable);
    }
    
    public bool IsDefined(string function, Expression[] args)
    {
        if (functions.ContainsKey(function)){
            foreach (var ar in functions[function]){
                if (ar.Length == args.Length)
                    return true;
            }
        }
        
        return parent != null && parent.IsDefined(function, args);
    }

    public Expression GetValue(string variable)
    {
        return variables[variable];
    }
}

public enum ExpressionType 
{
    String,
    Bool,
    Numeric,
    Void
}

public class CompilerProgram : Expression
{
    public Expression Line;

    public CompilerProgram() : base("")
    {
    }

    public override bool Validate(Context context)
    {
        return Line.Validate(context);
    }
    
    public override void Evaluate(Context context)
    {
        Line.Evaluate(context);
        Value = Line.Value;
        Type = ExpressionType.Void;
    }
}

public class Print : Expression
{
    public Expression Expr;

    public Print(Expression expression) : base("")
    {
        Expr = expression;
    }

    public override bool Validate(Context context)
    {
        return Expr.Validate(context);
    }

    public override void Evaluate(Context context)
    {
        Expr.Evaluate(context);
        Value = Expr.Value;
        Type = ExpressionType.Void;
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
    }

    public override bool Validate(Context context)
    {
        Context innerContext = new Context(context);

        foreach (string arg in Args)
            if (!innerContext.Define(arg))
                return false;

        if (!Body.Validate(innerContext) || context.Define(Identifier, Args))
            return false;

        return true;
    }

    public override void Evaluate(Context context)
    {
        Value = "";
        Type = ExpressionType.Void;
    }
}

public class LetIn : Expression
{
    public Dictionary<string, Expression> Variables;
    
    public Expression Body;

    public LetIn(Dictionary<string, Expression> variables, Expression body) : base("")
    {
        Variables = new Dictionary<string, Expression>();
        foreach (string variable in variables.Keys)
            Variables[variable] = variables[variable];

        Body = body;
    }

    public override bool Validate(Context context)
    {
        Context innerContext = new Context(context);

        foreach (string variable in Variables.Keys){
            if (!Variables[variable].Validate(context))
                return false;
            if (!innerContext.Define(variable))
                return false;
        }

        if (!Body.Validate(innerContext))
            return false;
        
        return true;
    }

    public override void Evaluate(Context context)
    {        
        Body.Evaluate(context);
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
        }
        else{
            Negative.Evaluate(context);
            Value = Negative.Value;
        }
    }
}