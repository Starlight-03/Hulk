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
            List<string> tokens = Read(line);
            foreach (string token in tokens) Console.WriteLine(token);
        }
        else 
        {
            lexError = new LexError("Missing ';'");
            lexError.Show();
        }
    }

    private List<string> Read(string line)
    {
        List<string> tokens = new List<string>();

        string token = "";

        for (int i = 0; i < line.Length; i++)
        {
            if (token == "\"")
            {
                string subLine = token + line[i++];
                token = "";
                for (; i < line.Length && !subLine.EndsWith('\"'); i++)
                    subLine += line[i];
                i--;
            }
            else if (IsToken(token))
            {
                tokens.Add(token);
                token = line[i].ToString();
            }
            else
                token += line[i];
        }

        tokens.Add(token);
        
        return tokens;
    }

    private bool IsToken(string s)
    {
        foreach (string token in Token.Tokens)
            if (s == token)
                return true;
        
        return false;
    }
}

public class Token
{
    public static string[] Tokens = new string[]
    {
        "(",
        ")",
        ";",
        "print",
    };
}