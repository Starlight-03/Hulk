namespace HULK;

public class PredeterminedFunction : Expression
{
    private readonly PredFunc Func;

    private readonly string[] Args;

    public PredeterminedFunction(PredFunc func, Type type, params string[] args)
    {
        Func = func;
        Type = type;
        
        Args = new string[args.Length];
        for (int i = 0; i < args.Length; i++){
            Args[i] = args[i];
        }
    }

    public override bool Validate(Context context)
    {  
        return true;
    }

    public override string Evaluate(Context context)
    {
        Context innerContext = new Context(context);

        foreach (string arg in Args){
            innerContext.Define(arg);
        }
        string[] values = new string[Args.Length];
        for (int i = 0; i < Args.Length; i++){
            values[i] = innerContext.GetValue(Args[i]);
        }

        return GetValue(values);
    }

    private string GetValue(string[] values)
    {
        switch (Func){
            case PredFunc.sin:
                return Math.Round(Math.Sin(double.Parse(values[0]))).ToString();
            case PredFunc.cos:
                return Math.Round(Math.Cos(double.Parse(values[0]))).ToString();
            case PredFunc.log:
                return Math.Round(Math.Log(double.Parse(values[1]), double.Parse(values[0]))).ToString();
            case PredFunc.ln:
                return Math.Round(Math.Log(double.Parse(values[0]))).ToString();
            default:
                return "";
        }
    }
}

public enum PredFunc
{
    sin,
    cos,
    log,
    ln
}