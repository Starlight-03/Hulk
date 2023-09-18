using System.Runtime.InteropServices;

public class Parser
{
    private SyntaxError syntaxError;
    private List<Token> Tokens;
    private int index;
    public Parser()
    {
        
    }

    // public void Parse(List<Token> tokens)
    // {
        // index = 0;
        // string expression = "";
        // if (tokens[index++].Value != "print")
        // {
        //     syntaxError = new SyntaxError("Missing expression print");
        //     syntaxError.Show();
        //     return;
        // }

        // if (tokens[index++].Value != "(")
        // {
        //     syntaxError = new SyntaxError("Missing opening parenthesis after keyword print");
        //     syntaxError.Show();
        //     return;
        // }

        // if (tokens[index].Type != TokenType.Expression)
        // {
        //     syntaxError = new SyntaxError("Missing literal after print expression");
        //     syntaxError.Show();
        //     return;
        // }
        // else
        // {
        //     expression = tokens[index++].Value;
        // }

        // if (tokens[index++].Value != ")")
        // {
        //     syntaxError = new SyntaxError($"Missing closing parenthesis after {expression} expression");
        //     syntaxError.Show();
        //     return;
        // }

        // if (tokens[index++].Value != ";")
        // {
        //     syntaxError = new SyntaxError("Missing semicolon after the end of line");
        //     syntaxError.Show();
        //     return;
        // }

        // System.Console.WriteLine(expression);
    // }

    // private void ParsePrint(Token[] tokens)
    // {
        // // print ( " expression " ) ;
        // if (tokens[index].Type == TokenType.Keyword) ;

    // }

    // private Expression ParseExpression()
    // {
    //     if (this.Tokens[index] != )
    // }

    private Expression Parse()
    {
        if (this.Tokens[index].Type == TokenType.Keyword)
        {
            if (this.Tokens[index].Value == "print")
            {
                index++;
                return ParsePrint();
            }
            if (this.Tokens[index].Value == "function")
            {
                index++;
                return ParseFunction();
            }
            if (this.Tokens[index].Value == "let")
            {
                index++;
                return ParseLetIn();
            }
            if (this.Tokens[index].Value == "if")
            {
                index++;
                return ParseIfElse();
            }

            return null;
        }

        else if (this.Tokens[index].Type == TokenType.Expression)
        {
            return ParseExpression();
        }

        else
        {
            this.syntaxError = new SyntaxError("Missing instruction");
            return null;
        }


    }

    private Expression ParsePrint()
    {
        // print ( " expression " ) ;

        string expression = "";

        if (this.Tokens[index++].Value != "(")
            this.syntaxError = new SyntaxError("Missing ( after print instruction");
        
        if (this.Tokens[index].Type != TokenType.Expression)
        {
            if (this.Tokens[index].Type == TokenType.Keyword)
            {
                index++;
                expression = Parse().ToString();
            }
            else
                this.syntaxError = new SyntaxError("Missing expression to print after print instruction");
        }
        else
            index++;

        if (this.Tokens[index++].Value != ")")
            this.syntaxError = new SyntaxError("Missing ) after print expression");

        if (this.Tokens[index++].Value != ";")
            this.syntaxError = new SyntaxError("Missing ; after print expression");

        System.Console.WriteLine(expression);
        return null;
    }

    private Expression ParseFunction()
    {
        return null;
    }

    private Expression ParseLetIn()
    {
        return null;
    }

    private Expression ParseIfElse()
    {
        return null;
    }

    private Expression ParseExpression()
    {
        return null;
    }
}