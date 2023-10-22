internal class Program
{
    private static void Main(string[] args)
    {
        Interpreter();
    }

    public static void Interpreter()
    {
        Context context = InitializeGlobalContext();
        string line = "";

        do {
            Console.Write("> ");
            line = Console.ReadLine();
            
            Lexer lexer = new Lexer(line);
            List<Token> tokens = lexer.GetTokens();
            
            if (tokens != null){
                Parser parser = new Parser(tokens);
                Expression expression = parser.Parse();

                if (expression != null && expression.Validate(context)){
                    expression.Evaluate(context);
                    if (expression.Value != "")
                        Console.WriteLine(expression.Value);
                }
            }
            else
                break;
        } while (line != "");
    }

    private static Context InitializeGlobalContext()
    {
        Context context = new Context(null);
        context.Define("PI");
        context.SetValue("PI", new Number(Math.PI.ToString()));
        context.Define("sin", new string[] {"x"}, new PredeterminedFunction(PredeterminedFunctions.sin, new Variable("x")));
        context.Define("cos", new string[] {"x"}, new PredeterminedFunction(PredeterminedFunctions.cos, new Variable("x")));
        context.Define("log", new string[] {"a", "b"}, new PredeterminedFunction(PredeterminedFunctions.log, new Variable("a"), new Variable("b")));
        context.Define("ln", new string[] {"x"}, new PredeterminedFunction(PredeterminedFunctions.ln, new Variable("x")));

        return context;
    }
}