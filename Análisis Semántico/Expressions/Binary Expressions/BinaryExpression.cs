namespace HULK;

public abstract class BinaryExpression : Expression // Una de las expresiones más complejas del compilador (y a la vez más sencillas)
{
    // Posee dos expresiones internas, una izquierda y otra derecha
    // Al validar (evaluar) es necesario validar (evaluar) las dos expresiones internas antes de continuar con el resto del proceso
    // Si dos tipos de expresión no son válidos para el operdor que se está utilizando, se lanza una excepción

    protected readonly Expression Left;
    
    protected readonly Expression Right;

    protected string Operator;

    public BinaryExpression(Expression left, Expression right)
    {
        Left = left;
        Right = right;
        Operator = "";
    }
}

public class Concat : BinaryExpression
{
    public Concat(Expression left, Expression right) : base(left, right)
    {
        Type = Type.String;
        Operator = "@";
    }

    public override bool Validate(Context context)
    {
        if (!Left.Validate(context) || !Right.Validate(context))
            return false;
        if (Left.Type == Type.NotSet) 
            Left.SetType(context, Type.String);
        if (Left.Type != Type.NotSet || Left.Type != Type.String){
            Semantic.Show($"Operator \'{Operator}\' cannot be used with a \'{Left.Type}\' on the left.");
            return false;
        }
        return Left.Type == Type.String || Left.Type == Type.NotSet;
    }

    public override string Evaluate(Context context) => Left.Evaluate(context) + " " + Right.Evaluate(context);
}