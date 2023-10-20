internal class Program
{
    private static void Main(string[] args)
    {
        Interpreter();
    }

    public static void Interpreter()
    {
        Context context = new Context(null);

        while (true)
        {
            Console.Write("> ");
            // string line = Console.ReadLine();
            
            string line = "print(\"Hello, World!\");";
            Console.WriteLine(line);

            if (line == "")
                break;
            
            Lexer lexer = new Lexer(line);
            List<Token> tokens = lexer.GetTokens();
            
            if (tokens == null) 
                break;
            
            Parser parser = new Parser(tokens);
            Expression expression = parser.Parse();

            if (expression != null && expression.Validate(context))
            {
                expression.Evaluate(context);
                if (expression.Value != "")
                    Console.WriteLine(expression.Value);
            }
        }
    }
}