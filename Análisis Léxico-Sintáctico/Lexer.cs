public class Lexer
{
    public LexError Lex;

    private List<Token> tokens;

    private readonly string line;

    private int i;

    private int N => this.line.Length;
    
    public Lexer(string line)
    {
        Lex = new LexError();
        tokens = new List<Token>();
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
            {
                this.tokens.Add(TokenValues.Grammar["\""]);
                this.tokens.Add(GetStringExpression());
                this.tokens.Add(TokenValues.Grammar["\""]);
            }
            else if (char.IsLetterOrDigit(Look()) || char.IsPunctuation(Look()) || char.IsSymbol(Look()))
                this.tokens.Add(GetToken());
        }
        
        foreach (Token token in tokens)
            if (token == null)
                return null;

        return tokens;
    }

    #region Lex Tools
    private void GoForward()
    {
        i++;
    }

    private void GoBack()
    {
        i--;
    }

    private bool CanLook()
    {
        return i < N && i >= 0;
    }

    private bool CanLookAhead()
    {
        return i < N - 1;
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

    private bool IsToken(string token)
    {
        return TokenValues.Grammar.ContainsKey(token);
    }

    private bool IsWhiteSpaceOrPunctuationOrSymbol(char c)
    {
        return char.IsWhiteSpace(c) || char.IsPunctuation(c) || char.IsSymbol(c);
    }
    #endregion

    #region Getting Tokens
    private Token GetToken()
    {
        if (char.IsPunctuation(Look()) || char.IsSymbol(Look()))
            return GetSeparatorOrOperator();
        else if (char.IsDigit(Look()))
            return GetNumber();
        else if (char.IsLetter(Look()))
            return GetKeywordOrIdentifier();
        else
            return null;
    }

    private Token GetSeparatorOrOperator()
    {
        string token = Look().ToString();

        if (IsToken(token) && !IsToken(token + LookAhead()))
            return TokenValues.Grammar[token];
        else
        {
            GoForward();
            token += Look();

            if (IsToken(token) && !IsToken(token + LookAhead()))
                return TokenValues.Grammar[token];
            else
            {
                Lex.Info = $"\'{token}\' is not a valid token";
                return null;
            }
        }
    }

    private Token GetNumber()
    {
        string token = "";
        bool point = false;

        for (; CanLook(); GoForward()){
            if (char.IsDigit(Look()))
                token += Look();
            else if (Look() == ',' && char.IsDigit(LookAhead()) && !point){
                token += '.';
                point = true;
            }
            else if (Look() == '.' && !point){
                token += Look();
                point = true;
            }
            else if (point && (Look() == ',' || Look() == '.')){
                token += Look();
                Lex.Info = $"\'{token}\' is not a valid token";
                break;
            }
            else if (IsWhiteSpaceOrPunctuationOrSymbol(Look())){
                GoBack();
                return new Token(token, TokenType.NumericLiteral);
            }
            else{
                token += Look();
                Lex.Info = $"\'{token}\' is not a valid token";
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
            if (char.IsLetterOrDigit(Look()))
                token += Look();
            else if (IsWhiteSpaceOrPunctuationOrSymbol(Look()))
            {
                if (IsToken(token))
                {
                    GoBack();
                    return TokenValues.Grammar[token];
                }
                else
                {
                    GoBack();
                    return new Token(token, TokenType.Identifier);
                }
            }
        }

        Lex.Info = $"\'{token}\' is not a valid token";
        return null;
    }

    private Token GetStringExpression()
    {
        GoForward();
        string str = "";

        for (; CanLook(); GoForward())
        {
            if (Look() == '\"')
                return new Token(str, TokenType.StringLiteral);
            else
                str += Look();
        }

        Lex.Info = "String expression missing closure";
        return null;
    }
    #endregion
}