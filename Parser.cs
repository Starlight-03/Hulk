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
        return E() && Match(GetToken(";"));
    }

    private bool Match(Token token)
    {
        return this.tokens[index++] == token;
    }

    private bool MatchNumber()
    {
        return this.tokens[index++].Type == TokenType.NumericLiteral;
    }

    private bool Reset(int pos)
    {
        index = pos;
        return true;
    }

    private Token GetToken(string value)
    {
        return TokenValues.Grammar[value];
    }

    private bool E()
    {
        // E -> T X Y
        return T() && X() && Y();
    }

    private bool T()
    {
        // T -> int Y | (E) Y
        int pos = index;
        return T1() || Reset(pos) && T2();
    }

    private bool T1()
    {
        // T -> int Y 
        return MatchNumber() && Y();
    }

    private bool T2()
    {
        // T -> (E)
        return Match(GetToken("(")) && E() && Match(GetToken(")")) && Y();
    }

    private bool X()
    {
        // X -> + E | - E | e
        int pos = index;
        return X1() || Reset(pos) && X2() || Reset(pos) && true;
    }

    private bool X1()
    {
        // X -> + E
        return Match(GetToken("+")) && E();
    }

    private bool X2()
    {
        // X -> - E
        return Match(GetToken("-")) && E();
    }

    private bool Y()
    {
        // Y -> * T | / T | e
        int pos = index;
        return Y1() || Reset(pos) && Y2() || Reset(pos) && true;
    }

    private bool Y1()
    {
        // Y -> * T
        return Match(GetToken("*")) && T();
    }

    private bool Y2()
    {
        // Y -> / T
        return Match(GetToken("/")) && T();
    }
}