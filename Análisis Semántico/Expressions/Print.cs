namespace HULK;

public class Print : Expression // Expresión de llamada al método print, para mostrar en pantalla el resultado de la expresión interior
{
    // Todo print tiene una expresión interna
    // Al validar (evaluar) sólo se valida (evalúa) la expresión interna, y el valor devuelto es el valor de la expresión interna en String
    private readonly Expression Expr;

    public Print(Expression expression)
    {
        Expr = expression;
        Type = Type.String;
    }

    public override bool Validate(Context context)
    {
        return Expr.Validate(context);
    }

    public override string Evaluate(Context context)
    {
        return Expr.Evaluate(context);
    }
}