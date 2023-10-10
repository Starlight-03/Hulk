public class Token
{
    public string Value { get; private set; }

    public TokenType Type { get; private set; }

    public Token(string value, TokenType key)
    {
        this.Value = value;
        this.Type = key;
    }
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

public static class TokenValues
{
    public static Dictionary<string, Token> Grammar = new Dictionary<string, Token>()
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
        {"false", new Token("false", TokenType.BooleanLiteral)}
    };
}