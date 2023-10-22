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
            case Operator.Sum: case Operator.Sub: 
            case Operator.Mul: case Operator.Div: 
            case Operator.Mod: case Operator.Pow:
                NumericOperation(); 
                break;
            case Operator.And: case Operator.Or:
            case Operator.Minor: case Operator.MinorEqual:
            case Operator.Major: case Operator.MajorEqual:
            case Operator.Equals: case Operator.NotEqual:
                BooleanOperation();
                break;
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
    
    public void BooleanOperation()
    {
        switch (Op)
        {
            case Operator.And:
                Value = (bool.Parse(Left.Value) && bool.Parse(Right.Value)).ToString();
                break;
            case Operator.Or:
                Value = (bool.Parse(Left.Value) || bool.Parse(Right.Value)).ToString();
                break;
            case Operator.Minor:
                Value = (double.Parse(Left.Value) < double.Parse(Right.Value)).ToString();
                break;
            case Operator.MinorEqual:
                Value = (double.Parse(Left.Value) <= double.Parse(Right.Value)).ToString();
                break;
            case Operator.Major:
                Value = (double.Parse(Left.Value) > double.Parse(Right.Value)).ToString();
                break;
            case Operator.MajorEqual:
                Value = (double.Parse(Left.Value) >= double.Parse(Right.Value)).ToString();
                break;
            case Operator.Equals:
                Value = (double.Parse(Left.Value) == double.Parse(Right.Value)).ToString();
                break;
            case Operator.NotEqual:
                Value = (double.Parse(Left.Value) != double.Parse(Right.Value)).ToString();
                break;
        }
        Type = ExpressionType.Bool;
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