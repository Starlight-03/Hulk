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

    private bool MatchIdentifier()
    {
        return this.tokens[index++].Type == TokenType.Identifier;
    }

    private bool Reset(int pos)
    {
        index = pos;
        return true;
    }
    #endregion

    private bool Expression()
    {
        // Expr -> Print
        //       | Func
        //       | LetIn
        //       | IfElse
        //       | Ident
        //       | Val
        int pos = index;
        return Print()
            || Reset(pos) && Function() 
            // || Reset(pos) && LetIn() 
            // || Reset(pos) && IfElse() 
            || Reset(pos) && Identifier() 
            || Reset(pos) && Value();
    }

    #region Parsing Print
    private bool Print()
    {
        // Func -> print(Expr)
        return Match(TokenValues.Grammar["print"]) 
            && Match(TokenValues.Grammar["("]) 
            && Expression() 
            && Match(TokenValues.Grammar[")"]);
    }
    #endregion

    #region Parsing Function
        private bool Function()
    {
        // Func -> function Ident => Expr
        return Match(TokenValues.Grammar["function"]) 
            && Identifier() 
            && Match(TokenValues.Grammar["=>"]) 
            && Expression();
    }

    private bool Identifier()
    {
        // Ident -> Id Param (+|-) (*|/)
        return MatchIdentifier() && Parameter() && SumSub() && MulDiv();
    }

    private bool Parameter()
    {
        // Param -> (Param1) | e
        int pos = index;
        return Match(TokenValues.Grammar["("]) 
            && Parameter1() 
            && Match(TokenValues.Grammar[")"]) 
            || Reset(pos) &&  true;
    }

    private bool Parameter1()
    {
        // Param1 -> Ident Param2 | int Param2 | e
        int pos = index;
        return Identifier() && Parameter2() 
            || Reset(pos) && MatchNumber() && Parameter2() 
            || Reset(pos) && true;
    }

    private bool Parameter2()
    {
        // Param2 -> , Param1 | e
        int pos = index;
        return Match(TokenValues.Grammar[","]) && Parameter1() 
            || Reset(pos) && true;
    }
    #endregion

    #region Parsing Let In
    private bool LetIn()
    {
        throw new NotImplementedException();
    }
    #endregion

    #region Parsing If Else
    private bool IfElse()
    {
        throw new NotImplementedException();
    }
    #endregion

    #region Parsing Values
    private bool Value()
    {
        // Val -> NumExpr
        //      | Bool
        //      | Str
        int pos = index;
        return NumericalExpression()
            // || Reset(pos) && Bool()
            // || Reset(pos) && StringExpression()
            ;
    }

    #region Parsing Numerical Expressions
    private bool NumericalExpression()
    {
        // NumExpr -> Term (+|-) (*|/)
        return Term() && SumSub() && MulDiv();
    }

    private bool Term()
    {
        // Term -> int (*|/) | (NumExpr) (*|/)
        int pos = index;
        return Term1() 
            || Reset(pos) && Term2();
    }

    private bool Term1()
    {
        // Term -> int (*|/)
        return MatchNumber() && MulDiv();
    }

    private bool Term2()
    {
        // Term -> (NumExpr) (*|7)
        return Match(TokenValues.Grammar["("]) 
            && NumericalExpression() 
            && Match(TokenValues.Grammar[")"]) && MulDiv();
    }

    private bool SumSub()
    {
        // (+|-) -> + NumExpr | - NumExpr | e
        int pos = index;
        return Sum() 
            || Reset(pos) && Sub() 
            || Reset(pos) && true;
    }

    private bool Sum()
    {
        // + NumExpr
        return Match(TokenValues.Grammar["+"]) && NumericalExpression();
    }

    private bool Sub()
    {
        // - NumExpr
        return Match(TokenValues.Grammar["-"]) && NumericalExpression();
    }

    private bool MulDiv()
    {
        // (*|/) -> * Term | / Term | e
        int pos = index;
        return Mul() 
            || Reset(pos) && Div() 
            || Reset(pos) && true;
    }

    private bool Mul()
    {
        // * Term
        return Match(TokenValues.Grammar["*"]) && Term();
    }

    private bool Div()
    {
        // / Term
        return Match(TokenValues.Grammar["/"]) && Term();
    }
    #endregion

    #region Parsing Boolean Expressions
    private bool Bool()
    {
        throw new NotImplementedException();
    }
    #endregion

    #region Parsing String Expressions
    private bool StringExpression()
    {
        throw new NotImplementedException();
    }
    #endregion

    #endregion
}