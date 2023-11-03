namespace HULK;

public class Lexer
{
    private LexError lex;

    private string line;

    private int i;

    public Lexer()
    {
        lex = new LexError();
        line = "";
    }

    public List<Token>? Tokenize(string line)
    {
        List<Token> tokens = new List<Token>();
        this.line = line;
        i = 0;

        for (; i < line.Length; i++){
            if (char.IsWhiteSpace(line[i])){
                continue;
            }
            else if (line[i] == '\"'){
                tokens.Add(Token.GetToken("\""));
                tokens.Add(GetStringExpression());
                if (line[i] == '\"'){
                    tokens.Add(Token.GetToken("\""));
                }
            }
            else if (char.IsLetterOrDigit(line[i]) || char.IsPunctuation(line[i]) || char.IsSymbol(line[i])){
                tokens.Add(GetToken());
            }
        }
        
        foreach (Token token in tokens){
            if (token == null){
                return null;
            }
        }
        return tokens;
    }

    private Token GetToken()
    {
        if (char.IsPunctuation(line[i]) || char.IsSymbol(line[i]))
            return GetSeparatorOrOperator();
        else if (char.IsDigit(line[i]))
            return GetNumericLiteral();
        else if (char.IsLetter(line[i]))
            return GetKeywordOrIdentifier();
        else
            return null;
    }

    private Token GetSeparatorOrOperator()
    {
        string token = line[i].ToString();

        if (Token.IsToken(token) && !Token.IsToken(token + ((i < line.Length - 1) ? line[i + 1] : ' '))){
            return Token.GetToken(token);
        }
        else{
            token += line[++i];

            if (Token.IsToken(token) && !Token.IsToken(token + ((i < line.Length - 1) ? line[i + 1] : ' '))){
                return Token.GetToken(token);
            }
            else{
                lex.Show($"\'{token}\' is not a valid token");
                return null;
            }
        }
    }

    private Token GetNumericLiteral()
    {
        string token = line[i++].ToString();
        bool point = false;

        for (; i < line.Length && i >= 0; i++){
            if (char.IsDigit(line[i])){
                token += line[i];
            }
            else if (line[i] == ',' && char.IsDigit((i < line.Length - 1) ? line[i + 1] : ' ') && !point){
                token += '.';
                point = true;
            }
            else if (line[i] == '.' && !point){
                token += line[i];
                point = true;
            }
            else if (point && (line[i] == ',' || line[i] == '.')){
                token += '.';
                lex.Show($"\'{token}\' is not a valid token");
                break;
            }
            else if (char.IsWhiteSpace(line[i]) || char.IsPunctuation(line[i]) || char.IsSymbol(line[i])){
                i--;
                return new Token(token, TokenType.NumericLiteral);
            }
            else{
                token += line[i];
                lex.Show($"\'{token}\' is not a valid token");
                break;
            }
        }
        return null;
    }

    private Token GetKeywordOrIdentifier()
    {
        string token = line[i++].ToString();

        while (i < line.Length)
        {
            if (char.IsLetterOrDigit(line[i]))
                token += line[i++];
            else if (char.IsWhiteSpace(line[i]) || char.IsPunctuation(line[i]) || char.IsSymbol(line[i])){
                i--;
                return Token.IsToken(token) ? Token.GetToken(token) : new Token(token, TokenType.Identifier);
            }
        }

        lex.Show($"\'{token}\' is not a valid token");
        return null;
    }

    private Token GetStringExpression()
    {
        i++;
        string str = line[i++].ToString();

        while (i < line.Length && line[i] != '\"'){
            str += line[i++];
        }

        return new Token(str, TokenType.StringLiteral);
    }
}