public class Lexer
{
    private LexError? lexError;

    public List<Token> Tokens;

    private readonly string line;

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
        for (; CanLook(); GoForward())
        {
            if (char.IsWhiteSpace(Look()))
                continue;
            else if (Look() == '\"')
                Tokens.Add(GetStringExpression());
            else if (char.IsLetterOrDigit(Look()) || char.IsPunctuation(Look()) || char.IsSymbol(Look()))
                Tokens.Add(GetToken());
        }
        
        foreach (Token token in Tokens)
            if (token == null)
                return null;

        return Tokens;
    }

    #region Lex Tools
    private bool CanLook()
    {
        return i < N && i >= 0;
    }

    private bool CanLookAhead()
    {
        return i < N - 1;
    }

    private bool CanLookBack()
    {
        return i > 0;
    }

    private void GoForward()
    {
        i++;
    }

    private void GoBack()
    {
        i--;
    }

    private char Look()
    {
        if (CanLook())
            return this.line[i];
        else
            return ' ';
    }

    private char LookAhead()
    {
        if (CanLookAhead())
            return this.line[i + 1];
        else
            return ' ';
    }

    private char LookBack()
    {
        if (CanLookBack())
            return this.line[i - 1];
        else
            return ' ';
    }

    private bool IsToken(string token)
    {
        return Grammar.ValidTokens.ContainsKey(token);
    }

    private void ThrowNewLexError(string error)
    {
        lexError = new LexError(error);
        lexError.Show();
    }

    private bool IsWhiteSpaceOrPunctuationOrSymbol(char c)
    {
        return char.IsWhiteSpace(c) || char.IsPunctuation(c) || char.IsSymbol(c);
    }
    #endregion

    #region Tokens Getters
    private Token GetToken()
    {
        if (char.IsPunctuation(Look()))
            return GetSeparator();
        else if (char.IsSymbol(Look()))
            return GetOperator();
        else if (char.IsDigit(Look()))
            return GetNumber();
        else if (char.IsLetter(Look()))
            return GetKeywordOrIdentifier();
        else
            return null;
    }

    private Token GetSeparator()
    {
        string token = Look().ToString();

        if (IsToken(token) && !IsToken(token + LookAhead()))
            return new Token(Grammar.ValidTokens[token], token);
        else
        {
            lexError = new LexError($"{token} is not a valid token");
            lexError.Show();
            return null;
        }
    }
    
    private Token GetOperator()
    {
        string token = Look().ToString();

        if (IsToken(token) && !IsToken(token + LookAhead()))
            return new Token(TokenType.Operator, token);
        else
        {
            GoForward();
            token += Look();

            if (IsToken(token) && !IsToken(token + LookAhead()))
                return new Token(TokenType.Operator, token);
            else
            {
                ThrowNewLexError($"{token} is not a valid token");
                return null;
            }
        }
    }

    private Token GetNumber()
    {
        string token = "";
        bool point = false;

        for (; CanLook(); GoForward())
        {
            if (char.IsDigit(Look()))
                token += Look();
            else if (Look() == ',' && char.IsDigit(LookAhead()) && !point)
            {
                token += '.';
                point = true;
            }
            else if (Look() == '.' && !point)
            {
                token += Look();
                point = true;
            }
            else if (IsWhiteSpaceOrPunctuationOrSymbol(Look()))
            {
                GoBack();
                return new Token(TokenType.NumericLiteral, token);
            }
            else
            {
                token += Look();
                ThrowNewLexError($"{token} is not a valid token");
                break;
            }
        }
        return null;
    }

    private Token GetKeywordOrIdentifier()
    {
        string token = "";

        for (; CanLook(); GoForward())
        {
            if (IsToken(token) && !IsToken(token + Look()))
            {
                GoBack();
                return new Token(Grammar.ValidTokens[token], token);
            }
            else if (IsWhiteSpaceOrPunctuationOrSymbol(Look()) && !IsToken(token) && token != "")
            {
                GoBack();
                return new Token(TokenType.Identifier, token);
            }
            else
                token += Look();
        }

        ThrowNewLexError($"{token} is not a valid token");
        return null;
    }

    private Token GetStringExpression()
    {
        GoForward();
        string str = "";

        for (; CanLook(); GoForward())
        {
            if (Look() == '\"')
                return new Token(TokenType.StringLiteral, str);
            else
                str += Look();
        }

        ThrowNewLexError("String expression missing closure");
        return null;
    }
    #endregion
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
    Expression,
    NumericLiteral,
    StringLiteral,
    BooleanLiteral
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