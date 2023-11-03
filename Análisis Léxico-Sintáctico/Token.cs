namespace HULK;

public class Token
{
    public string Value { get; private set; }

    public TokenType Type { get; private set; }

    public Token(string value, TokenType type)
    {
        Value = value;
        Type = type;
    }

    public static Token GetToken(string token)
    {
        if (TokenValues.ContainsKey(token))
            return TokenValues[token];
        else
            return TokenValues["null"];
    }

    public static bool IsToken(string token)
    {
        return TokenValues.ContainsKey(token);
    }

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
    Expression,
    NumericLiteral,
    StringLiteral,
    BooleanLiteral,
    Null
}