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
            // string line = Console.ReadLine();
            string line = "print(\"Hello World\");";
            System.Console.WriteLine(line);
            Lexer lexer = new Lexer(line);
            List<Token> tokens = lexer.GetTokens();
            if (tokens != null)
            {
                // Parser parser = new Parser();
                // parser.Parse(tokens);
            }
            break;
        }
    }
}