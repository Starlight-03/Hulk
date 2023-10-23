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
        Expression expression = Expression();
        if (expression != null && Match(TokenValues.Grammar[";"]) && index == tokens.Count)
            return expression;
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
        Expression expr;
        if (Match(TokenValues.Grammar["print"])){ // Expr -> Print
            expr = Print();
            if (expr != null) return expr;
        }
        if (Reset(pos) && Match(TokenValues.Grammar["function"])){ // Expr -> Func
            expr = Function();
            if (expr != null) return expr;
        }
        if (Reset(pos) && Match(TokenValues.Grammar["let"])){ // Expr -> LetIn
            expr = LetIn();
            if (expr != null) return expr;
        }
        if (Reset(pos) && Match(TokenValues.Grammar["if"])){ // Expr -> IfElse
            expr = IfElse();
            if (expr != null) return expr;
        }
        Reset(pos);
        expr = Value(); // Expr -> Val
        if (expr != null) 
            return expr;
        if (Reset(pos) && Match(TokenValues.Grammar["("])){ // Expr -> (Expr)
            expr = Expression();
            if (expr != null  && Match(TokenValues.Grammar[")"])) return expr;
        }
        return null;
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
            do{
                if (tokens[index].Type == TokenType.Identifier)
                    args.Add(tokens[index++].Value);
                else if (tokens[index] == TokenValues.Grammar[","])
                    index++;
                else 
                    return null;
            } while (tokens[index] != TokenValues.Grammar[")"]);
            index++;
        }

        if (!Match(TokenValues.Grammar["=>"]))
            return null;
            
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
        while (tokens[index].Type == TokenType.Identifier)
        {
            string id = tokens[index++].Value;
            
            if (!Match(TokenValues.Grammar["="]))
                return null;
            
            Expression expr = Value();
            if (expr != null) 
                variables.Add(id, expr);
            else 
                return null;
            
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
            Expression condition = Bool();
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
        Expression expr;

        if (Match(TokenValues.Grammar["\""]) && Reset(pos)){ // Val -> " StrExpr "
            expr = StringExpression();
            if (expr != null)
                return expr;
        }
        if (Reset(pos) && MatchNumber() && Reset(pos) 
            || Match(TokenValues.Grammar["("]) && Reset(pos)){ // Val -> Bool: NumExpr Compar NumExpr | NumExpr
            for (int i = index; i < tokens.Count; i++){
                if (tokens[i] == TokenValues.Grammar["<"]
                || Reset(pos) && tokens[i] == TokenValues.Grammar["<="]
                || Reset(pos) && tokens[i] == TokenValues.Grammar[">"]
                || Reset(pos) && tokens[i] == TokenValues.Grammar[">="]
                || Reset(pos) && tokens[i] == TokenValues.Grammar["=="]
                || Reset(pos) && tokens[i] == TokenValues.Grammar["!="]){
                    expr = OpBool(OpComp(NumericalExpression()));
                    if (expr != null)
                        return expr;
                }
            }
            Reset(pos);
            expr = NumericalExpression();
            if (expr != null)
                return expr;
        }
        if (Reset(pos) && MatchIdentifier()){ // Val -> FuncCall X Y Concat | Var X Y Concat
            if (Match(TokenValues.Grammar["("])){
                expr = Concatenation(Y(X(FuncCall())));
                if (expr != null)
                    return expr;
            }
            Reset(pos);
            expr = Concatenation(Y(X(new Variable(tokens[index++].Value))));
            if (expr != null)
                return expr;
        }
        Reset(pos);
        expr = Bool();
        if (expr != null)
            return expr;
        Reset(pos);
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
            if (tokens[index] == TokenValues.Grammar[","])
                index++;
        } while (tokens[index] != TokenValues.Grammar[")"]);
        index++;

        return new FuncCall(id, args);
    }

    #region Parsing Numerical Expressions
    private Expression NumericalExpression()
    {
        // NumExpr -> Term X Y
        return Y(X(Term()));
    }

    private Expression Term()
    {
        int pos = index;
        Expression term;
        if (MatchNumber()){ // Term -> int Y
            term = Y(new Number(tokens[index - 1].Value));
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
            term = Y(LetIn());
            if (term != null)
                return term;
        }
        Reset(pos);
        return null;
    }

    private Expression X(Expression left)
    {
        int pos = index;
        Expression right;

        if (Match(TokenValues.Grammar["+"])){ // X -> + NumExpr
            right = NumericalExpression();
            if (right != null)
                return new BinaryExpression(left, Operator.Sum, right);
        }
        if (Reset(pos) && Match(TokenValues.Grammar["-"])){ // X -> - NumExpr
            right = NumericalExpression();
            if (right != null)
                return new BinaryExpression(left, Operator.Sub, right);
        }

        Reset(pos);
        return left; // X -> e
    }

    private Expression Y(Expression left)
    {
        int pos = index;
        Expression right;

        if (Match(TokenValues.Grammar["*"])){ // Y -> * Term
            right = Term();
            if (right != null)
                return new BinaryExpression(left, Operator.Mul, right);
        }
        if (Reset(pos) && Match(TokenValues.Grammar["/"])){ // Y -> / Term
            right = Term();
            if (right != null)
                return new BinaryExpression(left, Operator.Div, right);
        }
        if (Reset(pos) && Match(TokenValues.Grammar["%"])){ // Y -> % Term
            right = Term();
            if (right != null)
                return new BinaryExpression(left, Operator.Mod, right);
        }
        if (Reset(pos) && Match(TokenValues.Grammar["^"])){ // Y -> ^ Term
            right = Term();
            if (right != null)
                return new BinaryExpression(left, Operator.Pow, right);
        }

        Reset(pos);
        return left; // Y -> e
    }
    #endregion

    #region Parsing Boolean Expressions
    private Expression Bool()
    {
        int pos = index;
        Expression expr;

        if (Match(TokenValues.Grammar["true"]) 
        || Reset(pos) && Match(TokenValues.Grammar["false"])){ // Bool -> BoolLit OpBool
            expr = OpBool(new BooleanLiteral(tokens[index - 1].Value));
            if (expr != null)
                return expr;
        }
        if (Reset(pos) && Match(TokenValues.Grammar["("])){ // Bool -> (Bool) OpBool
            expr = Bool();
            if (expr != null && Match(TokenValues.Grammar[")"]))
                return OpBool(expr);
        }
        if (Reset(pos) && MatchIdentifier() && Match(TokenValues.Grammar["("])){ // Bool -> FuncCall X Y OpComp OpBool
            expr = OpBool(OpComp(Y(X(FuncCall()))));
            if (expr != null)
                return expr;
        }
        if (Reset(pos) && MatchIdentifier()){ // Bool -> Var X Y OpComp OpBool
            expr = OpBool(OpComp(Y(X(new Variable(tokens[index - 1].Value)))));
            if (expr != null)
                return expr;
        }
        if (Reset(pos) && Match(TokenValues.Grammar["!"])){ // Bool -> !Bool OpBool
            expr = OpBool(new NoBool(Bool()));
            if (expr != null)
                return expr;
        }
        if (Reset(pos)){ // Bool -> NumExpr OpComp OpBool
            expr = OpComp(NumericalExpression());
            if (expr != null)
                return OpBool(expr);
        }

        return null;
    }

    private Expression OpComp(Expression left)
    {
        if (left == null)
            return null;

        int pos = index;
        Expression right;

        if (Match(TokenValues.Grammar["<"])){ // OpComp -> < NumExpr
            right = NumericalExpression();
            if (right != null)
                return new BinaryExpression(left, Operator.Minor, right);
        }
        if (Reset(pos) && Match(TokenValues.Grammar["<="])){ // OpComp -> <= NumExpr
            right = NumericalExpression();
            if (right != null)
                return new BinaryExpression(left, Operator.MinorEqual, right);
        }
        if (Reset(pos) && Match(TokenValues.Grammar[">"])){ // OpComp -> > NumExpr
            right = NumericalExpression();
            if (right != null)
                return new BinaryExpression(left, Operator.Major, right);
        }
        if (Reset(pos) && Match(TokenValues.Grammar[">="])){ // OpComp -> >= NumExpr
            right = NumericalExpression();
            if (right != null)
                return new BinaryExpression(left, Operator.MajorEqual, right);
        }
        if (Reset(pos) && Match(TokenValues.Grammar["=="])){ // OpComp -> == NumExpr
            right = NumericalExpression();
            if (right != null)
                return new BinaryExpression(left, Operator.Equals, right);
        }
        if (Reset(pos) && Match(TokenValues.Grammar["!="])){ // OpComp -> != NumExpr
            right = NumericalExpression();
            if (right != null)
                return new BinaryExpression(left, Operator.NotEqual, right);
        }
        return null;
    }

    private Expression OpBool(Expression left)
    {
        if (left == null)
            return null;

        int pos = index;
        Expression right;
        if (Match(TokenValues.Grammar["&"])){ // OpBool -> & Bool
            right = Bool();
            if (right != null)
                return new BinaryExpression(left, Operator.And, right);
        }
        if (Reset(pos) && Match(TokenValues.Grammar["|"])){ // OpBool -> | Bool
            right = Bool();
            if (right != null)
                return new BinaryExpression(left, Operator.And, right);
        }

        Reset(pos);
        return left; // OpBool -> e
    }
    #endregion

    #region Parsing String Expressions
    private Expression StringExpression()
    {
        if (MatchString()){ // StrExpr -> String Conc
            Expression str = Concatenation(new StringLiteral(tokens[index - 2].Value));
            if (str != null)
                return str;
        }

        return null;
    }

    private Expression Concatenation(Expression left)
    {
        if (left == null)
            return null;
        
        int pos = index;

        if (Match(TokenValues.Grammar["@"])) // Conc -> @ Val
            return new BinaryExpression(left, Operator.Concat, Value());
        
        Reset(pos);
        return left; // Conc -> e
    }
    #endregion

    #endregion
}