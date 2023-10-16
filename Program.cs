internal class Program
{
    private static void Main(string[] args)
    {
        Interpreter();
    }

    public static void Interpreter()
    {
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
            if (parser.Parse())
                Console.WriteLine(true);
            else
                Console.WriteLine(false);
            
            break;
        }
    }
}