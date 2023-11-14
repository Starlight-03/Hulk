namespace HULK;

public class PredeterminedFunction : LiteralExpression // Estas expresiones literales son un poco especiales, ya que dependen del valor de su(s) argumento(s)
{
    // Al igual que las expresiones literales, estas ya son válidas de por sí, así que sólo se devuelve true al validarlas
    // Estas expresiones no tienen un valor fijo ni un valor inicial al definirlas, así que no podemos utilizar este valor 
    // (pero en el resto se comporta como una expresión literal)
    // Son llamadas a validarse y evaluarse por los llamados de función, así que también se comportan algo similar a las FunctionCall

    // Toda función predeterminada posee un enumerador que actúa como identificador, y una lista de argumentos que puede aceptar

    // Al evaluar tomamos los valores de los argumentos dados por la llamada a la función hecha anteriormente, 
    // y luego se evalúa en el tipo de función que sea

    // Hasta ahora están predefinidas las funciones: sin(x), cos(x), log(a, b), ln(x)

    private readonly PredFunc Func;

    private readonly string[] Args;

    public PredeterminedFunction(PredFunc func, Type type, string[] args) : base("")
    {
        Func = func;
        Type = type;
        Args = args;
    }

    public override string Evaluate(Context context)
    {
        if (context.IsDefined(Func.ToString(), Args.Length)){
            string[] values = new string[Args.Length];
            for (int i = 0; i < Args.Length; i++){
                values[i] = context.GetValue(Args[i]);
            }
            return GetValue(values);
        }
        return "";
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