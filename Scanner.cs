public class Scanner
{
    private LexError lexError;
    
    public Scanner()
    {

    }

    public void Tokenize(string line)
    {
        if (line.EndsWith(';'))
        {
            List<Token> tokens = Read(line);
            Parser parser = new Parser();
            parser.Parse(tokens);
        }
        else 
        {
            lexError = new LexError("Missing ';'");
            lexError.Show();
        }
    }

    private List<Token> Read(string line)
    {
        List<Token> tokens = new List<Token>();

        string token = "";

        for (int i = 0; i < line.Length; i++)
        {
            if (IsToken(token))
            {
                AddToken(tokens, token);
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

                tokens.Add(new Token(Token.TokenType.Literal, expression));
            }
        }

        AddToken(tokens, token);
        
        return tokens;
    }

    private bool IsToken(string s)
    {
        foreach (Token token in Token.Tokens)
            if (s == token.Value)
                return true;
        
        return false;
    }

    private static void AddToken(List<Token> tokens, string value)
    {
        foreach (Token token in Token.Tokens)
            if (token.Value == value)
                tokens.Add(token);
    }
}

public class Token
{
    public static Token[] Tokens = new Token[]
    {
        new Token(TokenType.Separator, "("),
        new Token(TokenType.Separator, ")"),
        new Token(TokenType.Separator, ";"),
        new Token(TokenType.Keyword, "print"),
        new Token(TokenType.Keyword, "function"),
        new Token(TokenType.Operator, "+"),
        new Token(TokenType.Operator, "-"),
        new Token(TokenType.Operator, "*"),
        new Token(TokenType.Operator, "/"),
        new Token(TokenType.Operator, "="),
        new Token(TokenType.Operator, "%"),
        new Token(TokenType.Operator, "^"),
        new Token(TokenType.Operator, "=>"),
        new Token(TokenType.Keyword, "let"),
        new Token(TokenType.Keyword, "in"),
        new Token(TokenType.Operator, "<"),
        new Token(TokenType.Operator, "<="),
        new Token(TokenType.Operator, ">"),
        new Token(TokenType.Operator, ">="),
        new Token(TokenType.Operator, "=="),
        new Token(TokenType.Operator, "!="),
        new Token(TokenType.Keyword, "if"),
        new Token(TokenType.Keyword, "else")
    };

    private TokenType key;
    private string value;
    
    public Token(TokenType key, string value)
    {
        this.key = key;
        this.value = value;
    }

    public TokenType Key { get { return this.key; } }
    public string Value { get { return this.value; } }

    public enum TokenType
    {
        Identifier,
        Keyword,
        Separator,
        Operator,
        Literal
    }
}