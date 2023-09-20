internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine(char.IsPunctuation('.'));
        Console.WriteLine(char.IsPunctuation(','));
        Console.WriteLine(char.IsPunctuation(';'));
        Console.WriteLine(char.IsPunctuation(':'));
        Interpreter interpreter = new Interpreter();
        interpreter.Start();
    }
}