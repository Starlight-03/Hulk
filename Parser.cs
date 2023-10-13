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

    private bool MatchString()
    {
        return this.tokens[index++].Type == TokenType.StringLiteral;
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
        //       | (Expr)
        int pos = index;
        return Print()
            || Reset(pos) && Function() 
            || Reset(pos) && LetIn() 
            || Reset(pos) && IfElse() 
            || Reset(pos) && Value()
            || Reset(pos) && Match(TokenValues.Grammar["("]) && Expression() && Match(TokenValues.Grammar[")"]);
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
    #endregion

    #region Parsing Identifier
    private bool Identifier()
    {
        // Ident -> Id Param (+|-) (*|/) Conc
        return MatchIdentifier() 
            && Parameter() 
            && SumSub() 
            && MulDivMod() 
            && Concatenation();
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
        // LetIn -> let Var in Expr

        return Match(TokenValues.Grammar["let"]) 
            && Variable() 
            && Match(TokenValues.Grammar["in"]) 
            && Expression();
    }

    private bool Variable()
    {
        // Var -> Ident = Val Var1
        return Identifier() && Match(TokenValues.Grammar["="]) && Value() && Variable1();
    }

    private bool Variable1()
    {
        // Var1 -> , Var | e
        int pos = index;
        return Match(TokenValues.Grammar[","]) && Variable() 
            || Reset(pos) && true;
    }
    #endregion

    #region Parsing If Else
    private bool IfElse()
    {
        // IfElse -> if (Bool) Expr else Expr
        return Match(TokenValues.Grammar["if"]) 
            && Match(TokenValues.Grammar["("]) 
            && Bool() && Match(TokenValues.Grammar[")"]) 
            && Expression() 
            && Match(TokenValues.Grammar["else"]) 
            && Expression();
    }
    #endregion

    #region Parsing Values
    private bool Value()
    {
        // Val -> NumExpr
        //      | Bool
        //      | Str
        int pos = index;
        return StringExpression() 
            || Reset(pos) && Bool()
            || Reset(pos) && Identifier()
            || Reset(pos) && NumericalExpression();
    }

    #region Parsing Numerical Expressions
    private bool NumericalExpression()
    {
        // NumExpr -> Term (+|-) (*|/|%)
        return Term() && SumSub() && MulDivMod();
    }

    private bool Term()
    {
        // Term -> int (*|/) | (NumExpr) (*|/|%) | Ident (*|/|%) | LetIn (*|/|%)
        int pos = index;
        return Term1() 
            || Reset(pos) && Term2()
            || Reset(pos) && Term3()
            || Reset(pos) && Term4();
    }

    private bool Term1()
    {
        // Term -> int (*|/|%)
        return MatchNumber() && MulDivMod();
    }

    private bool Term2()
    {
        // Term -> (NumExpr) (*|/|%)
        return Match(TokenValues.Grammar["("]) 
            && NumericalExpression() 
            && Match(TokenValues.Grammar[")"]) 
            && MulDivMod();
    }

    private bool Term3()
    {
        // Term -> Ident (*|/|%)
        return Identifier() && MulDivMod();
    }

    private bool Term4()
    {
        // Term -> LetIn (*|/|%)
        return LetIn() && MulDivMod();
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
        int pos = index;
        return Match(TokenValues.Grammar["+"]) && NumericalExpression();
    }

    private bool Sub()
    {
        // - NumExpr
        int pos = index;
        return Match(TokenValues.Grammar["-"]) && NumericalExpression();
    }

    private bool MulDivMod()
    {
        // (*|/) -> * Term | / Term | % Term | e
        int pos = index;
        return Mul() 
            || Reset(pos) && Div() 
            || Reset(pos) && Mod() 
            || Reset(pos) && true;
    }

    private bool Mul()
    {
        // * Term
        int pos = index;
        return Match(TokenValues.Grammar["*"]) && Term();
    }

    private bool Div()
    {
        // / Term
        int pos = index;
        return Match(TokenValues.Grammar["/"]) && Term();
    }

    private bool Mod()
    {
        // % Term
        int pos = index;
        return Match(TokenValues.Grammar["%"]) && Term();
    }
    #endregion

    #region Parsing Boolean Expressions
    private bool Bool()
    {
        // Bool -> litBool
        //       | Ident
        //       | Compar
        //       | !Bool
        //       | Bool OpBool
        int pos = index;
        return Bool1() 
            || Reset(pos) && Bool2() 
            || Reset(pos) && Bool3() 
            || Reset(pos) && Bool4() 
            || Reset(pos) && Bool5();
    }

    private bool Bool1()
    {
        return LitBool();
    }

    private bool Bool2()
    {
        return Match(TokenValues.Grammar["("]) && Bool() && Match(TokenValues.Grammar[")"]) && OpBool();
    }

    private bool Bool3()
    {
        return Comparation() && OpBool();
    }

    private bool Bool4()
    {
        return Identifier() && OpBool();
    }

    private bool Bool5()
    {
        return Match(TokenValues.Grammar["!"]) && Bool();
    }

    private bool Comparation()
    {
        // Compar -> NumExpr OpComp
        return NumericalExpression() && OpComp();
    }

    private bool OpComp()
    {
        // OpComp -> < NumExpr | <= NumExpr | > NumExpr | >= NumExpr | == NumExpr | != NumExpr
        int pos = index;
        return OpComp1()
            || Reset(pos) && OpComp2()
            || Reset(pos) && OpComp3()
            || Reset(pos) && OpComp4()
            || Reset(pos) && OpComp5()
            || Reset(pos) && OpComp6();
    }

    private bool OpComp1()
    {
        // OpComp -> < NumExpr
        return Match(TokenValues.Grammar["<"]) && NumericalExpression();
    }

    private bool OpComp2()
    {
        // OpComp -> <= NumExpr
        return Match(TokenValues.Grammar["<="]) && NumericalExpression();
    }

    private bool OpComp3()
    {
        // OpComp -> > NumExpr
        return Match(TokenValues.Grammar[">"]) && NumericalExpression();
    }

    private bool OpComp4()
    {
        // OpComp -> >= NumExpr
        return Match(TokenValues.Grammar[">="]) && NumericalExpression();
    }

    private bool OpComp5()
    {
        // OpComp -> == NumExpr
        return Match(TokenValues.Grammar["=="]) && NumericalExpression();
    }

    private bool OpComp6()
    {
        // OpComp -> != NumExpr
        return Match(TokenValues.Grammar["!="]) && NumericalExpression();
    }

    private bool OpBool()
    {
        // OpBool -> &Bool | |Bool | e
        int pos = index;
        return Match(TokenValues.Grammar["&"]) && Bool()
            || Reset(pos) && Match(TokenValues.Grammar["|"]) && Bool()
            || Reset(pos) && true;
    }

    private bool LitBool()
    {
        // LitBool -> true | false
        int pos = index;
        return Match(TokenValues.Grammar["true"])
            || Reset(pos) && Match(TokenValues.Grammar["false"]);
    }
    #endregion

    #region Parsing String Expressions
    private bool StringExpression()
    {
        // StrExpr -> String Conc
        return MatchString() && Concatenation();
    }

    private bool Concatenation()
    {
        // Conc -> @ Val
        //       | e
        int pos = index;
        return Match(TokenValues.Grammar["@"]) && Value() 
            || Reset(pos) && true;
    }
    #endregion

    #endregion
}