namespace HULK;

public abstract class Error // Los errores son una herramienta del compilador que notifica al usuario si ha cometido un error
{
    // Los errores contienen un tipo de error (Léxico, Sintáctico y Semántico) y 
    // una descripción del error como información

    protected ErrorType type;

    private bool hasShowed;
    
    public void Show(string info) // Lanza el error con la información asignada
    {
        // Los errores al ser lanzados tienen la forma: "! <Tipo de error> ERROR: <descripción>"

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