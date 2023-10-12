public abstract class Error
{
    protected ErrorType type;
    
    protected string info = "";

    public void Show()
    {
        Console.WriteLine("! {0} ERROR: {1}", type, info);
    }
}

public enum ErrorType
{
    LEXICAL,
    SYNTAX,
    SEMANTIC
}

public class LexError : Error
{
    public LexError(string token) : base()
    {
        this.info = $"\'{token}\' is not a valid token.";
        this.type = ErrorType.LEXICAL;
        Show();
    }
}

public class SyntaxError : Error
{
    public SyntaxError(string info) : base()
    {
        this.type = ErrorType.SYNTAX;
    }
}

public class SemanticError : Error
{
    public SemanticError(string info) : base()
    {
        this.type = ErrorType.SEMANTIC;
    }
}