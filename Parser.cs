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
        return Expression() && Match(TokenValues.Grammar[";"]);
    }

    #region Parsing Tools
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
    #endregion

    private bool Expression()
    {
        if (this.tokens[index].Type == TokenType.NumericLiteral)
            return NumericalExpression();
        else if (this.tokens[index].Type == TokenType.Keyword)
            return Function();
        else 
            return false;
    }

    private bool Function()
    {
        throw new NotImplementedException();
    }

    #region Parsing Numerical Expressions
    private bool NumericalExpression()
    {
        // E -> T (+|-) (*|/)
        return Term() && SumSub() && MulDiv();
    }

    private bool Term()
    {
        // T -> int (*|/) | (E) (*|/)
        int pos = index;
        return Term1() 
            || Reset(pos) && Term2();
    }

    private bool Term1()
    {
        // T -> int (*|/)
        return MatchNumber() && MulDiv();
    }

    private bool Term2()
    {
        // T -> (E) (*|7)
        return Match(TokenValues.Grammar["("]) 
            && NumericalExpression() 
            && Match(TokenValues.Grammar[")"]) && MulDiv();
    }

    private bool SumSub()
    {
        // (+|-) -> + E | - E | e
        int pos = index;
        return Sum() 
            || Reset(pos) && Sub() 
            || Reset(pos) && true;
    }

    private bool Sum()
    {
        // + E
        return Match(TokenValues.Grammar["+"]) && NumericalExpression();
    }

    private bool Sub()
    {
        // - E
        return Match(TokenValues.Grammar["-"]) && NumericalExpression();
    }

    private bool MulDiv()
    {
        // (*|/) -> * T | / T | e
        int pos = index;
        return Mul() 
            || Reset(pos) && Div() 
            || Reset(pos) && true;
    }

    private bool Mul()
    {
        // * T
        return Match(TokenValues.Grammar["*"]) && Term();
    }

    private bool Div()
    {
        // / T
        return Match(TokenValues.Grammar["/"]) && Term();
    }
    #endregion
}