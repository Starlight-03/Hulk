public abstract class Error
{
    protected ErrorType type;

    protected string info;

    public Error()
    {
        info = "";
    }
    
    public void Show()
    {
        Console.WriteLine("! {0} ERROR: {1}", type, info);
    }

    public string Info { get { return info; } set { if (info == "") info = value; } }
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