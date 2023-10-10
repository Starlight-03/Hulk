public class Lexer
{
    private LexError? lexError;

    private List<Token> Tokens;

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
        return Token.Grammar.ContainsKey(token);
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
            return new Token(TokenType.Separator, token);
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

    private Token GetKeywordOrIdentifier() // ! Necesita unos arreglos
    {
        string token = "";

        for (; CanLook(); GoForward())
        {
            if (IsToken(token) && !IsToken(token + Look()))
            {
                GoBack();
                return new Token(Token.Grammar[token], token);
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