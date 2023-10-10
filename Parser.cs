using System.Runtime.InteropServices;

public class Parser
{
    private SyntaxError syntaxError;
    
    private readonly List<Token> tokens;
    
    private int index;
    
    public Parser(List<Token> tokens)
    {
        this.tokens = tokens;
        this.index = 0;
    }

    public bool Parse()
    {
        throw new NotImplementedException();
    }

    private bool Match(Token token)
    {
        return this.tokens[index++] == token;
    }

    private void Reset(int pos)
    {
        index = pos;
    }

    private bool E()
    {
        // Parsea un no-terminal E
        int pos = index;
        if (E1()) 
            return true;
        
        Reset(pos);
        if (E2())
            return true;

        return false;
    }

    private bool E1()
    {
        // E -> T
        return T();
    }

    private bool E2()
    {
        // E -> T + E
        return T() && Match(TokenValues.Grammar["+"]) && E();
    }

    private bool T()
    {
        // Parsea un no-terminal T
        throw new NotImplementedException();
    }

    // La semántica de cada uno de estos métodos es que devuelven true si y solo si 
    // el no-terminal correspondiente genera una parte de la cadena, comenzando en 
    // la posición nextToken.
}