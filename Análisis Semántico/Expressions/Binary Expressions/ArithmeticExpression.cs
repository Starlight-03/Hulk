namespace HULK;

public abstract class ArithmeticExpression : BinaryExpression
{
    protected ArithmeticExpression(Expression left, Expression right) : base(left, right) { Type = Type.Number; }

    public override bool Validate(Context context)
    {
        if (!Left.Validate(context) || !Right.Validate(context))
            return false;
        if (Left.Type != Right.Type){
            if (Left.Type == Type.NotSet) Left.SetType(context, Type.Number);
            if (Right.Type == Type.NotSet) Right.SetType(context, Type.Number);
        }
        if (Left.Type == Right.Type && Left.Type != Type.NotSet && Left.Type != Type.Number){
            Semantic.Show($"Operator \'{Operator}\' cannot be used between \'{Left.Type}\' and \'{Right.Type}\'.");
            return false;
        }
        return Left.Type == Right.Type && (Left.Type == Type.Number || Left.Type == Type.NotSet);
    }
}

public class Sum : ArithmeticExpression
{
    public Sum(Expression left, Expression right) : base(left, right) { Operator = "+"; }

    public override string Evaluate(Context context) => (double.Parse(Left.Evaluate(context)) + double.Parse(Right.Evaluate(context))).ToString();
}

public class Sub : ArithmeticExpression
{
    public Sub(Expression left, Expression right) : base(left, right) { Operator = "-"; }

    public override string Evaluate(Context context) => (double.Parse(Left.Evaluate(context)) - double.Parse(Right.Evaluate(context))).ToString();
}

public class Mul : ArithmeticExpression
{
    public Mul(Expression left, Expression right) : base(left, right) { Operator = "*"; }

    public override string Evaluate(Context context) => (double.Parse(Left.Evaluate(context)) * double.Parse(Right.Evaluate(context))).ToString();
}

public class Div : ArithmeticExpression
{
    public Div(Expression left, Expression right) : base(left, right) { Operator = "/"; }

    public override string Evaluate(Context context) => (double.Parse(Left.Evaluate(context)) / double.Parse(Right.Evaluate(context))).ToString();
}

public class Mod : ArithmeticExpression
{
    public Mod(Expression left, Expression right) : base(left, right) { Operator = "%"; }

    public override string Evaluate(Context context) => (double.Parse(Left.Evaluate(context)) % double.Parse(Right.Evaluate(context))).ToString();
}

public class Pow : ArithmeticExpression
{
    public Pow(Expression left, Expression right) : base(left, right) { Operator = "^"; }

    public override string Evaluate(Context context) => Math.Pow(double.Parse(Left.Evaluate(context)), double.Parse(Right.Evaluate(context))).ToString();
}