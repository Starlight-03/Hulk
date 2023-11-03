namespace HULK;

public class FuncDef : Expression
{
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

    public override string Evaluate(Context context)
    {
        return "";
    }
}