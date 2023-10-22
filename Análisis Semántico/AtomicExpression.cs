public abstract class AtomicExpression : Expression
{
    protected AtomicExpression(string value) : base(value)
    {
    }

    public override bool Validate(Context context)
    {
        return true;
    }

    public override void Evaluate(Context context)
    {
    }
}

public class PredeterminedFunction : AtomicExpression
{
    public PredeterminedFunctions Function;

    public Variable[] Variables;

    public PredeterminedFunction(PredeterminedFunctions func, params Variable[] variables) : base("")
    {
        Function = func;
        
        Variables = new Variable[variables.Length];
        for (int i = 0; i < variables.Length; i++)
            Variables[i] = variables[i];
    }

    public override bool Validate(Context context)
    {
        foreach (Variable variable in Variables)
            if (!variable.Validate(context))
                return false;
                
        return true;
    }

    public override void Evaluate(Context context)
    {
        foreach (Variable variable in Variables)
            variable.Evaluate(context);
        
        switch (Function)
        {
            case PredeterminedFunctions.sin:
                Value = Math.Round(Math.Sin(double.Parse(Variables[0].Value))).ToString();
                break;
            case PredeterminedFunctions.cos:
                Value = Math.Round(Math.Cos(double.Parse(Variables[0].Value))).ToString();
                break;
            case PredeterminedFunctions.log:
                Value = Math.Round(Math.Log(double.Parse(Variables[1].Value), 
                                            double.Parse(Variables[0].Value))).ToString();
                break;
            case PredeterminedFunctions.ln:
                Value = Math.Round(Math.Log(double.Parse(Variables[0].Value))).ToString();
                break;
        }
        Type = ExpressionType.Numeric;
    }
}

public enum PredeterminedFunctions
{
    sin,
    cos,
    log,
    ln
}

public class FuncCall : AtomicExpression
{
    public string Identifier;

    public Expression[] Args;

    public FuncCall(string identifier, List<Expression> args) : base("")
    {
        Identifier = identifier;
        
        Args = new Expression[args.Count];
        for (int i = 0; i < args.Count; i++)
            Args[i] = args[i];

        Type = ExpressionType.Void;
    }

    public override bool Validate(Context context)
    {
        foreach (Expression arg in Args)
            if (!arg.Validate(context))
                return false;

        return context.IsDefined(Identifier, Args);
    }

    public override void Evaluate(Context context)
    {
        Context innerContext = new Context(context);

        foreach (var args in context.GetFunction(Identifier).Keys)
        {
            if (args.Length == Args.Length)
            {
                for (int i = 0; i < Args.Length; i++)
                {
                    if (innerContext.Define(args[i]))
                        innerContext.SetValue(args[i], Args[i]);
                }
                Expression body = context.GetBody(Identifier, args);
                body.Evaluate(innerContext);
                Value = body.Value;
            }
        }
    }
}

public class Variable : AtomicExpression
{
    public string Identifier;

    public Variable(string identifier) : base("")
    {
        Identifier = identifier;
    }

    public override bool Validate(Context context)
    {
        return context.IsDefined(Identifier);
    }

    public override void Evaluate(Context context)
    {
        Expression val = context.GetValue(Identifier);
        val.Evaluate(context);
        Value = val.Value; 

        if (double.TryParse(Value, out double n))
            Type = ExpressionType.Numeric;
        else if (bool.TryParse(Value, out bool b))
            Type = ExpressionType.Bool;
        else
            Type = ExpressionType.String;
    }
}

public class NoBool : AtomicExpression
{
    public Expression Boolean;

    public NoBool(Expression boolean) : base("")
    {
        Boolean = boolean;
        Type = ExpressionType.Bool;
    }

    public override bool Validate(Context context)
    {
        return Boolean.Validate(context);
    }

    public override void Evaluate(Context context)
    {
        Boolean.Evaluate(context);
        Value = (!bool.Parse(Boolean.Value)).ToString();
    }
}

public class Number : AtomicExpression
{
    public Number(string value) : base(value)
    {
        Type = ExpressionType.Numeric;
    }
}

public class BooleanLiteral : AtomicExpression
{
    public BooleanLiteral(string value) : base(value)
    {
        Type = ExpressionType.Bool;
    }
}

public class StringLiteral : AtomicExpression
{
    public StringLiteral(string value) : base(value)
    {
        Type = ExpressionType.String;
    }
}