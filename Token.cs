public class Token
{
    public TokenType Type { get; private set; }
    
    public string Value { get; private set; }

    public Token(TokenType key, string value)
    {
        this.Type = key;
        this.Value = value;
    }

    public static Dictionary<string, TokenType> Grammar = new Dictionary<string, TokenType>()
    {
        {"+", TokenType.Operator},
        {"-", TokenType.Operator},
        {"*", TokenType.Operator},
        {"/", TokenType.Operator},
        {"%", TokenType.Operator},
        {"^", TokenType.Operator},
        {"@", TokenType.Operator},
        {"=", TokenType.Operator},
        {"=>", TokenType.Operator},
        {"<", TokenType.Operator},
        {"<=", TokenType.Operator},
        {">", TokenType.Operator},
        {">=", TokenType.Operator},
        {"==", TokenType.Operator},
        {"!=", TokenType.Operator},
        {"(", TokenType.Separator},
        {")", TokenType.Separator},
        {",", TokenType.Separator},
        {";", TokenType.Separator},
        {"print", TokenType.Keyword},
        {"function", TokenType.Keyword},
        {"let", TokenType.Keyword},
        {"in", TokenType.Keyword},
        {"if", TokenType.Keyword},
        {"else", TokenType.Keyword},
        {"true", TokenType.BooleanLiteral},
        {"false", TokenType.BooleanLiteral}
    };
}

public enum TokenType
{
    Identifier,
    Keyword,
    Separator,
    Operator,
    Expression,
    NumericLiteral,
    StringLiteral,
    BooleanLiteral
}