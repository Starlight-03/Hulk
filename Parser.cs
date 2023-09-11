public class Parser
{
    private SyntaxError syntaxError;
    private int index;
    public Parser()
    {
        
    }

    public void Parse(List<Token> tokens)
    {
        index = 0;
        string expression = "";
        if (tokens[index++].Value != "print")
        {
            syntaxError = new SyntaxError("Missing expression print");
            syntaxError.Show();
            return;
        }

        if (tokens[index++].Value != "(")
        {
            syntaxError = new SyntaxError("Missing opening parenthesis after keyword print");
            syntaxError.Show();
            return;
        }

        if (tokens[index].Type != TokenType.Expression)
        {
            syntaxError = new SyntaxError("Missing literal after print expression");
            syntaxError.Show();
            return;
        }
        else
        {
            expression = tokens[index++].Value;
        }

        if (tokens[index++].Value != ")")
        {
            syntaxError = new SyntaxError($"Missing closing parenthesis after {expression} expression");
            syntaxError.Show();
            return;
        }

        if (tokens[index++].Value != ";")
        {
            syntaxError = new SyntaxError("Missing semicolon after the end of line");
            syntaxError.Show();
            return;
        }

        System.Console.WriteLine(expression);
    }

    private void EvaluateFunction(Token[] tokens)
    {
        if (tokens[index].Type == TokenType.Keyword) ;

    }
}