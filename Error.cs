public abstract class Error
{
    protected ErrorType type;
    private string info;
    public Error(string info)
    {
        this.info = info;
    }
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
    public LexError(string info) : base(info)
    {
        this.type = ErrorType.LEXICAL;
    }
}

public class SyntaxError : Error
{
    public SyntaxError(string info) : base(info)
    {
        this.type = ErrorType.SYNTAX;
    }
}

public class SemanticError : Error
{
    public SemanticError(string info) : base(info)
    {
        this.type = ErrorType.SEMANTIC;
    }
}