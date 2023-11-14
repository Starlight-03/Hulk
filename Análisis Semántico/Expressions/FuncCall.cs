namespace HULK;

public class FuncCall : Expression // Esta clase nos ayuda con el llamado de las funciones, si ya están definidas
{
    // Toda expresión de llamado a una función posee el identificador de la expresión, y una lista de argumentos 
    // que puede tener la función
    
    // Para validar primero se revisa si la función está definida o no, luego se validan todos los argumentos, 
    // y luego se revisa y se define el tipo de valor que devuelve la función, a la vez que revisa la cantidad 
    // de argumentos que acepta la función y los tipos de los argumentos
    // Si algún argumento no es del mismo tipo del que se acepta en la variable, o la cantidad de argumentos no es
    // la misma que acepta la función, se lanza un error

    // Para evaluar se busca la función con la cantidad de argumentos deseada y su corresponidente cuerpo
    // y luego se definen en el contexto interno los valores sus argumentos y luego se evalúa el cuepo de la función

    public string Identifier;

    public Expression[] Args;

    public FuncCall(string identifier, List<Expression> args)
    {
        Identifier = identifier;
        
        Args = new Expression[args.Count];
        for (int i = 0; i < args.Count; i++){
            Args[i] = args[i];
        }
    }

    public override bool Validate(Context context)
    {
        if (!context.IsDefined(Identifier, Args.Length)){
            return false;
        }
        foreach (Expression arg in Args){
            if (!arg.Validate(context)){
                return false;
            }
        }
        if (!CheckArgumentTypes(Identifier, Args, context)){
            return false;
        }
        return true;
    }

    private bool CheckArgumentTypes(string function, Expression[] args, Context context)
    {
        Dictionary<string[], (Expression, Context)>? func = context.GetFunction(function);
        if (func != null){
            int maxArgs = 0;
            foreach (string[] arguments in func.Keys){
                maxArgs = Math.Max(maxArgs, arguments.Length);
                if (arguments.Length == args.Length){
                    (Expression expr, Context innerContext) = func[arguments];
                    for (int i = 0; i < args.Length; i++){
                        args[i].Validate(context);
                        innerContext.SetType(arguments[i], args[i].Type);
                        Type argType = args[i].Type;
                        Type setType = innerContext.GetType(arguments[i]);
                        if (argType != Type.NotSet && setType != argType){
                            Semantic.Show($"Function \'{function}\' receives \'{setType}\', not \'{argType}\'.");
                            return false;
                        }
                    }
                    Type = expr.Type;
                    return true;
                }
            }
            Semantic.Show($"Function \'{function}\' receives a maximum of {maxArgs} argument(s), but {args.Length} were given.");
        }
        return false;
    }

    public override string Evaluate(Context context)
    {
        Dictionary<string[], (Expression, Context)>? func = context.GetFunction(Identifier);
        if (func != null){
            foreach (string[] arguments in func.Keys){
                if (arguments.Length == Args.Length){
                    Context innerContext = new Context(context);
                    Expression expr = func[arguments].Item1;
                    for (int i = 0; i < Args.Length; i++){
                        innerContext.Define(arguments[i]);
                        innerContext.SetValue(arguments[i], Args[i]);
                    }
                    return expr.Evaluate(innerContext);
                }
            }
        }
        return "";
    }
}