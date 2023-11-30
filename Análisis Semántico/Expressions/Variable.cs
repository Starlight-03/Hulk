namespace HULK;

public class Variable : Expression // Esta expresión se utiliza para el llamado de variables, auxiliándose de la calse Context
{
    // Toda variable posee un identificador
    // Al validarse se debe revisar si fue definida anteriormente, entonces se define su tipo y se valida
    // Si se llama a una variable que no haya sido definida anteriormente en este contexto, se lanza una excepción
    // Para evaluarla, se devuelve el valor que debe tener guardado el contexto en el que se encuentra
    
    public string Identifier;

    public Variable(string identifier) { Identifier = identifier; }

    public override void SetType(Context context, Type type)
    {
        context.SetType(Identifier, type);
        Type = type;
    }

    public override bool Validate(Context context)
    {
        if (context.IsDefined(Identifier)){
            Type = context.GetType(Identifier);
            return true;
        }
        Semantic.Show($"Variable \'{Identifier}\' has not been defined in this context.");
        return false;
    }

    public override string Evaluate(Context context) => context.GetValue(Identifier);
}