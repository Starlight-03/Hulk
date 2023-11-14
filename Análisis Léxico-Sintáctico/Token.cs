namespace HULK;

public class Token  // Utilizaremos esta clase como herramienta para crear los "tokens"
{
    // Los tokens son una herramienta para facilitar el trabajo a la hora del parseo
    // Consisten en una dupla de tipo y valor:
    // - En el valor se guardará el "nombre" del token, lo que nosotros podemos leer
    // - En el tipo de guarda el tipo de token que sea:
    //   - Identificador
    //   - Palabra reservada (Keyword)
    //   - Separador
    //   - Operador
    //   - Literales (numéricos, booleanos o de string)
    //   - Null (en caso de tener que devolver un token que no exista)

    public string Value { get; private set; }

    public TokenType Type { get; private set; }

    public Token(string value, TokenType type)
    {
        Value = value;
        Type = type;
    }

    public static Token GetToken(string token) // Devuelve un token de los predeterminados en nuestro lenguaje
    {
        if (TokenValues.ContainsKey(token))
            return TokenValues[token];
        else
            return TokenValues["null"]; 
            // En caso de no pertenecer el token, se devuelve un token "nulo"
    }

    public static bool IsToken(string token) // Devuelve si un token dado pertenece a la lista de tokens predeterminados
    {
        return TokenValues.ContainsKey(token);
    }

    // Nuestra lista de tokens predeterminados, con un método de acceso algo más cómodo
    // al acceder a estos tokens indexándolos por su valor
    private static Dictionary<string, Token> TokenValues = new Dictionary<string, Token>()
    {
        {"+", new Token("+", TokenType.Operator)},
        {"-", new Token("-", TokenType.Operator)},
        {"*", new Token("*", TokenType.Operator)},
        {"/", new Token("/", TokenType.Operator)},
        {"%", new Token("%", TokenType.Operator)},
        {"^", new Token("^", TokenType.Operator)},
        {"@", new Token("@", TokenType.Operator)},
        {"=", new Token("=", TokenType.Operator)},
        {"=>", new Token("=>", TokenType.Operator)},
        {"<", new Token("<", TokenType.Operator)},
        {"<=", new Token("<=", TokenType.Operator)},
        {">", new Token(">", TokenType.Operator)},
        {">=", new Token(">=", TokenType.Operator)},
        {"==", new Token("==", TokenType.Operator)},
        {"!=", new Token("!=", TokenType.Operator)},
        {"!", new Token("!", TokenType.Operator)},
        {"&", new Token("&", TokenType.Operator)},
        {"|", new Token("|", TokenType.Operator)},
        {"\"", new Token("\"", TokenType.Operator)},
        {"(", new Token("(", TokenType.Separator)},
        {")", new Token(")", TokenType.Separator)},
        {",", new Token(",", TokenType.Separator)},
        {";", new Token(";", TokenType.Separator)},
        {"print", new Token("print", TokenType.Keyword)},
        {"function", new Token("function", TokenType.Keyword)},
        {"let", new Token("let", TokenType.Keyword)},
        {"in", new Token("in", TokenType.Keyword)},
        {"if", new Token("if", TokenType.Keyword)},
        {"else", new Token("else", TokenType.Keyword)},
        {"true", new Token("true", TokenType.BooleanLiteral)},
        {"false", new Token("false", TokenType.BooleanLiteral)},
        {"null", new Token("", TokenType.Null)}
    };
}

public enum TokenType
{
    Identifier,
    Keyword,
    Separator,
    Operator,
    NumericLiteral,
    StringLiteral,
    BooleanLiteral,
    Null
}