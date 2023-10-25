internal class Program
{
    private static void Main(string[] args)
    {
        Context globalContext = InitializeGlobalContext();
        while (true){
            Console.Write("> ");
            // string line = Console.ReadLine();
            string line = "function fib(n) => if (n > 1) fib(n-1) + fib(n-2) else 1;"; Console.WriteLine(line);
            if (line == "")
                break;
            
            var lexer = new Lexer(line);
            if (lexer.Tokens == null){
                lexer.Lex.Show();
                continue;
            }

            var parser = new Parser(lexer.Tokens);
            if (parser.Expr == null){
                parser.Syntax.Show();
                continue;
            }

            var AST = new AbstractSintaxisTree(parser.Expr, globalContext);
            if (!AST.Valid){
                AST.Semantic.Show();
                continue;
            }

            if (AST.Value != "")
                Console.WriteLine(parser.Expr.Value);

            Console.Write("> ");
            line = "fib(5);"; Console.WriteLine(line);
            if (line == "")
                break;
            lexer = new Lexer(line);
            if (lexer.Tokens == null){
                lexer.Lex.Show();
                continue;
            }
            parser = new Parser(lexer.Tokens);
            if (parser.Expr == null){
                parser.Syntax.Show();
                continue;
            }
            AST = new AbstractSintaxisTree(parser.Expr, globalContext);
            if (!AST.Valid){
                AST.Semantic.Show();
                continue;
            }
            if (AST.Value != "")
                Console.WriteLine(parser.Expr.Value);
        }
    }

    private static Context InitializeGlobalContext()
    {
        Context context = new Context(null);
        context.Define("PI");
        context.SetValue("PI", new Number(Math.PI.ToString()));
        context.Define("E");
        context.SetValue("E", new Number(Math.E.ToString()));
        context.Define("sin", new string[] {"x"}, 
                       new PredeterminedFunction(PredeterminedFunctions.sin, new Variable("x")));
        context.Define("cos", new string[] {"x"}, 
                       new PredeterminedFunction(PredeterminedFunctions.cos, new Variable("x")));
        context.Define("log", new string[] {"a", "b"}, 
                       new PredeterminedFunction(PredeterminedFunctions.log, new Variable("a"), 
                                                                             new Variable("b")));
        context.Define("ln", new string[] {"x"}, 
                       new PredeterminedFunction(PredeterminedFunctions.ln, new Variable("x")));

        return context;
    }
}