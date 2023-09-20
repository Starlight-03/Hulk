public class Lexer
{
    private LexError? lexError;

    public List<Token> Tokens;

    private string line;

    private int i;

    private int N => this.line.Length;
    
    public Lexer(string line)
    {
        this.Tokens = new List<Token>();
        this.line = line;
        i = 0;
    }

    public List<Token> GetTokens()
    {
        for (; i <= this.N; i++)
        {
            if (char.IsWhiteSpace(Look()))
                continue;
            // if (char.IsDigit())
            //     this.Tokens
            if (char.IsLetter(Look()))
                this.Tokens.Add(GetToken());


            // if (i == line.Length && IsToken(token))
            //     AddToken(this.Tokens, token);

            // if (IsToken(token))
            // {
            //     AddToken(this.Tokens, token);
            //     token = line[i].ToString();
            // }
            // else
            //     token += line[i];

            // if (line[i] == '\"')
            // {
            //     i++;
            //     token = "";
            //     string expression = "";

            //     for (; i < line.Length && line[i] != '\"'; i++)
            //         expression += line[i];

            //     this.Tokens.Add(new Token(TokenType.Expression, expression));
            // }
        }
        
        return this.Tokens;
    }

    private void GoForward()
    {
        if (i < this.N - 1)
            i++;
    }

    private void GoBack()
    {
        if (i > 0)
            i--;
    }

    private char Look()
    {
        return this.line[i];
    }

    private char LookAhead()
    {
        return this.line[i + 1];
    }

    private char LookBack()
    {
        return this.line[i - 1];
    }

    private Token GetToken()
    {
        string token = "";

        for (; i < this.line.Length; i++)
        {
            if (IsToken(token) && !IsToken(token + LookAhead()))
            {
                if (Grammar.ValidTokens.ContainsKey(token))
                    return new Token(Grammar.ValidTokens[token], token);
                else
                    return new Token(TokenType.Identifier, token);
            }
            else
                token += Look();
        }

        return null;
    }

    private Token GetNumber()
    {
        string s = "";

        for (int i = 0; i < this.N; i++)
        {
            char c = Look();
            if (char.IsDigit(c))
                s += c;
            // else if (char.IsPunctuation(c))

        }
        return null;
    }

    private static bool IsToken(string s)
    {
        return Grammar.ValidTokens.ContainsKey(s);
    }

    private static void AddToken(List<Token> tokens, string value)
    {
        if (IsToken(value))
            tokens.Add(new Token(Grammar.ValidTokens[value], value));
    }
}

public class Token
{
    public TokenType Type { get; private set; }
    
    public string Value { get; private set; }

    public Token(TokenType key, string value)
    {
        this.Type = key;
        this.Value = value;
    }
}

public enum TokenType
{
    Identifier,
    Keyword,
    Separator,
    Operator,
    Expression
}

public class Grammar
{
    public static Dictionary<string, TokenType> ValidTokens = new Dictionary<string, TokenType>()
    {
        {"+", TokenType.Operator},
        {"-", TokenType.Operator},
        {"*", TokenType.Operator},
        {"/", TokenType.Operator},
        {"%", TokenType.Operator},
        {"^", TokenType.Operator},
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
        {";", TokenType.Separator},
        {"print", TokenType.Keyword},
        {"function", TokenType.Keyword},
        {"let", TokenType.Keyword},
        {"in", TokenType.Keyword},
        {"if", TokenType.Keyword},
        {"else", TokenType.Keyword},
        {"true", TokenType.Expression},
        {"false", TokenType.Expression}
    };
}