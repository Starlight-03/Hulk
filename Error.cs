public abstract class Error
{
    protected ErrorType type;

    public string Info { get; set; }

    public Error()
    {
        Info = "";
    }
    
    public void Show()
    {
        Console.WriteLine("! {0} ERROR: {1}", type, Info);
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
    public LexError() : base()
    {
        type = ErrorType.LEXICAL;
    }
}

public class SyntaxError : Error
{
    public SyntaxError() : base()
    {
        type = ErrorType.SYNTAX;
    }
}

public class SemanticError : Error
{
    public SemanticError() : base()
    {
        type = ErrorType.SEMANTIC;
    }
}