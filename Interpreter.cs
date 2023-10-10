public class Interpreter
{
    public Interpreter()
    {

    }

    public void Start()
    {
        while (true)
        {
            Console.Write("> ");
            string line = Console.ReadLine();
            
            // string line = "4+64;";
            // Console.WriteLine(line);

            if (line == "")
                break;
            
            Lexer lexer = new Lexer(line);
            List<Token> tokens = lexer.GetTokens();
            
            if (tokens == null) 
                break;
            
            foreach (Token token in tokens)
                Console.WriteLine(token.Value);
            // Parser parser = new Parser();
            // parser.Parse(tokens);
            
            // break;
        }
    }
}