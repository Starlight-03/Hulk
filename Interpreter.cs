public class Interpreter
{
    public void Start()
    {
        while (true)
        {
            Console.Write("> ");
            // string line = Console.ReadLine();
            
            string line = "(5 + 1) * (2 - 3) / 4;";
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