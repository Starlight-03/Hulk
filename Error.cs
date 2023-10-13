public abstract class Error
{
    protected ErrorType type;
    
    public void Throw(string info)
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
    public LexError() : base()
    {
        this.type = ErrorType.LEXICAL;
    }
}

public class SyntaxError : Error
{
    public SyntaxError() : base()
    {
        this.type = ErrorType.SYNTAX;
    }
}

public class SemanticError : Error
{
    public SemanticError() : base()
    {
        this.type = ErrorType.SEMANTIC;
    }
}