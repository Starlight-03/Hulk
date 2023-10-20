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

public class Function
{
    public string Identifier;

    public Variable[] Args;

    public Context InnerContext;

    public Expression Body;

    public Function(string identifier, List<Variable> args, Context innerContext, Expression body)
    {
        Identifier = identifier;

        Args = new Variable[args.Count];
        for (int i = 0; i < args.Count; i++)
            Args[i] = args[i];

        InnerContext = innerContext;

        Body = body;
    }
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

        if (!context.IsDefined(Identifier, Args))
            return false;

        return true;
    }

    public override void Evaluate(Context context)
    {
        
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