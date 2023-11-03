namespace HULK;

public class Parser // El parser revisa la semántica del código, la evalúa y devuelve un árbol de derivación de expressiones
{
    private SyntaxError syntax;
    
    private List<Token> tokens;
    
    private int index;
    
    public Parser()
    {
        syntax = new SyntaxError();
    }

    public Expression Parse(List<Token> tokens)
    {
        this.tokens = tokens;
        index = 0;
        
        Expression expr = Expression();

        if (expr == null){
            syntax.Show("Invalid expression.");
            return null;
        }
        
        if (index == tokens.Count - 1 && Match(Token.GetToken(";"))){
            return expr;
        }
        else{
            syntax.Show("Missing \'closing semicolon\'.");
            return null;
        }
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
        if (Match(Token.GetToken("print"))) return Print(); // Expr -> Print
        if (Reset(pos) && Match(Token.GetToken("function"))) return Function(); // Expr -> Func
        if (Reset(pos) && Match(Token.GetToken("let"))) return LetIn(); // Expr -> LetIn
        if (Reset(pos) && Match(Token.GetToken("if"))) return IfElse(); // Expr -> IfElse

        Reset(pos);
        Expression expr = Value(); // Expr -> Val
        if (expr != null) return expr;

        if (Reset(pos) && Match(Token.GetToken("("))){ // Expr -> (Expr)
            expr = Expression();
            if (expr == null){
                syntax.Show("Invalid expression after \'opening parenthesis\'.");
                return null;
            }
            if (!Match(Token.GetToken(")"))){
                syntax.Show($"Missing \'closing parenthesis\' after \'{tokens[index - 2].Value}\'.");
                return null;
            }
            return expr;
        }

        return null;
    }
    
    private Expression Print()
    {
        // Func -> print(Expr)
        if (!Match(Token.GetToken("("))){
            syntax.Show("Missing \'opening parenthesis\' after \'print\' keyword in \'print\' expression.");
            return null;
        }

        Expression expr = Expression();

        if (expr == null){
            syntax.Show("Invalid expression in \'print\' expression.");
            return null;
        }

        if (!Match(Token.GetToken(")"))){
            syntax.Show("Missing \'closing parenthesis\' in \'print\' expression.");
            return null;
        }
            
        return new Print(expr);
    }
    
    private Expression Function()
    {
        // Func -> function Id(args) => Expr
        if (!Match(TokenType.Identifier)){
            syntax.Show($"Invalid token \'{tokens[index - 1].Value}\' in \'function definition\' expression.");
            return null;
        }

        string id = tokens[index - 1].Value;
        
        if (!Match(Token.GetToken("("))){
            syntax.Show($"Missing \'opening parenthesis\' after \'{tokens[index - 2].Value}\' in \'function definition\' expression.");
            return null;
        }

        List<string> args = GetArgs();

        if (!Match(Token.GetToken("=>"))){
            syntax.Show("Missing \'=>\' operator in \'function definition\' expression.");
            return null;
        }

        Expression body = Expression();

        if (body == null){
            syntax.Show("Invalid expression in \'function body\' in \'function definition\' expression.");
            return null;
        }

        return new FuncDef(id, args, body);
    }

    private List<string> GetArgs()
    {
        List<string> args = new List<string>();

        while (index < tokens.Count){
            if (Match(Token.GetToken(")"))){
                break;
            }
            else{
                index--;
            }
            if (Match(TokenType.Identifier)){
                args.Add(tokens[index - 1].Value);
            }
            else{
                syntax.Show($"Missing identifier after \'{tokens[index - 2].Value}\' in \'function definition\' expression.");
                return null;
            }
            if (!Match(Token.GetToken(","))){
                index--;
            }
        }

        if (tokens[index - 1] != Token.GetToken(")")){
            syntax.Show($"Missing \'closing parenthesis\' after \'{tokens[index - 2].Value}\' in \'function definition\' expression.");
            return null;
        }

        return args;
    }
    
    private Expression LetIn()
    {
        // LetIn -> let Var = Val in Expr
        Dictionary<string, Expression> variables = GetVariables();
        
        if (!Match(Token.GetToken("in"))){
            syntax.Show("Missing \'in\' keyword in \'let-in\' expression.");
            return null;
        }

        Expression expr = Expression();

        if (expr == null){
            syntax.Show("Invalid expression after \'in\' in \'let-in\' expression.");
            return null;
        }
        
        return new LetIn(variables, expr);
    }

    private Dictionary<string, Expression> GetVariables()
    {
        Dictionary<string, Expression> variables = new Dictionary<string, Expression>();

        while (Match(TokenType.Identifier)){
            string id = tokens[index - 1].Value;

            if (!Match(Token.GetToken("="))){
                syntax.Show($"Missing \'assign operator\' after variable \'{id}\' in \'let-in\' expression.");
                return null;
            }

            Expression expr = Value();

            if (expr == null){
                syntax.Show($"Invalid expression after variable \'{id}\' in \'let-in\' expression.");
                return null;
            }
            
            variables.Add(id, expr);

            if (!Match(Token.GetToken(","))){
                index--;
                break;
            }
        }

        return variables;
    }
    
    private Expression IfElse()
    {
        // IfElse -> if (Bool) Expr else Expr
        if (!Match(Token.GetToken("("))){
            syntax.Show("Missing \'opening parenthesis\' after \'if\' in \'if-else\' expression.");
            return null;
        }
        Expression condition = Bool();
        if (condition == null){
            syntax.Show("Invalid \'condition expression\' after \'if\' in \'if-else\' expression.");
            return null;
        }
        if (!Match(Token.GetToken(")"))){
            syntax.Show($"Missing \'closing parenthesis\' after \'{tokens[index - 2].Value}\' in \'if-else\' expression.");
            return null;
        }
        Expression positive = Expression();
        if (positive == null){
            syntax.Show("Invalid \'positive expression\' in \'if-else\' expression.");
            return null;
        }
        if (!Match(Token.GetToken("else"))){
            syntax.Show("Missing \'else\' keyword in \'if-else\' expression.");
            return null;
        }
        Expression negative = Expression();
        if (negative == null){
            syntax.Show("Invalid \'negative expression\' after \'else\' in \'if-else\' expression.");
            return null;
        }
        return new IfElse(condition, positive, negative);
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
            syntax.Show($"Missing \'closing parenthesis\' after \'{tokens[index - 2].Value}\'.");
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
            return Y(new NumericLiteral(tokens[index - 1].Value));
        }
        else if (Reset(pos) && Match(Token.GetToken("("))){ // Term -> (NumExpr) Y
            Expression term = Y(NumericalExpression());
            if (term == null){
                return null;
            }
            if (!Match(Token.GetToken(")"))){
                syntax.Show($"Missing \'closing parenthesis\' after \'{tokens[index - 2].Value}\'.");
                return null;
            }
            return term; 
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
            return X(left, new Operator(Op.Sum));
        }
        if (Reset(pos) && Match(Token.GetToken("-"))){ // X -> - NumExpr
            return X(left, new Operator(Op.Sub));
        }

        Reset(pos);
        return left; // X -> e
    }

    private Expression X(Expression left, Operator op)
    {
        Expression right = NumericalExpression();
        
        if (right == null){
            syntax.Show($"Missing \'numeric expression\' after \'{op}\' operator.");
            return null;
        }
        
        return new BinaryExpression(left, op, right);
    }

    private Expression Y(Expression left)
    {
        int pos = index;

        if (Match(Token.GetToken("*"))){ // Y -> * Term
            return Y(left, new Operator(Op.Mul));
        }
        else if (Reset(pos) && Match(Token.GetToken("/"))){ // Y -> / Term
            return Y(left, new Operator(Op.Div));
        }
        else if (Reset(pos) && Match(Token.GetToken("%"))){ // Y -> % Term
            return Y(left, new Operator(Op.Mod));
        }
        else if (Reset(pos) && Match(Token.GetToken("^"))){ // Y -> ^ Term
            return Y(left, new Operator(Op.Pow));
        }

        Reset(pos);
        return left; // Y -> e
    }

    private Expression Y(Expression left, Operator op)
    {
        Expression right = Term();

        if (right == null){
            syntax.Show($"Missing \'numeric term\' after \'{op}\' operator.");
            return null;
        }
        
        return new BinaryExpression(left, op, right);
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
            if (expr == null){
                return null;
            }
            if (!Match(Token.GetToken(")"))){
                syntax.Show($"Missing \'closing parenthesis\' after \'{tokens[index - 2].Value}\'.");
                return null;
            }
            return OpBool(expr);
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
            return OpComp(left, new Operator(Op.Minor));
        }
        else if (Reset(pos) && Match(Token.GetToken("<="))){ // OpComp -> <= NumExpr
            return OpComp(left, new Operator(Op.MinorEqual));
        }
        else if (Reset(pos) && Match(Token.GetToken(">"))){ // OpComp -> > NumExpr
            return OpComp(left, new Operator(Op.Major));
        }
        else if (Reset(pos) && Match(Token.GetToken(">="))){ // OpComp -> >= NumExpr
            return OpComp(left, new Operator(Op.MajorEqual));
        }
        else if (Reset(pos) && Match(Token.GetToken("=="))){ // OpComp -> == NumExpr
            return OpComp(left, new Operator(Op.Equals));
        }
        else if (Reset(pos) && Match(Token.GetToken("!="))){ // OpComp -> != NumExpr
            return OpComp(left, new Operator(Op.NotEqual));
        }
        else
            return null;
    }

    private Expression OpComp(Expression left, Operator op)
    {
        Expression right = NumericalExpression();
        
        if (right == null){
            syntax.Show($"Missing \'numerical expression\' after \'{op}\' operator.");
            return null;
        }
        
        return new BinaryExpression(left, op, right);
    }

    private Expression OpBool(Expression left)
    {
        if (left == null)
            return null;

        int pos = index;

        if (Match(Token.GetToken("&"))){ // OpBool -> & Bool
            return OpBool(left, new Operator(Op.And));
        }
        if (Reset(pos) && Match(Token.GetToken("|"))){ // OpBool -> | Bool
            return OpBool(left, new Operator(Op.Or));
        }

        Reset(pos);
        return left; // OpBool -> e
    }

    private Expression OpBool(Expression left, Operator op)
    {
        Expression right = Bool();
        
        if (right == null){
            syntax.Show($"Missing \'boolean expression\' after \'{op}\' operator.");
            return null;
        }

        return new BinaryExpression(left, op, right);
    }
    #endregion

    #region Parsing String Expressions
    private Expression StringExpression()
    {
        // StrExpr -> String Conc
        if (!Match(Token.GetToken("\""))){
            syntax.Show($"Missing \'closing \"\' after \'{tokens[index - 1].Value}\'.");
            return null;
        }

        return Concatenation(new StringLiteral(tokens[index - 2].Value));
    }

    private Expression Concatenation(Expression left)
    {
        int pos = index;

        if (Match(Token.GetToken("@"))) // Conc -> @ Val
            return new BinaryExpression(left, new Operator(Op.Concat), Value());
        
        Reset(pos);
        return left; // Conc -> e
    }
    #endregion
    #endregion
    #endregion
}