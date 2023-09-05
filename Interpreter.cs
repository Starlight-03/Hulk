public class Interpreter
{
    public Interpreter()
    {

    }

    public void Start()
    {
        Scanner scanner = new Scanner();
        while (true)
        {
            Console.Write("> ");
            // string line = Console.ReadLine();
            string line = "print(\"Hello World\");";
            System.Console.WriteLine(line);
            scanner.Tokenize(line);
            break;
        }
    }
}