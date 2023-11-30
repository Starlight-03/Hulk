namespace HULK;

public abstract class PredeterminedFunction : Expression // Estas funciones son un poco especiales, ya que no se definen en el contexto como las demás
{
    // Estas funciones ya son válidas de por sí, así que sólo se devuelve true al validarlas
    // Estas expresiones no tienen un valor fijo ni un valor inicial al definirlas, así que no podemos utilizar este valor 
    // (pero en el resto se comporta como una expresión literal)
    // Son llamadas a validarse y evaluarse por los llamados de función, así que también se comportan algo similar a las FunctionCall

    // Toda función predeterminada posee una lista de argumentos que puede aceptar

    // Al evaluar tomamos los valores de los argumentos dados por la llamada a la función hecha anteriormente, 
    // y luego se evalúa en el tipo de función que sea

    // Hasta ahora están predefinidas las funciones: sin(x), cos(x), log(a, b), ln(x)

    protected readonly string[] Args;

    public PredeterminedFunction(string[] args) { Args = args; }

    public override bool Validate(Context context) => true;
}

public class Sin : PredeterminedFunction
{
    public Sin(string[] args) : base(args) { Type = Type.Number; }

    public override string Evaluate(Context context) 
    => context.IsDefined("sin", Args.Length) ? Math.Round(Math.Sin(double.Parse(context.GetValue(Args[0])))).ToString() : "";
}

public class Cos : PredeterminedFunction
{
    public Cos(string[] args) : base(args) { Type = Type.Number; }

    public override string Evaluate(Context context) 
    => context.IsDefined("cos", Args.Length) ? Math.Round(Math.Cos(double.Parse(context.GetValue(Args[0])))).ToString() : "";
}

public class Log : PredeterminedFunction
{
    public Log(string[] args) : base(args) { Type = Type.Number; }

    public override string Evaluate(Context context) 
    => context.IsDefined("log", Args.Length) ? Math.Round(Math.Log(double.Parse(context.GetValue(Args[1])), double.Parse(context.GetValue(Args[0])))).ToString() : "";
}

public class Ln : PredeterminedFunction
{
    public Ln(string[] args) : base(args) { Type = Type.Number; }

    public override string Evaluate(Context context) 
    => context.IsDefined("ln", Args.Length) ? Math.Round(Math.Log(double.Parse(context.GetValue(Args[0])))).ToString() : "";
}