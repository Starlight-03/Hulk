public class Parser
{
    private SyntaxError syntaxError;
    public Parser()
    {
        
    }

    public void Parse(List<Token> tokens)
    {
        int i = 0;
        string expression = "";
        if (tokens[i++].Value != "print")
        {
            syntaxError = new SyntaxError("Missing expression print");
            syntaxError.Show();
            return;
        }

        if (tokens[i++].Value != "(")
        {
            syntaxError = new SyntaxError("Missing opening parenthesis after keyword print");
            syntaxError.Show();
            return;
        }

        if (tokens[i++].Key != Token.TokenType.Literal)
        {
            syntaxError = new SyntaxError("Missing literal after print expression");
            syntaxError.Show();
            return;
        }
        else
        {
            expression = tokens[i-1].Value;
        }

        if (tokens[i++].Value != ")")
        {
            syntaxError = new SyntaxError($"Missing closing parenthesis after {expression} expression");
            syntaxError.Show();
            return;
        }

        if (tokens[i++].Value != ";")
        {
            syntaxError = new SyntaxError("Missing semicolon after the end of line");
            syntaxError.Show();
            return;
        }

        System.Console.WriteLine(expression);
    }
}