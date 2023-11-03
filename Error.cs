namespace HULK;

public abstract class Error
{
    protected ErrorType type;

    private bool hasShowed;
    
    public void Show(string info)
    {
        if (!hasShowed && info != ""){
            Console.WriteLine("! {0} ERROR: {1}", type, info);
            hasShowed = true;
        }
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