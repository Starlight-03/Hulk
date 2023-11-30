namespace HULK;

public class Parser // El parser revisa la semántica del código, la evalúa y devuelve un árbol de derivación de expressiones
{
    // Un parser posee un error sintáctico, una lista de tokens dada y un índice 
    // que nos ayuda a recorrer todos los tokens de dicha lista

    private readonly SyntaxError syntax;
    
    private List<Token> tokens;
    
    private int index;
    
    public Parser()
    {
        syntax = new SyntaxError();
        tokens = new List<Token>(0);
        index = 0;
    }

    public Expression? Parse(List<Token> tokens) // Evalúa la lista de tokens dada y devuelve la expressión resultante según la sintaxis
    {
        this.tokens = tokens;
        
        Expression? expr = Expression(); // Comenzar a "parsear" la expresión

        if (expr == null){ // De ser inválida la expressión, lanzar una excepción
            syntax.Show("Invalid expression.");
            return null;
        }
        
        if (index == tokens.Count - 1){
            if (!Match(Token.GetToken(";"))){ // Si no se encuentra un ";" al finalizar la línea, lanzar una excepción
                syntax.Show("Missing \'closing semicolon\'.");
                return null;
            }
            return expr; // Si no, devolver la expresión resultante
        }
        else if (index < tokens.Count - 1){ // Si ha ocurrido un error que no permitió terminar de parsear, lanzar una expresión
            syntax.Show($"Parsing error. Invalid expression after \'{tokens[index].Value}\'.");
            return null;
        }
        return null;
    }

    #region Herramientas para parsear
    private bool Match(Token token) // Determina si el token actual es igual al token dado
                                    // Devuelve verdadero de ser iguales, y falso de no serlo o si el índice sobrepasa la longitud de la lista
    {
        if (index < tokens.Count){
            if (tokens[index] == token){
                index++;
                return true;
            }
        }
        return false;
    }

    private bool Match(TokenType type) // Determina si el token actual tiene igual que el tipo dado
                                       // Devuelve verdadero de ser iguales, y falso de no serlo o si el índice sobrepasa la longitud de la lista
    {
        if (index < tokens.Count){
            if (tokens[index].Type == type){
                index++;
                return true;
            }
        }
        return false;
    }
    #endregion

    #region Parseando expresiones
    private Expression? Expression() // Comienza a parsear la expressión, analizando qué tipo de expresión puede ser
                                     // Sintaxis y posibles valores: Expr -> Print | Func | LetIn | IfElse | Val | (Expr)
    {
        int pos = index; // Se guarda la posición inicial para poder resetearla más tarde
        if (Match(TokenType.Keyword)){
            index--;
            if (Match(Token.GetToken("print"))) // Si la expresión comienza con un "print", se predice que es una expresión de llamada al print
                return Print();
            if (Match(Token.GetToken("function"))) // Si comienza con "function", se predice que es una expresión de declaración de función
                return Function();
            if (Match(Token.GetToken("let"))) // Si comienza con "let", se predice que es una expresión "let-in"
                return LetIn();
            if (Match(Token.GetToken("if"))) // Si comienza con "if", se predice que es una expresión "if-else"
                return IfElse();
        }
        Expression? expr = Value(); // Si no es una de las anteriores, entonces intentamos averiguar si es una expresión de un valor (numérico, booleano o de string)
        if (expr != null) // Si es válida (no es vacía) devolver dicha expresión
            return expr;
        else
            index = pos; // Si no, se resetea el index hasta la posición inicial

        if (Match(Token.GetToken("("))){ // Si no, en última instancia se intenta ver si es una expresión dentro de paréntesis
            expr = Expression(); // Se evalúa la expresión interna
            if (expr == null){ // Si no es válida la expresión interna, lanzar un error
                syntax.Show("Invalid expression after \'opening parenthesis\'.");
                return null;
            }
            if (!Match(Token.GetToken(")"))){ // Si no aparecen los paréntesis de cierre, lanzar una excepción
                syntax.Show($"Missing \'closing parenthesis\' after \'{tokens[index - 2].Value}\'.");
                return null;
            }
            return expr;
        } // Intentamos las expresiones por valores antes que una expresión entre paréntesis porque pueden existir expresiones numéricas o booleanas que comienzan 
          // entre paréntesis, y éstos son parte de esa expresión. Luego, de no ser así, es que se averigua que sea, por ejemplo, un let-in entre paréntesis
        return null; // Si ninguna de estas opciones es válida, devolver una expresión vacía
    }
    
    private Expression? Print() // Parsea una expresión de llamada al print
                                // Sintaxis: Func -> print(Expr)
    {
        if (!Match(Token.GetToken("("))){ // Si no aparece un paréntesis de apertura después de la palabra reservada "print", lanzar un error
            syntax.Show("Missing \'opening parenthesis\' after \'print\' keyword in \'print\' expression.");
            return null;
        }

        Expression? expr = Expression(); // Se vuelve a llamar a expression para revisar la expresión en el interior del print

        if (expr == null){ // Si la expresión no es válida, y no se ha lanzado un error anteriormente, lanzar un error
            syntax.Show("Invalid expression in \'print\' expression.");
            return null;
        }

        if (!Match(Token.GetToken(")"))){ // Si no aparece un paréntesis de cierre después de la expresión interna, lanzar un error
            syntax.Show("Missing \'closing parenthesis\' in \'print\' expression.");
            return null;
        }
            
        return new Print(expr); // Y devuelve una expresión de llamada al print
    }
    
    private Expression? Function() // Parsea una expresión de declaración de función
                                   // Sintaxis: Func -> function Id(args) => Expr
    {
        if (!Match(TokenType.Identifier)){ // Si no aparece un identificador válido, lanzar una excepción. En caso contrario, guardar el valor del identificador
            syntax.Show($"Invalid token \'{tokens[index - 1].Value}\' in \'function definition\' expression.");
            return null;
        }

        string id = tokens[index - 1].Value;
        if (!Match(Token.GetToken("("))){ // Si no aparece un paréntesis de apertura después del identificador, lanzar una excepción
            syntax.Show($"Missing \'opening parenthesis\' after \'{tokens[index - 2].Value}\' in \'function definition\' expression.");
            return null;
        }

        List<string>? args = GetArgs(); // De lo contrario, recoger todos los argumentos (si es que hay alguno) hasta llegar a unos paréntesis de cierre
        if (args == null){ // Si ocurrió un error al recoger los argumentos, devolver una expresión vacía. De lo contrario, continuar
            return null;
        }

        if (!Match(Token.GetToken("=>"))){ // Si no aparece el operador de definición de función, lanzar un error
            syntax.Show("Missing \'=>\' operator in \'function definition\' expression.");
            return null;
        }

        Expression? body = Expression(); // De lo contrario, se revisa la expresión del cuerpo de la función
        if (body == null){ // Si la expresión del cuerpo de la función no es válida, lanzar una excepción
            syntax.Show("Invalid expression in \'function body\' in \'function definition\' expression.");
            return null;
        }

        return new FuncDef(id, args, body); // Si no ocurrieron errores, devolver una expresión de definición de función
    }

    private List<string>? GetArgs() // Devuelve una lista de los argumentos que posee una función para definirla
    {
        List<string> args = new List<string>();

        while (index < tokens.Count){
            if (Match(Token.GetToken(")"))){ // Si se encuentra un paréntesis de cierre se termina el bucle
                break;
            }
            if (Match(TokenType.Identifier)){ // Si todavía se está dentro del bucle, revisar si el próximo token es un identificador, si no lo es entonces se lanza un error
                args.Add(tokens[index - 1].Value);
            }
            else{
                syntax.Show($"Missing identifier after \'{tokens[index - 2].Value}\' in \'function definition\' expression.");
                return null;
            }
            if (Match(Token.GetToken(","))){ // Si se encuentra una coma, continuar con el bucle
                continue;
            }
        }
        // Si se cerró el bucle puede ser porque se llegó a un paréntesis, pero también puede ser porque se ha llegado al final de la línea
        if (tokens[index - 1] != Token.GetToken(")")){ // En caso de que haya sido la segunda causa, se verifica revisando si no se detuvo por encontrar un paréntesis de cierre
            syntax.Show($"Missing \'closing parenthesis\' after \'{tokens[index - 2].Value}\' in \'function definition\' expression."); // De ser así, lanzar una excepción
            return null;
        }

        return args; // Si no han ocurrido errores, devolver la lista de argumentos encontrada
    }
    
    private Expression? LetIn() // Parsea una expresión de tipo "let-in"
                                // Sintaxis: LetIn -> let Var = Val in Expr
    {
        Dictionary<string, Expression>? variables = GetVariables(); // Buscamos las variables y sus expresiones de valor asignadas después de la palabra reservada "let"
        if (variables == null){ // Si ocurrió un error en la recolección de variables y sus valores entonces devolvemos una expresión vacía
            return null;
        }
        
        if (!Match(Token.GetToken("in"))){ // Si no aparece la palabra reservada "in" después de las asignaciones de las variables, lanzar una excepción
            syntax.Show("Missing \'in\' keyword in \'let-in\' expression.");
            return null;
        }

        Expression? expr = Expression(); // Se revisa la expresión dentro del "in"
        if (expr == null){ // Si la expresión no es válida, lanzar una excepción
            syntax.Show("Invalid expression after \'in\' in \'let-in\' expression.");
            return null;
        }
        
        return new LetIn(variables, expr); // Si no han ocurrido errores, devolver una expresión de tipo "let-in"
    }

    private Dictionary<string, Expression>? GetVariables() // Recolecta todas las variables y sus expresiones de valor asignadas para las expresiones "let-in"
    {
        Dictionary<string, Expression> variables = new Dictionary<string, Expression>();

        do{
            if (!Match(TokenType.Identifier)){ // Mientras se pueda encontrar un identificador
                break;
            }

            string id = tokens[index - 1].Value; // Se guarda el identificador

            if (!Match(Token.GetToken("="))){ // Si no aparece un operador de asignación después del identificador, lanzar una excepción
                syntax.Show($"Missing \'assign operator\' after variable \'{id}\' in \'let-in\' expression.");
                return null;
            }

            Expression? expr = Value(); // Se revisa el valor que se le va a asignar a la variable

            if (expr == null){ // Si la expresión del valor no es válida, lanzar un eror (en caso de que no se haya lanzado antes)
                syntax.Show($"Invalid expression after variable \'{id}\' in \'let-in\' expression.");
                return null;
            }

            variables.Add(id, expr); // Si no hubo errores, entonces se guardan el identificador y su expresión asignada en la lista

        } while (Match(Token.GetToken(","))); // Si no aparece una coma, cerrar el ciclo

        return variables; // Una vez terminado el ciclo, devolver la lista de identificadores con sus expresiones asignadas
    }
    
    private Expression? IfElse() // Parsea una expresión de tipo "if-else"
                                 // Sintaxis: IfElse -> if (Bool) Expr else Expr
    {
        if (!Match(Token.GetToken("("))){ // Si no aparece un paréntesis de apertura después de la palabra reservada "if", lanzar una excepción
            syntax.Show("Missing \'opening parenthesis\' after \'if\' in \'if-else\' expression.");
            return null;
        }
        Expression? condition = Bool(); // Revisar la expresión booleana de la condición del "if"
        if (condition == null){ // Si la expresión no es válida, lanzar una excepción
            syntax.Show("Invalid \'condition expression\' after \'if\' in \'if-else\' expression.");
            return null;
        }
        if (!Match(Token.GetToken(")"))){ // Si no aparece un paréntesis de cierre despúes de la condición del "if", lanzar una excepción
            syntax.Show($"Missing \'closing parenthesis\' after \'{tokens[index - 2].Value}\' in \'if-else\' expression.");
            return null;
        }
        Expression? positive = Expression(); // Revisar la expresión del caso positivo
        if (positive == null){ // Si la expresión no es válida, lanzar una excepción
            syntax.Show("Invalid \'positive expression\' in \'if-else\' expression.");
            return null;
        }
        if (!Match(Token.GetToken("else"))){ // Si no aparece la palabra clave "else" después de la expresión positiva, lanzar una excepción
            syntax.Show("Missing \'else\' keyword in \'if-else\' expression.");
            return null;
        }
        Expression? negative = Expression(); // Revisar la expresión del caso negativo
        if (negative == null){ // Si la expresión no es válida, lanzar una excepción
            syntax.Show("Invalid \'negative expression\' after \'else\' in \'if-else\' expression.");
            return null;
        }
        return new IfElse(condition, positive, negative); // Si no ocurrieron errores, devolver una expresión de tipo "if-else"
    }

    private Expression? Value() // Parsea una expresión de tipo por valor
                                // Sintaxis y posibles valores: Val -> StrExpr | Bool : NumExpr Compar NumExpr | NumExpr 
                                //                                  | FuncCall X Y Concat | Var X Y Concat
    {
        if (Match(TokenType.StringLiteral)) // Si el valor es un literal de string, devolver una expresión de string
           return Concatenation(new StringLiteral(tokens[index - 1].Value));

        int pos = index; // Se guarda la posición inicial para poder resetearla más tarde
        if (Match(TokenType.NumericLiteral) || Match(Token.GetToken("("))){ // Si lo siguiente es una expresión numérica, revisar si es una expresión de compraración o una simple expresión numérica
            for (int i = index; i < tokens.Count; i++){ // Si próximamente se encuentra un operador de comparación después de una expresión numérica
                if (tokens[i] == Token.GetToken("<") || tokens[i] == Token.GetToken("<=")
                    || tokens[i] == Token.GetToken(">") || tokens[i] == Token.GetToken(">=")
                    || tokens[i] == Token.GetToken("==") || tokens[i] == Token.GetToken("!=")){
                        index = pos; // Reseteamos el index a la posición inicial
                        Expression? expr = OpBool(OpComp(NumericalExpression())); // Revisar si es válida la expresión de comparación
                        if (expr != null) // Si es válida, devolver la expresión. Si no, revisar si es simplemente una expresión numérica
                            return expr;
                }
            }
            index = pos; // Reseteamos el index a la posición inicial
            return NumericalExpression(); // Y devolvemos la expresión numérica
        }

        if (Match(TokenType.Identifier)) // Si el siguiente token es un identificador: y además le sucede un paréntesis de apertura, entonces devolver una llamada a una función. Si no, entonces devolver una llamada a una variable
            return Concatenation(OpBool(OpComp(X(Y(Match(Token.GetToken("(")) ? 
                    FuncCall() : new Variable(tokens[index - 1].Value))))));

        return Bool(); // Y en última instancia buscamos una expresión booleana

        // * Tener en cuenta que el return de un llamado a una expresión numérica o booleana puede devolver una expresión no válida,
        // pero de ser así, se debe haber lanzado un error anteriormente, así que podemos devolver la expresión vacía sin preocupaciones
    }

    private Expression? FuncCall() // Parsea el llamado a una función
                                   // Sintaxis: Term -> FuncCall Y
    {
        string id = tokens[index - 2].Value; // Se guarda el identificador de la función
        List<Expression> args = new List<Expression>(); // Se crea la lista de argumentos de la función que se va a llamar
        
        while (index < tokens.Count - 1){
            if (Match(Token.GetToken(")"))){ // Si se encuentra un paréntesis de cierre se termina el bucle
                index--;
                break;
            }

            Expression? expr = Value(); // Se revisa el valor del argumento

            if (expr == null){ // Si la expresión del argumento no es válida, ya se habrá lanzado una aexcepción anteriormente, así que solo se devuelve una expresión vacía
                return null;
            }
            
            args.Add(expr); // De lo contrario, se guarda el argumento en la lista

            if (Match(Token.GetToken(","))){ // Si se encuentra una coma, continuar con el bucle
                continue;
            }
        }
        // Si se cerró el bucle puede ser porque se llegó a un paréntesis, pero también puede ser porque se ha llegado al final de la línea
        if (!Match(Token.GetToken(")"))){ // En caso de que haya sido la segunda causa, se verifica revisando si no se detuvo por encontrar un paréntesis de cierre
            syntax.Show($"Missing \'closing parenthesis\' after \'{tokens[index - 1].Value}\'."); // De ser así, lanzar una excepción
            return null;
        }

        return new FuncCall(id, args); // Si no hubo errores, entonces se devuelve una expresión de llamado de una función
    }

    private Expression? NumericalExpression() // Parsea una expresión numérica
                                              // Sintaxis: NumExpr -> Term X
    {
        return X(Term());
    }

    private Expression? Term() // Parsea un término (numérico)
                               // Sintaxis y posibles expresiones: Term -> int Y | (NumExpr) Y | FuncCall Y | Var Y | LetIn Y
    {
        if (Match(TokenType.NumericLiteral)){
            return Y(new NumericLiteral(tokens[index - 1].Value));
        }
        else if (Match(Token.GetToken("("))){
            Expression? term = NumericalExpression();
            if (term == null){
                return null;
            }
            if (!Match(Token.GetToken(")"))){
                syntax.Show($"Missing \'closing parenthesis\' after \'{tokens[index - 1].Value}\'.");
                return null;
            }
            return Y(term); 
        }
        else if (Match(TokenType.Identifier)){
            return Y(Match(Token.GetToken("(")) ? FuncCall() : new Variable(tokens[index].Value));
        }
        else if (Match(Token.GetToken("let"))){
            return Y(LetIn());
        }
        return null;
    }

    private Expression? X(Expression? left) // Parsea las operaciones de segundo nivel (adición y diferencia)
                                            // Sintaxis y posibles expresiones: X -> + Z | - Z | e
                                            //                                  Z -> int Y X | (NumExpr) Y X
    {
        if (left == null)
            return null;

        if (!Match(Token.GetToken("+")) && !Match(Token.GetToken("-")))
            return left;

        string op = tokens[index - 1].Value;
        Expression? right = null;

        if (Match(Token.GetToken("("))){
            right = Y(NumericalExpression());
            if (!Match(Token.GetToken(")"))){
                syntax.Show($"Missing \'closing parenthesis\' after \'{tokens[index - 1].Value}\'.");
                return null;
            }
        }
        else
            right = Y(Match(TokenType.NumericLiteral) ? new NumericLiteral(tokens[index - 1].Value) : 
                        Match(TokenType.Identifier) ? Match(Token.GetToken("(")) ? FuncCall() : 
                            new Variable(tokens[index - 1].Value) : 
                        null);

        if (right == null){
            syntax.Show($"Missing \'numeric expression\' after \'{op}\' operator.");
            return null;
        }

        return X(op == "+" ? new Sum(left, right) : new Sub(left, right));
    }

    private Expression? Y(Expression? left) // Parsea las operaciones de primer nivel (multiplicación, división, módulo y potencia)
                                            // Sintaxis y posibles expresiones: X -> * Z | / Z | % Z | ^ Z | e
                                            //                                  Z -> int Y | (NumExpr) Y
    {
        if (left == null)
            return null;

        if (!Match(Token.GetToken("*")) && !Match(Token.GetToken("/")) 
            && !Match(Token.GetToken("%")) && !Match(Token.GetToken("^")))
            return left;

        string op = tokens[index - 1].Value;
        Expression? right = null;

        if (Match(Token.GetToken("("))){
            right = NumericalExpression();
            if (!Match(Token.GetToken(")"))){
                syntax.Show($"Missing \'closing parenthesis\' after \'{tokens[index - 1].Value}\'.");
                return null;
            }
        }
        else
            right = Match(TokenType.NumericLiteral) ? new NumericLiteral(tokens[index - 1].Value) : 
                    Match(TokenType.Identifier) ? Match(Token.GetToken("(")) ? FuncCall() : 
                        new Variable(tokens[index - 1].Value) : 
                    null;

        if (right == null){
            syntax.Show($"Missing \'numeric expression\' after \'{op}\' operator.");
            return null;
        }

        return Y(op == "*" ? new Mul(left, right) : 
                op == "/" ? new Div(left, right) : 
                op == "%" ? new Mod(left, right) : new Pow(left, right));
    }

    private Expression? Bool() // Parsea una expresión booleana
                               // Sintaxis y posibles expresiones: Bool -> BoolLit OpBool | (Bool) OpBool | FuncCall X Y OpComp OpBool 
                               //                                       | Var X Y OpComp OpBool | !Bool OpBool | NumExpr OpComp OpBool
    {
        if (Match(TokenType.BooleanLiteral)){
            return OpBool(new BooleanLiteral(tokens[index - 1].Value));
        }
        else if (Match(Token.GetToken("("))){
            Expression? expr = Bool();
            if (expr == null){
                return null;
            }
            if (!Match(Token.GetToken(")"))){
                syntax.Show($"Missing \'closing parenthesis\' after \'{tokens[index - 1].Value}\'.");
                return null;
            }
            return OpBool(expr);
        }
        else if (Match(TokenType.Identifier)){
            return OpBool(OpComp(Y(X(Match(Token.GetToken("(")) ? FuncCall() : new Variable(tokens[index - 1].Value)))));
        }
        else if (Match(Token.GetToken("!"))){
            Expression? expr = Bool();
            if (expr == null)
                return null;
            return OpBool(new NoBool(expr));
        }
        return OpBool(OpComp(NumericalExpression()));
    }

    private Expression? OpComp(Expression? left) // Parsea una operación de comparación
                                                 // Sintaxis y posibles expresiones: OpComp -> < NumExpr | <= NumExpr | > NumExpr
                                                 //                                         | >= NumExpr | == NumExpr | != NumExpr | e
    {
        if (left == null)
            return null;

        if (!Match(Token.GetToken("<")) && !Match(Token.GetToken("<=")) 
            && !Match(Token.GetToken(">")) && !Match(Token.GetToken(">=")) 
            && !Match(Token.GetToken("==")) && !Match(Token.GetToken("!=")))
            return left;

        string op = tokens[index - 1].Value;
        Expression? right = NumericalExpression();

        if (right == null){
            syntax.Show($"Missing \'numeric expression\' after \'{op}\' operator.");
            return null;
        }

        return op == "<" ? new Minor(left, right) : 
                op == "<=" ? new MinorEqual(left, right) : 
                op == ">" ? new Major(left, right) : 
                op == ">=" ? new MajorEqual(left, right) : 
                op == "==" ? new Equals(left, right) : new NotEqual(left, right);
    }

    private Expression? OpBool(Expression? left) // Parsea una operación booleana (Y lógico u O lógico)
                                                 // Sintaxis y posibles expresiones: OpBool -> & Bool |  | Bool | e
    {
        if (left == null){
            return null;
        }

        if (!Match(Token.GetToken("&")) && !Match(Token.GetToken("|")))
            return left;

        string op = tokens[index - 1].Value;
        Expression? right = Bool();

        if (right == null){
            syntax.Show($"Missing \'numeric expression\' after \'{op}\' operator.");
            return null;
        }

        return op == "&" ? new And(left, right) : new Or(left, right);
    }

    private Expression? Concatenation(Expression? left) // Devuelve una expresión binaria de concatenación dada una expresión de string en el lado izquierdo de la concatenación
                                                        // Sintaxis: Conc -> @ Val | e
    {
        if (left == null)
            return null;

        if (!Match(Token.GetToken("@")))
            return left;
        
        Expression? right = Value();
        return right != null ? new Concat(left, right) : null;
    }
    #endregion
}