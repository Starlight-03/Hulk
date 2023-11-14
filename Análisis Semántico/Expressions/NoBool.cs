namespace HULK;

public class NoBool : Expression // Esta expresión es una representación del uso del operador booleano de negación
{
    // Simplemente posee una expresión booleana que será la que se negará como resultado de evaluar esta expresión
    // Para validarla, simplemente hay que validar la expresión booleana
    // Para evaluar, es solo negar la expresión dada
    
    private readonly Expression Boolean;

    public NoBool(Expression boolean)
    {
        Boolean = boolean;
        Type = Type.Boolean;
    }

    public override bool Validate(Context context)
    {
        if (Boolean.Validate(context) && Boolean.Type == Type.Boolean){
            return true;
        }
        Semantic.Show($"Operator \'!\' cannot be used with \'{Boolean.Type}\'");
        return false;
    }

    public override string Evaluate(Context context)
    {
        return Boolean.Evaluate(context);
    }
}