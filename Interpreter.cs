public class Interpreter
{
    public void Start()
    {
        while (true)
        {
            Console.Write("> ");
            // string line = Console.ReadLine();
            
            string line = "function fib(n) => if (n > 1) fib(n-1) + fib(n-2) else 1;";
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