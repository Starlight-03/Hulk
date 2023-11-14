namespace HULK;

public class LetIn : Expression // Esta expresión define variables que se pueden utilizar luego en un ámbito de una expresión interna
{
    // Toda expresión "let-in" posee una lista de pares, representando los identificadores de las variables definidas y sus expresiones que les dan valor
    // Además de una expresión interna que sería el cuerpo del "in"

    // Al validar la expresión primero se definen todas las variables declaradas en un contexto interno y se les asignan sus valores,
    // y luego se valida el cuerpo en el contexto interno
    // Para evaluar hace el mismo proceso de definir variables en un contexto interno, y luego se evalúa el cuerpo

    private readonly Dictionary<string, Expression> Variables;
    
    private readonly Expression Body;

    public LetIn(Dictionary<string, Expression> variables, Expression body)
    {
        Variables = variables;
        Body = body;
    }

    public override bool Validate(Context context)
    {
        Context innerContext = new Context(context);

        foreach (string variable in Variables.Keys){
            if (!Variables[variable].Validate(context))
                return false;
            innerContext.Define(variable);
            innerContext.SetValue(variable, Variables[variable]);
        }

        if (!Body.Validate(innerContext)){
            return false;
        }
        
        Type = Body.Type;
        return true;
    }

    public override string Evaluate(Context context)
    {
        Context innerContext = new Context(context);

        foreach (string variable in Variables.Keys){
            innerContext.Define(variable);
            innerContext.SetValue(variable, Variables[variable]);
        }

        return Body.Evaluate(innerContext);
    }
}