namespace HULK;

internal class Program
{
    private static void Main(string[] args)
    {
        Context globalContext = InitializeGlobalContext();
        Expression expression;
        string value;

        while (true){
            Console.Write("> ");
            
            string line = Console.ReadLine();
            if (line == "")
                break;

            var lexer = new Lexer();
            var tokens = lexer.Tokenize(line);
            if (tokens == null)
                continue;

            var parser = new Parser();
            expression = parser.Parse(tokens);
            if (expression == null)
                continue;

            if (!expression.Validate(globalContext))
                continue;
            
            value = expression.Evaluate(globalContext);
            if (value != "")
                Console.WriteLine(value);
        }
    }

    private static Context InitializeGlobalContext()
    {
        Context context = new Context(null);

        context.Define("PI");
        context.SetValue("PI", new NumericLiteral(Math.PI.ToString()));
        context.Define("E");
        context.SetValue("E", new NumericLiteral(Math.E.ToString()));

        context.Define("sin", new string[] {"x"}, new PredeterminedFunction(PredFunc.sin, Type.Number, "x"), new Context(context));
        context.Define("cos", new string[] {"x"}, new PredeterminedFunction(PredFunc.cos, Type.Number, "x"), new Context(context));
        context.Define("log", new string[] {"a", "b"}, new PredeterminedFunction(PredFunc.log, Type.Number, "a", "b"), new Context(context));
        context.Define("log", new string[] {"x"}, new PredeterminedFunction(PredFunc.ln, Type.Number, "x"), new Context(context));

        return context;
    }
}