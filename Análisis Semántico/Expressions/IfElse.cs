namespace HULK;

public class IfElse : Expression // Esta expresión define, valida y evalúa las expresiones "if-else"
{
    // Estas expresiones poseen tres expresiones internas: una condicional dentro del if, una expresión de 
    // caso positiva que va después del condicional, y una expresión de caso negativo que va después de la 
    // palabra reservada "else"

    // Para validar solo hace falta validar las tres expresiones internas
    // Para evaluar, se evalúa primero la expresión condicional y, en caso de que sea positiva, se devuelve 
    // el valor de la expresión positiva, en caso de que sea negativa, se devuelve el valor de la expresión negativa
    
    private readonly Expression Condition;

    private readonly Expression Positive;

    private readonly Expression Negative;

    public IfElse(Expression condition, Expression positive, Expression negative)
    {
        Condition = condition;
        Positive = positive;
        Negative = negative;
    }

    public override bool Validate(Context context) 
    => Condition.Validate(context) && Positive.Validate(context) && Negative.Validate(context);

    public override string Evaluate(Context context) 
    => bool.Parse(Condition.Evaluate(context)) ? Positive.Evaluate(context) : Negative.Evaluate(context);
}