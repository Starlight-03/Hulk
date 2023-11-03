namespace HULK;

public class BinaryExpression : Expression
{
    private readonly Expression Left;

    private readonly Operator Op;
    
    private readonly Expression Right;

    public BinaryExpression(Expression left, Operator op, Expression right)
    {
        Left = left;
        Op = op;
        Right = right;
        Type = GetType(op.Op);
    }

    private static Type GetType(Op op)
    {
        switch (op){
            case HULK.Op.Sum: case HULK.Op.Sub:
            case HULK.Op.Mul: case HULK.Op.Div: 
            case HULK.Op.Mod: case HULK.Op.Pow:
                return Type.Number;
            case HULK.Op.And: case HULK.Op.Or:
            case HULK.Op.Minor: case HULK.Op.MinorEqual:
            case HULK.Op.Major: case HULK.Op.MajorEqual:
            case HULK.Op.Equals: case HULK.Op.NotEqual:
                return Type.Boolean;
            case HULK.Op.Concat:
                return Type.String;
            default:
                return Type.NotSet;
        }
    }

    public override bool Validate(Context context)
    {
        if (!Left.Validate(context) || !Right.Validate(context))
            return false;
        if (Op.Op == HULK.Op.Concat)
            return true;
        if (Left.Type != Right.Type && (Left.Type == Type.NotSet || Right.Type == Type.NotSet)){
            if (Left.Type == Type.NotSet) 
                Left.SetType(context, Right.Type);
            else 
                Right.SetType(context, Left.Type);
        }
        if (Left.Type == Right.Type){
            switch (Op.Op){
                case HULK.Op.Sum: case HULK.Op.Sub: case HULK.Op.Mul: case HULK.Op.Div: 
                case HULK.Op.Mod: case HULK.Op.Pow: case HULK.Op.Minor: case HULK.Op.MinorEqual:
                case HULK.Op.Major: case HULK.Op.MajorEqual: case HULK.Op.Equals: case HULK.Op.NotEqual:
                    if (Left.Type != Type.Number && Left.Type != Type.NotSet){
                        Semantic.Show($"Operator \'{Op}\' cannot be used between \'{Left.Type}\' and \'{Right.Type}\'.");
                        return false;
                    }
                    return true;
                case HULK.Op.And: case HULK.Op.Or:
                    if (Left.Type != Type.Boolean && Left.Type != Type.NotSet){
                        Semantic.Show($"Operator \'{Op}\' cannot be used between \'{Left.Type}\' and \'{Right.Type}\'.");
                        return false;
                    }
                    return true;
            }
        }
        Semantic.Show($"Operator \'{Op}\' cannot be used between \'{Left.Type}\' and \'{Right.Type}\'.");
        return false;
    }

    public override string Evaluate(Context context)
    {
        string left = Left.Evaluate(context);
        string right = Right.Evaluate(context);

        switch (Type){
            case Type.Number:
                return NumericOperation(left, right);
            case Type.Boolean:
                return BooleanOperation(left, right);
            case Type.String:
                return left + " " + right;
            default:
                return "";
        }
    }
    
    private string NumericOperation(string left, string right)
    {
        switch (Op.Op){
            case HULK.Op.Sum:
                return (double.Parse(left) + double.Parse(right)).ToString();
            case HULK.Op.Sub:
                return (double.Parse(left) - double.Parse(right)).ToString();
            case HULK.Op.Mul:
                return (double.Parse(left) * double.Parse(right)).ToString();
            case HULK.Op.Div:
                return (double.Parse(left) / double.Parse(right)).ToString();
            case HULK.Op.Mod:
                return (double.Parse(left) % double.Parse(right)).ToString();
            case HULK.Op.Pow:
                return Math.Pow(double.Parse(left), double.Parse(right)).ToString();
            default:
                return "";
        }
    }
    
    private string BooleanOperation(string left, string right)
    {
        switch (Op.Op){
            case HULK.Op.And:
                return (bool.Parse(left) && bool.Parse(right)).ToString();
            case HULK.Op.Or:
                return (bool.Parse(left) || bool.Parse(right)).ToString();
            case HULK.Op.Minor:
                return(double.Parse(left) < double.Parse(right)).ToString();
            case HULK.Op.MinorEqual:
                return (double.Parse(left) <= double.Parse(right)).ToString();
            case HULK.Op.Major:
                return (double.Parse(left) > double.Parse(right)).ToString();
            case HULK.Op.MajorEqual:
                return (double.Parse(left) >= double.Parse(right)).ToString();
            case HULK.Op.Equals:
                return (double.Parse(left) == double.Parse(right)).ToString();
            case HULK.Op.NotEqual:
                return (double.Parse(left) != double.Parse(right)).ToString();
            default:
                return "";
        }
    }
}