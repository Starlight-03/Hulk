namespace HULK;

public abstract class BooleanExpression : BinaryExpression
{
    protected BooleanExpression(Expression left, Expression right) : base(left, right) { Type = Type.Boolean; }
}

public abstract class ComparisonExpression : BooleanExpression
{
    protected ComparisonExpression(Expression left, Expression right) : base(left, right) { }

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

public abstract class LogicExpression : BooleanExpression
{
    protected LogicExpression(Expression left, Expression right) : base(left, right) { }

    public override bool Validate(Context context)
    {
        if (!Left.Validate(context) || !Right.Validate(context))
            return false;
        if (Left.Type != Right.Type){
            if (Left.Type == Type.NotSet) Left.SetType(context, Type.Boolean);
            if (Right.Type == Type.NotSet) Right.SetType(context, Type.Boolean);
        }
        if (Left.Type == Right.Type && Left.Type != Type.NotSet && Left.Type != Type.Boolean){
            Semantic.Show($"Operator \'{Operator}\' cannot be used between \'{Left.Type}\' and \'{Right.Type}\'.");
            return false;
        }
        return Left.Type == Right.Type && (Left.Type == Type.Boolean || Left.Type == Type.NotSet);
    }
}

public class Minor : ComparisonExpression
{
    public Minor(Expression left, Expression right) : base(left, right) { Operator = "<"; }

    public override string Evaluate(Context context) => (double.Parse(Left.Evaluate(context)) < double.Parse(Right.Evaluate(context))).ToString();
}

public class MinorEqual : ComparisonExpression
{
    public MinorEqual(Expression left, Expression right) : base(left, right) { Operator = "<="; }

    public override string Evaluate(Context context) => (double.Parse(Left.Evaluate(context)) <= double.Parse(Right.Evaluate(context))).ToString();
}

public class Major : ComparisonExpression
{
    public Major(Expression left, Expression right) : base(left, right) { Operator = ">"; }

    public override string Evaluate(Context context) => (double.Parse(Left.Evaluate(context)) > double.Parse(Right.Evaluate(context))).ToString();
}

public class MajorEqual : ComparisonExpression
{
    public MajorEqual(Expression left, Expression right) : base(left, right) { Operator = ">="; }

    public override string Evaluate(Context context) => (double.Parse(Left.Evaluate(context)) >= double.Parse(Right.Evaluate(context))).ToString();
}

public class Equals : ComparisonExpression
{
    public Equals(Expression left, Expression right) : base(left, right) { Operator = "=="; }

    public override string Evaluate(Context context) => (double.Parse(Left.Evaluate(context)) == double.Parse(Right.Evaluate(context))).ToString();
}

public class NotEqual : ComparisonExpression
{
    public NotEqual(Expression left, Expression right) : base(left, right) { Operator = "!="; }

    public override string Evaluate(Context context) => (double.Parse(Left.Evaluate(context)) != double.Parse(Right.Evaluate(context))).ToString();
}

public class And : LogicExpression
{
    public And(Expression left, Expression right) : base(left, right) { Operator = "&"; }

    public override string Evaluate(Context context) => (bool.Parse(Left.Evaluate(context)) && bool.Parse(Right.Evaluate(context))).ToString();
}

public class Or : LogicExpression
{
    public Or(Expression left, Expression right) : base(left, right) { Operator = "|"; }

    public override string Evaluate(Context context) => (bool.Parse(Left.Evaluate(context)) || bool.Parse(Right.Evaluate(context))).ToString();
}