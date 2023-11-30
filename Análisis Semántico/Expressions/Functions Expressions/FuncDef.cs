namespace HULK;

public class FuncDef : Expression // Esta expresión nos ayudará a definir funciones que se guardarán en el contexto global
{
    // Toda definición de una función posee el identificador de la función, los argumentos que recibe (que se considerarían variables internas), 
    // y una expresión que sería el cuerpo de la función
    // Durante la validación se definirá y utilizará un contexto interno en donde se definirán las variables argumentos de la función
    // Primero se definen los argumentos en el contexto interno, y luego en el contexto padre se definirá la función y se valida el cuerpo
    // Si el cuerpo no es válido, indefinimos (eliminamos del contexto) la función
    private readonly string Identifier;

    private readonly string[] Args;

    private readonly Expression Body;

    public FuncDef(string identifier, List<string> args, Expression body)
    {
        Identifier = identifier;
        
        Args = new string[args.Count];
        for (int i = 0; i < args.Count; i++)
            Args[i] = args[i];

        Body = body;
    }

    public override bool Validate(Context context)
    {
        Context innerContext = new Context(context);
        foreach (string arg in Args){
            innerContext.Define(arg);
        }
        context.Define(Identifier, Args, Body, innerContext);
        if (Body.Validate(innerContext)){
            return true;
        }
        context.Undefine(Identifier, Args);
        return false;
    }

    public override string Evaluate(Context context) => "";
}