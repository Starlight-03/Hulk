namespace HULK;

public class FuncCall : Expression
{
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

    public override void SetType(Context context, Type type)
    {
        Type = type;
    }

    public override bool Validate(Context context)
    {
        foreach (Expression arg in Args){
            if (!arg.Validate(context)){
                return false;
            }
        }
        if (!CheckArgumentTypes(Identifier, Args, context)){
            return false;
        }
        if (!context.IsDefined(Identifier, Args.Length)){
            return false;
        }
        return true;
    }

    private bool CheckArgumentTypes(string function, Expression[] args, Context context)
    {
        Dictionary<string[], (Expression, Context)> func = context.GetFunction(function);
        int maxArgs = 0;
        foreach (string[] arguments in func.Keys){
            maxArgs = Math.Max(maxArgs, arguments.Length);
            if (arguments.Length == args.Length){
                (Expression expr, Context innerContext) = func[arguments];
                for (int i = 0; i < args.Length; i++){
                    if (!innerContext.SetType(arguments[i], args[i].Type)){
                        Semantic.Show($"Function \'{function}\' receives \'{innerContext.GetType(arguments[i])}\', not \'{args[i].Type}\'.");
                        return false;
                    }
                }
                return true;
            }
        }
        Semantic.Show($"Function \'{function}\' receives a maximum of {maxArgs} argument(s), but {args.Length} were given.");
        return false;
    }

    public override string Evaluate(Context context)
    {
        Dictionary<string[], (Expression, Context)> func = context.GetFunction(Identifier);
        foreach (string[] arguments in func.Keys){
            if (arguments.Length == Args.Length){
                (Expression expr, Context innerContext) = func[arguments];
                for (int i = 0; i < Args.Length; i++){
                    innerContext.SetValue(arguments[i], Args[i]);
                }
                return expr.Evaluate(innerContext);
            }
        }
        return "";
    }
}