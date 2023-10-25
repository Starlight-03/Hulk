public class Parser
{
    // El parser revisa la semántica del código, la evalúa y devuelve un árbol de derivación
    public SyntaxError Syntax { get; private set; }

    public Expression Expr { get; private set; }
    
    private readonly List<Token> tokens;
    
    private int index;
    
    public Parser(List<Token> tokens)
    {
        Syntax = new SyntaxError();
        this.tokens = tokens;
        index = 0;
        Expr = Parse();
    }

    private Expression Parse()
    {
        Expr = Expression();
        if (Expr != null){
            if (index == tokens.Count - 1 && Match(Token.GetToken(";")))
                return Expr;
            else
                Syntax.Info = "Missing \'closing semicolon\'.";
        }
        return null;
    }

    #region Parsing Tools
    private bool Match(Token token)
    {
        if (index < tokens.Count)
            return tokens[index++] == token;
        else
            return false;
    }

    private bool Match(TokenType type)
    {
        if (index < tokens.Count)
            return tokens[index++].Type == type;
        else
            return false;
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
        if (Match(Token.GetToken("print"))){ // Expr -> Print
            return Print();
        }
        if (Reset(pos) && Match(Token.GetToken("function"))){ // Expr -> Func
            return Function();
        }
        if (Reset(pos) && Match(Token.GetToken("let"))){ // Expr -> LetIn
            return LetIn();
        }
        if (Reset(pos) && Match(Token.GetToken("if"))){ // Expr -> IfElse
            return IfElse();
        }
        Reset(pos);
        Expression expr = Value(); // Expr -> Val
        if (expr != null) 
            return expr;
        if (Reset(pos) && Match(Token.GetToken("("))){ // Expr -> (Expr)
            expr = Expression();
            if (expr != null){
                if (Match(Token.GetToken(")")))
                    return expr;
                Syntax.Info = $"Missing \'closing parenthesis\' after \'{tokens[index - 2].Value}\'.";
            }
            Syntax.Info = "Invalid expression after \'opening parenthesis\'.";
        }
        Syntax.Info = "Invalid expression.";
        return null;
    }
    
    private Expression Print()
    {
        // Func -> print(Expr)
        if (Match(Token.GetToken("("))){
            Expression expr = Expression();
            if (expr != null  && Match(Token.GetToken(")")))
                return new Print(expr);
            else 
                Syntax.Info = "Invalid expression in \'print\' expression.";
        }
        else 
            Syntax.Info = "Missing \'opening parenthesis\' after \'print\' keyword in \'print\' expression.";
        
        return null;
    }
    
    private Expression Function()
    {
        // Func -> function Id(args) => Expr
        string id;
        if (Match(TokenType.Identifier)) id = tokens[index - 1].Value;
        else{
            Syntax.Info = $"Invalid token \'{tokens[index - 1].Value}\' in \'function definition\' expression.";
            return null;
        }
        List<string> args = new List<string>();
        if (Match(Token.GetToken("("))){
            while (index < tokens.Count){
                if (Match(Token.GetToken(")"))) break;
                else index--;
                if (Match(TokenType.Identifier)) args.Add(tokens[index - 1].Value);
                else Syntax.Info = $"Missing identifier after \'{tokens[index - 2].Value}\' in \'function definition\' expression.";
                if (!Match(Token.GetToken(","))) index--;
            }
            if (tokens[index - 1] != Token.GetToken(")")) Syntax.Info = $"Missing \'closing parenthesis\' after \'{tokens[index - 2].Value}\' in \'function definition\' expression.";
        }
        else Syntax.Info = $"Missing \'opening parenthesis\' after \'{tokens[index - 2].Value}\' in \'function definition\' expression.";
        if (!Match(Token.GetToken("=>"))){
            Syntax.Info = "Missing \'=>\' operator in \'function definition\' expression.";
            return null;
        }
        Expression body = Expression();
        if (body != null) return new FuncDef(id, args, body);
        else{
            Syntax.Info = "Invalid expression in \'function body\' in \'function definition\' expression.";
            return null;
        }
    }
    
    private Expression LetIn()
    {
        // LetIn -> let Var = Val in Expr
        Dictionary<string, Expression> variables = new Dictionary<string, Expression>();
        while (Match(TokenType.Identifier)){
            string id = tokens[index - 1].Value;
            if (!Match(Token.GetToken("="))){
                Syntax.Info = $"Missing \'assign operator\' after \'{id}\' in \'let-in\' expression.";
                return null;
            }
            Expression expr = Value();
            if (expr != null) variables.Add(id, expr);
            else{
                Syntax.Info = $"Invalid expression after \'{id}\' in \'let-in\' expression.";
                return null;
            }
            if (!Match(Token.GetToken(","))){
                index--; break;
            }
        }
        if (Match(Token.GetToken("in"))){
            Expression expr = Expression();
            if (expr != null)
                return new LetIn(variables, expr);
            else
                Syntax.Info = "Invalid expression after \'in\' in \'let-in\' expression.";
        }
        else if (Match(TokenType.Identifier))
            Syntax.Info = $"Invalid token \'{tokens[index - 1].Value}\' in \'let-in\' expression.";
        else
            Syntax.Info = "Missing \'in\' keyword in \'let-in\' expression.";
        return null;
    }
    
    private Expression IfElse()
    {
        // IfElse -> if (Bool) Expr else Expr
        if (Match(Token.GetToken("("))){
            Expression condition = Bool();
            if (condition != null){
                if (Match(Token.GetToken(")"))){
                    Expression positive = Expression();
                    if (positive != null){
                        if (Match(Token.GetToken("else"))){
                            Expression negative = Expression();
                            if (negative != null)
                                return new IfElse(condition, positive, negative);
                            else
                                Syntax.Info = "Invalid \'negative expression\' after \'else\' in \'if-else\' expression.";
                        }
                        else Syntax.Info = "Missing \'else\' keyword in \'if-else\' expression.";
                    }
                    else Syntax.Info = "Invalid \'positive expression\' in \'if-else\' expression.";
                }
                else Syntax.Info = $"Missing \'closing parenthesis\' after \'{tokens[index - 2].Value}\' in \'if-else\' expression.";
            }
            else Syntax.Info = "Invalid \'condition expression\' after \'if\' in \'if-else\' expression.";
        }
        else Syntax.Info = "Missing \'opening parenthesis\' after \'if\' in \'if-else\' expression.";

        return null;
    }

    #region Parsing Values
    private Expression Value()
    {
        int pos = index;
        if (Match(Token.GetToken("\"")) && Match(TokenType.StringLiteral)){ // Val -> StrExpr
           return StringExpression();
        }
        else if (Reset(pos) && Match(TokenType.NumericLiteral) || Reset(pos) && Match(Token.GetToken("("))){
            Reset(pos);
            for (int i = index; i < tokens.Count; i++){
                if (tokens[i] == Token.GetToken("<") || tokens[i] == Token.GetToken("<=")
                    || tokens[i] == Token.GetToken(">") || tokens[i] == Token.GetToken(">=")
                    || tokens[i] == Token.GetToken("==") || tokens[i] == Token.GetToken("!=")){
                    Expression expr = OpBool(OpComp(NumericalExpression())); // Val -> Bool: NumExpr Compar NumExpr
                    if (expr != null) 
                        return expr;
                }
            }
            Reset(pos);
            return NumericalExpression(); // Val -> NumExpr
        }
        else if (Reset(pos) && Match(TokenType.Identifier) && Match(Token.GetToken("("))){ // Val -> FuncCall X Y Concat
            return X(Y(Concatenation(FuncCall())));
        }
        else if (Reset(pos) && Match(TokenType.Identifier)){ // Val -> Var X Y Concat
            return Concatenation(Y(X(new Variable(tokens[index - 1].Value))));
        }
        Reset(pos);
        return Bool();
    }

    private Expression FuncCall()
    {
        // Term -> FuncCall Y
        string id = tokens[index - 2].Value;
        List<Expression> args = new List<Expression>();
        
        while (index < tokens.Count){
            if (Match(Token.GetToken(")"))) 
                break;
            else
                index--;
            Expression expr = Value();
            if (expr != null)
                args.Add(expr);
            else
                return null;
            if (!Match(Token.GetToken(",")))
                index--;
        }

        if (tokens[index - 1] != Token.GetToken(")")){
            Syntax.Info = $"Missing \'closing parenthesis\' after \'{tokens[index - 2].Value}\'.";
            return null;
        }

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
        if (Match(TokenType.NumericLiteral)){ // Term -> int Y
            return Y(new Number(tokens[index - 1].Value));
        }
        else if (Reset(pos) && Match(Token.GetToken("("))){ // Term -> (NumExpr) Y
            Expression term = Y(NumericalExpression());
            if (term != null){
                if (Match(Token.GetToken(")")))
                    return term;
                Syntax.Info = $"Missing \'closing parenthesis\' after \'{tokens[index - 2].Value}\'.";
            }
            Syntax.Info = "Invalid \'numerical expression\' after opening parenthesis.";
        }
        else if (Reset(pos) && Match(TokenType.Identifier) && Match(Token.GetToken("("))){ // Term -> FuncCall Y
            return Y(FuncCall());
        }
        else if (Reset(pos) && Match(TokenType.Identifier)){ // Term -> Var Y
            return Y(new Variable(tokens[index - 1].Value));
        }
        else if (Reset(pos) && Match(Token.GetToken("let"))){ // Term -> LetIn Y
            return Y(LetIn());
        }

        return null;
    }

    private Expression X(Expression left)
    {
        int pos = index;

        if (Match(Token.GetToken("+"))){ // X -> + NumExpr
            return X(left, Operator.Sum);
        }
        if (Reset(pos) && Match(Token.GetToken("-"))){ // X -> - NumExpr
            return X(left, Operator.Sub);
        }

        Reset(pos);
        return left; // X -> e
    }

    private Expression X(Expression left, Operator op)
    {
        Expression right = NumericalExpression();
        if (right != null)
            return new BinaryExpression(left, op, right);
        else{
            string oper = "";
            switch (op){
                case Operator.Sum: oper = "+"; break;
                case Operator.Sub: oper = "-"; break;
            }
            Syntax.Info = $"Missing \'numeric expression\' after \'{oper}\' operator.";
            return null;
        }
    }

    private Expression Y(Expression left)
    {
        int pos = index;

        if (Match(Token.GetToken("*"))){ // Y -> * Term
            return Y(left, Operator.Mul);
        }
        else if (Reset(pos) && Match(Token.GetToken("/"))){ // Y -> / Term
            return Y(left, Operator.Div);
        }
        else if (Reset(pos) && Match(Token.GetToken("%"))){ // Y -> % Term
            return Y(left, Operator.Mod);
        }
        else if (Reset(pos) && Match(Token.GetToken("^"))){ // Y -> ^ Term
            return Y(left, Operator.Pow);
        }

        Reset(pos);
        return left; // Y -> e
    }

    private Expression Y(Expression left, Operator op)
    {
        Expression right = Term();
        if (right != null)
            return new BinaryExpression(left, op, right);
        else{
            string oper = "";
            switch (op){
                case Operator.Mul: oper = "*"; break;
                case Operator.Div: oper = "/"; break;
                case Operator.Mod: oper = "%"; break;
                case Operator.Pow: oper = "^"; break;
            }
            Syntax.Info = $"Missing \'numeric term\' after \'{oper}\' operator.";
            return null;
        }
    }
    #endregion

    #region Parsing Boolean Expressions
    private Expression Bool()
    {
        int pos = index;
        if (Match(TokenType.BooleanLiteral)){ // Bool -> BoolLit OpBool
            return OpBool(new BooleanLiteral(tokens[index - 1].Value));
        }
        else if (Reset(pos) && Match(Token.GetToken("("))){ // Bool -> (Bool) OpBool
            Expression expr = Bool();
            if (expr != null){
                if (Match(Token.GetToken(")")))
                    return OpBool(expr);
                else{
                    Syntax.Info = $"Missing \'closing parenthesis\' after \'{tokens[index - 2].Value}\'.";
                    return null;
                }
            }
            else
                return null;
        }
        else if (Reset(pos) && Match(TokenType.Identifier) && Match(Token.GetToken("("))){ // Bool -> FuncCall X Y OpComp OpBool
            return OpBool(OpComp(Y(X(FuncCall()))));
        }
        else if (Reset(pos) && Match(TokenType.Identifier)){ // Bool -> Var X Y OpComp OpBool
            return OpBool(OpComp(Y(X(new Variable(tokens[index - 1].Value)))));
        }
        else if (Reset(pos) && Match(Token.GetToken("!"))){ // Bool -> !Bool OpBool
            return OpBool(new NoBool(Bool()));
        }
        Reset(pos); // Bool -> NumExpr OpComp OpBool
        return OpBool(OpComp(NumericalExpression()));
    }

    private Expression OpComp(Expression left)
    {
        if (left == null)
            return null;

        int pos = index;

        if (Match(Token.GetToken("<"))){ // OpComp -> < NumExpr
            return OpComp(left, Operator.Minor);
        }
        else if (Reset(pos) && Match(Token.GetToken("<="))){ // OpComp -> <= NumExpr
            return OpComp(left, Operator.MinorEqual);
        }
        else if (Reset(pos) && Match(Token.GetToken(">"))){ // OpComp -> > NumExpr
            return OpComp(left, Operator.Major);
        }
        else if (Reset(pos) && Match(Token.GetToken(">="))){ // OpComp -> >= NumExpr
            return OpComp(left, Operator.MajorEqual);
        }
        else if (Reset(pos) && Match(Token.GetToken("=="))){ // OpComp -> == NumExpr
            return OpComp(left, Operator.Equals);
        }
        else if (Reset(pos) && Match(Token.GetToken("!="))){ // OpComp -> != NumExpr
            return OpComp(left, Operator.NotEqual);
        }
        else
            return null;
    }

    private Expression OpComp(Expression left, Operator op)
    {
        Expression right = NumericalExpression();
        if (right != null)
            return new BinaryExpression(left, op, right);
        else{
            string oper = "";
            switch (op){
                case Operator.Minor: oper = "<"; break;
                case Operator.MinorEqual: oper = "<="; break;
                case Operator.Major: oper = ">"; break;
                case Operator.MajorEqual: oper = ">="; break;
                case Operator.Equals: oper = "=="; break;
                case Operator.NotEqual: oper = "!="; break;
            }
            Syntax.Info = $"Missing \'numerical expression\' after \'{oper}\' operator.";
            return null;
        }
    }

    private Expression OpBool(Expression left)
    {
        if (left == null)
            return null;

        int pos = index;

        if (Match(Token.GetToken("&"))){ // OpBool -> & Bool
            return OpBool(left, Operator.And);
        }
        if (Reset(pos) && Match(Token.GetToken("|"))){ // OpBool -> | Bool
            return OpBool(left, Operator.Or);
        }

        Reset(pos);
        return left; // OpBool -> e
    }

    private Expression OpBool(Expression left, Operator op)
    {
        Expression right = Bool();
        if (right != null)
            return new BinaryExpression(left, op, right);
        else{
            string oper = "";
            switch (op){
                case Operator.And: oper = "&"; break;
                case Operator.Or: oper = "|"; break;
            }
            Syntax.Info = $"Missing \'boolean expression\' after \'{oper}\' operator.";
            return null;
        }
    }
    #endregion

    #region Parsing String Expressions
    private Expression StringExpression()
    {
        // StrExpr -> String Conc
        if (Match(Token.GetToken("\""))){
            Expression str = Concatenation(new StringLiteral(tokens[index - 2].Value));
            if (str != null)
                return str;
        }

        Syntax.Info = $"Missing \'closing \"\' after \'{tokens[index - 1].Value}\'.";
        return null;
    }

    private Expression Concatenation(Expression left)
    {
        int pos = index;

        if (Match(Token.GetToken("@"))) // Conc -> @ Val
            return new BinaryExpression(left, Operator.Concat, Value());
        
        Reset(pos);
        return left; // Conc -> e
    }
    #endregion
    #endregion
    #endregion
}