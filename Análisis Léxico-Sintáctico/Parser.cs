public class Parser
{
    // El parser revisa la semántica del código, la evalúa y devuelve un árbol de derivación
    private SyntaxError syntaxError;
    
    private readonly List<Token> tokens;
    
    private int index;
    
    public Parser(List<Token> tokens)
    {
        this.syntaxError = new SyntaxError();
        this.tokens = tokens;
        this.index = 0;
    }

    public Expression Parse()
    {
        if (tokens[tokens.Count - 1] == TokenValues.Grammar[";"])
            return Expression();
        else
            return null;
    }

    #region Parsing Tools
    private bool Match(Token token)
    {
        return tokens[index++] == token;
    }

    private bool MatchNumber()
    {
        return tokens[index++].Type == TokenType.NumericLiteral;
    }

    private bool MatchString()
    {
        return Match(TokenValues.Grammar["\""])
            && tokens[index++].Type == TokenType.StringLiteral
            && Match(TokenValues.Grammar["\""]);
    }

    private bool MatchIdentifier()
    {
        return tokens[index++].Type == TokenType.Identifier;
    }

    private bool Reset(int pos)
    {
        index = pos;
        return true;
    }
    #endregion

    #region Parsing Expressions
    private Expression Expression()
    {
        int pos = index;
        Expression node;
        if (Match(TokenValues.Grammar["print"])){ // Expr -> Print
            node = Print();
            if (node != null) return node;
        }
        if (Reset(pos) && Match(TokenValues.Grammar["function"])){ // Expr -> Func
            node = Function();
            if (node != null) return node;
        }
        if (Reset(pos) && Match(TokenValues.Grammar["let"])){ // Expr -> LetIn
            node = LetIn();
            if (node != null) return node;
        }
        if (Reset(pos) && Match(TokenValues.Grammar["if"])){ // Expr -> IfElse
            node = IfElse();
            if (node != null) return node;
        }
        if (Reset(pos) && Match(TokenValues.Grammar["("])){ // Expr -> (Expr)
            node = Expression();
            if (node != null  && Match(TokenValues.Grammar[")"])) return node;
        }
        // Expr -> Val
        Reset(pos);
        node = Value();
        if (node != null) return node;
        else return null;
    }
    #endregion

    #region Parsing Print
    private Expression Print()
    {
        // Func -> print(Expr)
        if (Match(TokenValues.Grammar["("]))
        {
            Expression expr = Expression();
            if (expr != null  && Match(TokenValues.Grammar[")"]))
                return new Print(expr);
        }
        
        return null;
    }
    #endregion

    #region Parsing Function
    private Expression Function()
    {
        // Func -> function Id(args) => Expr
        string id;
        if (MatchIdentifier())
            id = tokens[index - 1].Value;
        else
            return null;
        
        List<string> args = new List<string>();
        if (Match(TokenValues.Grammar["("])){
            while (tokens[index].Type == TokenType.Identifier || tokens[index] == TokenValues.Grammar[","]){
                if (tokens[index].Type == TokenType.Identifier){
                    args.Add(tokens[index - 1].Value);
                    index++;
                }
                else if (tokens[index] == TokenValues.Grammar[","]){
                    index++;
                }
            }
            if (!Match(TokenValues.Grammar[")"]))
                return null;
        }

        Expression body = Expression();
        if (body != null)
            return new FuncDef(id, args, body);
        else
            return null;
    }
    #endregion

    #region Parsing Let In
    private Expression LetIn()
    {
        // LetIn -> let Var = Val in Expr
        Dictionary<string, Expression> variables = new Dictionary<string, Expression>();
        while (MatchIdentifier())
        {
            string id = tokens[index - 1].Value;
            if (Match(TokenValues.Grammar["="]))
            {
                Expression expr = Value();
                if (expr != null) 
                    variables.Add(id, expr);
                else 
                    return null;
            }
            if (tokens[index] == TokenValues.Grammar[","])
                index++;
        }

        if (Match(TokenValues.Grammar["in"])){
            Expression expr = Expression();
            if (expr != null)
                return new LetIn(variables, expr);
        }
        
        return null;
    }
    #endregion

    #region Parsing If Else
    private Expression IfElse()
    {
        // IfElse -> if (Bool) Expr else Expr
        if (Match(TokenValues.Grammar["("]))
        {
            BooleanLiteral condition = (BooleanLiteral)Bool();
            if (condition != null && Match(TokenValues.Grammar[")"]))
            {
                Expression positive = Expression();
                if (positive != null && Match(TokenValues.Grammar["else"]))
                {
                    Expression negative = Expression();
                    if (negative != null)
                        return new IfElse(condition, positive, negative);
                }
            }
        }

        return null;
    }
    #endregion

    #region Parsing Values
    private Expression Value()
    {
        int pos = index;
        Expression expr ;

        if (Match(TokenValues.Grammar["\""])){ // Val -> Str
            expr = StringExpression();
            if (expr != null) 
                return expr;
        }

        Reset(pos);
        expr = Bool(); // Val -> Bool
        if (expr != null) 
            return expr;

        Reset(pos);
        expr = NumericalExpression(); // Val -> NumExpr
        if (expr != null) 
            return expr;

        return null;
    }

    private Expression FuncCall()
    {
        // Term -> FuncCall Y
        string id = tokens[index - 2].Value;
        List<Expression> args = new List<Expression>();
        
        do
        {
            Expression expr = Value();
            if (expr != null)
                args.Add(expr);
        } while (tokens[index] != TokenValues.Grammar[")"]);

        return new FuncCall(id, args);
    }

    #region Parsing Numerical Expressions
    private Expression NumericalExpression()
    {
        // NumExpr -> Term X
        Expression left = Term();

        Expression expr;

        if (left != null)
        {
            expr = X(left);
            if (expr != null) 
                return expr;
        }

        return null;
    }

    private Expression Term()
    {
        int pos = index;
        Expression term;
        if (MatchNumber()){ // Term -> int Y
            term = Y(new Number(tokens[index].Value));
            if (term != null)
                return term;
        }
        if (Reset(pos) && Match(TokenValues.Grammar["("])){ // Term -> (NumExpr) Y
            term = Y(NumericalExpression());
            if (term != null && Match(TokenValues.Grammar[")"]))
                return term;
        }
        if (Reset(pos) && MatchIdentifier() && Match(TokenValues.Grammar["("])){ // Term -> FuncCall Y
            term = Y(FuncCall());
            if (term != null)
                return term;
        }
        if (Reset(pos) && MatchIdentifier()){ // Term -> Var Y
            term = Y(new Variable(tokens[index - 1].Value));
            if (term != null)
                return term;
        }
        if (Reset(pos) && Match(TokenValues.Grammar["let"])){ // Term -> LetIn Y
            index--;
            term = Y(LetIn());
            if (term != null)
                return term;
        }
        return null;
    }

    private Expression X(Expression left)
    {
        int pos = index;
        Expression right;

        if (Match(TokenValues.Grammar["+"])){ // X -> + NumExpr
            right = NumericalExpression();
            if (right != null)
                return new BinaryExpression(left, Operator.Sum, Term());
        }
        if (Reset(pos) && Match(TokenValues.Grammar["-"])){ // X -> - NumExpr
            right = NumericalExpression();
            if (right != null)
                return new BinaryExpression(left, Operator.Sub, Term());
        }

        Reset(pos);
        return left; // X -> e
    }

    private Expression Y(Expression left)
    {
        if (left == null) 
            return null;

        int pos = index;
        Expression right;

        if (Match(TokenValues.Grammar["*"])){ // Y -> * Term
            right = Term();
            if (right != null)
                return new BinaryExpression(left, Operator.Mul, Term());
        }
        if (Reset(pos) && Match(TokenValues.Grammar["/"])){ // Y -> / Term
            right = Term();
            if (right != null)
                return new BinaryExpression(left, Operator.Div, Term());
        }
        if (Reset(pos) && Match(TokenValues.Grammar["%"])){ // Y -> % Term
            right = Term();
            if (right != null)
                return new BinaryExpression(left, Operator.Mod, Term());
        }
        if (Reset(pos) && Match(TokenValues.Grammar["^"])){ // Y -> ^ Term
            right = Term();
            if (right != null)
                return new BinaryExpression(left, Operator.Pow, Term());
        }

        Reset(pos);
        return left; // Y -> e
    }
    #endregion

    #region Parsing Boolean Expressions
    private Expression Bool()
    {
        int pos = index;
        if (Match(TokenValues.Grammar["true"])){ // Bool -> true OpBool
            return OpBool(new BooleanLiteral("true"));
        }
        if (Reset(pos) && Match(TokenValues.Grammar["false"])){ // Bool -> false OpBool
            return OpBool(new BooleanLiteral("false"));
        }
        if (Reset(pos) && Match(TokenValues.Grammar["("])){ // Bool -> (Bool) OpBool
            Expression boolean = Bool();
            if (boolean != null && Match(TokenValues.Grammar[")"]))
                return OpBool(boolean);
        }
        if (Reset(pos) && MatchIdentifier() && Match(TokenValues.Grammar["("])){ // Bool -> FuncCall OpBool
            return OpBool(FuncCall());
        }
        if (Reset(pos) && MatchIdentifier()){ // Bool -> Var OpBool
            return OpBool(new Variable(tokens[index - 1].Value));
        }
        if (Reset(pos) && Match(TokenValues.Grammar["!"])){ // Bool -> !Bool OpBool
            return OpBool(new NoBool(Bool()));
        }
        Reset(pos);
        Expression compar = OpComp(NumericalExpression()); // Bool -> NumExpr OpComp OpBool
        if (compar != null)
            return OpBool(compar);
        
        return null;
    }

    private Expression OpComp(Expression left)
    {
        if (left == null)
            return null;

        int pos = index;

        if (Match(TokenValues.Grammar["<"])){ // OpComp -> < NumExpr
            return new BinaryExpression(left, Operator.Minor, NumericalExpression());
        }
        if (Reset(pos) && Match(TokenValues.Grammar["<="])){ // OpComp -> <= NumExpr
            return new BinaryExpression(left, Operator.MinorEqual, NumericalExpression());
        }
        if (Reset(pos) && Match(TokenValues.Grammar[">"])){ // OpComp -> > NumExpr
            return new BinaryExpression(left, Operator.Major, NumericalExpression());
        }
        if (Reset(pos) && Match(TokenValues.Grammar[">="])){ // OpComp -> >= NumExpr
            return new BinaryExpression(left, Operator.MajorEqual, NumericalExpression());
        }
        if (Reset(pos) && Match(TokenValues.Grammar["=="])){ // OpComp -> == NumExpr
            return new BinaryExpression(left, Operator.Equals, NumericalExpression());
        }
        if (Reset(pos) && Match(TokenValues.Grammar["!="])){ // OpComp -> != NumExpr
            return new BinaryExpression(left, Operator.NotEqual, NumericalExpression());
        }
        return null;
    }

    private Expression OpBool(Expression left)
    {
        if (left == null)
            return null;

        int pos = index;
        if (Match(TokenValues.Grammar["&"])){ // OpBool -> & Bool
            return new BinaryExpression(left, Operator.And, Bool());
        }
        if (Reset(pos) && Match(TokenValues.Grammar["|"])){ // OpBool -> | Bool
            return new BinaryExpression(left, Operator.And, Bool());
        }

        Reset(pos);
        return left; // OpBool -> e
    }
    #endregion

    #region Parsing String Expressions
    private Expression StringExpression()
    {
        if (MatchString()){ // StrExpr -> String Conc
            return Concatenation(new StringLiteral(tokens[index - 2].Value));
        }

        return null;
    }

    private Expression Concatenation(Expression left)
    {
        if (left == null)
            return null;
        
        int pos = index;
        if (Match(TokenValues.Grammar["@"])){ // Conc -> @ Val
            return new BinaryExpression(left, Operator.Concat, Value());
        }
        Reset(pos);
        return left; // Conc -> e
    }
    #endregion

    #endregion
}