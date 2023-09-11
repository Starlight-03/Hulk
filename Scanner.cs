public class Scanner
{
    private LexError lexError;

    public List<Token> Tokens;
    
    public Scanner()
    {

    }

    public List<Token> GetTokens(string line)
    {
        if (line.EndsWith(';'))
        {
            this.Tokens = new List<Token>();

            string token = "";
            
            for (int i = 0; i <= line.Length; i++)
            {
                if (char.IsWhiteSpace(line[i]))
                    continue;

                if (i == line.Length && IsToken(token))
                    AddToken(this.Tokens, token);

                if (IsToken(token))
                {
                    AddToken(this.Tokens, token);
                    token = line[i].ToString();
                }
                else
                    token += line[i];

                if (line[i] == '\"')
                {
                    i++;
                    token = "";
                    string expression = "";

                    for (; i < line.Length && line[i] != '\"'; i++)
                        expression += line[i];

                    this.Tokens.Add(new Token(TokenType.Expression, expression));
                }
            }
            
            return this.Tokens;
        }
        else 
        {
            lexError = new LexError("Missing ';'");
            lexError.Show();
            return null;
        }
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
        {"else", TokenType.Keyword}
    };
}