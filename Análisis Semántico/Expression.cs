public abstract class Expression
{
    public string Value;

    public ExpressionType Type;

    public Expression(string value)
    {
        Value = value;
    }

    public abstract bool Validate(Context context);

    public abstract void Evaluate(Context context);
}

public enum ExpressionType 
{
    String,
    Bool,
    Numeric,
    Void
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

        return Body.Validate(innerContext) && context.Define(Identifier, Args, Body);
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

public class BinaryExpression : Expression
{
    public Operator Op;

    public Expression Left;
    
    public Expression Right;

    public BinaryExpression(Expression left, Operator op, Expression right) : base("")
    {
        Left = left;
        Op = op;
        Right = right;
    }

    public override bool Validate(Context context)
    {
        return Left.Validate(context) && Right.Validate(context);
    }

    public override void Evaluate(Context context)
    {
        Left.Evaluate(context);
        Right.Evaluate(context);

        switch (Op){
            case Operator.Sum: 
            case Operator.Sub: 
            case Operator.Mul: 
            case Operator.Div: 
            case Operator.Mod: 
            case Operator.Pow:
                NumericOperation(); break;
            case Operator.And:
                if (bool.Parse(Left.Value) && bool.Parse(Right.Value))
                    Value = "true";
                else 
                    Value = "false";
                Type = ExpressionType.Bool; break;
            case Operator.Or:
                if (bool.Parse(Left.Value) || bool.Parse(Right.Value))
                    Value = "true";
                else 
                    Value = "false";
                Type = ExpressionType.Bool; break;
            case Operator.Concat:
                Value = Left.Value + Right.Value;
                Type = ExpressionType.String;
                break;
            default:
                break;
        }
    }
    
    public void NumericOperation()
    {
        switch (Op)
        {
            case Operator.Sum:
                Value = (double.Parse(Left.Value) + double.Parse(Right.Value)).ToString();
                break;
            case Operator.Sub:
                Value = (double.Parse(Left.Value) - double.Parse(Right.Value)).ToString();
                break;
            case Operator.Mul:
                Value = (double.Parse(Left.Value) * double.Parse(Right.Value)).ToString();
                break;
            case Operator.Div:
                Value = (double.Parse(Left.Value) / double.Parse(Right.Value)).ToString();
                break;
            case Operator.Mod:
                Value = (double.Parse(Left.Value) % double.Parse(Right.Value)).ToString();
                break;
            case Operator.Pow:
                Value = Math.Pow(double.Parse(Left.Value), double.Parse(Right.Value)).ToString();
                break;
        }

        Type = ExpressionType.Numeric;
    }
}

public enum Operator
{
    Sum,
    Sub,
    Mul,
    Div,
    Mod,
    Pow,
    And,
    Or,
    Minor,
    MinorEqual,
    Major,
    MajorEqual,
    Equals,
    NotEqual,
    Concat
}

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
                Value = Math.Sin(double.Parse(Variables[0].Value)).ToString();
                break;
            case PredeterminedFunctions.cos:
                Value = Math.Cos(double.Parse(Variables[0].Value)).ToString();
                break;
            case PredeterminedFunctions.log:
                Value = Math.Log(double.Parse(Variables[1].Value), double.Parse(Variables[0].Value)).ToString();
                break;
            case PredeterminedFunctions.ln:
                Value = Math.Log(double.Parse(Variables[0].Value)).ToString();
                break;
        }
    }
}

public enum PredeterminedFunctions
{
    sin,
    cos,
    log,
    ln
}

public class FuncCall : Expression
{
    public string Identifier;

    public Expression[] Args;

    public FuncCall(string identifier, List<Expression> args) : base("")
    {
        Identifier = identifier;
        
        Args = new Expression[args.Count];
        for (int i = 0; i < args.Count; i++)
            Args[i] = args[i];
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