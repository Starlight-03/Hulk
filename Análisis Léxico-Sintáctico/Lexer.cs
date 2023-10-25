public class Lexer
{
    public LexError Lex { get; private set; }

    public List<Token> Tokens { get; private set;}

    private readonly string line;

    private int i;

    private int N => line.Length;
    
    public Lexer(string line)
    {
        Lex = new LexError();
        Tokens = new List<Token>();
        this.line = line;
        i = 0;

        Tokenize();
    }

    private void Tokenize()
    {
        for (; CanLook(); GoForward())
        {
            if (char.IsWhiteSpace(Look()))
                continue;
            else if (Look() == '\"')
            {
                Tokens.Add(Token.GetToken("\""));
                Tokens.Add(GetStringExpression());
                if (Look() == '\"')
                    Tokens.Add(Token.GetToken("\""));
            }
            else if (char.IsLetterOrDigit(Look()) || char.IsPunctuation(Look()) || char.IsSymbol(Look()))
                Tokens.Add(GetToken());
        }
        
        foreach (Token token in Tokens)
            if (token == null)
                Tokens = null;
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
            return line[i];
        else
            return ' ';
    }

    private char LookAhead()
    {
        if (CanLookAhead())
            return line[i + 1];
        else
            return ' ';
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

        if (Token.IsToken(token) && !Token.IsToken(token + LookAhead()))
            return Token.GetToken(token);
        else
        {
            GoForward();
            token += Look();

            if (Token.IsToken(token) && !Token.IsToken(token + LookAhead()))
                return Token.GetToken(token);
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
                if (Token.IsToken(token))
                {
                    GoBack();
                    return Token.GetToken(token);
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

        for (; CanLook() && Look() != '\"'; GoForward())
            str += Look();

        return new Token(str, TokenType.StringLiteral);
    }
    #endregion
}