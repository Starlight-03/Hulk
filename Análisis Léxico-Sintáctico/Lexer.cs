namespace HULK;

public class Lexer // Esta clase nos ayuda a realizar el proceso de tokenización, dada una línea de código no vacía
{
    // Todo lexer creado tendrá una instancia de Error Léxico, la línea de código dada
    // que se va a estar utilizando, y un indizador que nos ayude a recorrerla

    // La línea la provee la llamada al método Tokenizer(string line)

    // En caso de que ocurra un error léxico al tokenizar, se lanza dicho error 
    // notificando al usuario que el token encontrado no es válido, y se devuelve 
    // el token como nulo, para que luego, de existir algún token nulo, se devuelva 
    // una lista nula y se detenga la ejecución para empezar desde el principio

    private readonly LexError lex;

    private string line;

    private int i;

    public Lexer()
    {
        lex = new LexError();
        line = "";
        i = 0;
    }

    public List<Token>? Tokenize(string? line) // Devuelve una lista de tokens a partir de una línea de código dada
    {
        // El recorrido del token es sencillo:
        // Se recorre toda la línea de código, caracter por caracter, y 
        // se va analizando, según el tipo de caracter que sea, qué tipo de token puede ser

        if (line == null) return null;
        else this.line = line; 
        List<Token> tokens = new List<Token>();

        for (; i < line.Length; i++){
            if (char.IsWhiteSpace(line[i])){            // Si se encuentra un espacio no se debe guardar nada
                continue;                               // Así que se salta
            }
            else if (line[i] == '\"'){                  // Si se encuentra con unas comillas dobles entonces lo siguiente debe ser una expressión de string
                tokens.Add(GetStringExpression());
            }                                           // Si se encuentra algún otro caracter válido hay que analizar cúal es y qué tipo de operador devuelve
            else if (char.IsLetterOrDigit(line[i]) || char.IsPunctuation(line[i]) || char.IsSymbol(line[i])){ // Y se devuelve dicho token
                tokens.Add(GetToken());
            }
        }
        
        foreach (Token token in tokens){                // Si al revisar la lista de tokens, existe al menos un token nulo,
            if (token == Token.GetToken("null")){       // significa que se ha lanzado un error léxico, así que se devuelve 
                return null;                            // que la lista es nula para detener la ejecución y volver a empezar
            }
        }
        return tokens;                                  // Si no existen errores, devolver la lista y continuar con la ejecución
    }

    public char Peek() => i < line.Length - 1 ? line[i + 1] : ' ';

    private Token GetToken() // Devuelve un token dado la línea dada y el indizador en donde comience
    {
        // Primero se analiza el caracter en el que se encuentra al inicio del nuevo token,
        // luego se hace la llamada a que se devuelva el tipo de token según el caracter del inicio

        if (char.IsPunctuation(line[i]) || char.IsSymbol(line[i]))
            return GetSeparatorOrOperator();
        else if (char.IsDigit(line[i]))
            return GetNumericLiteral();
        else if (char.IsLetter(line[i]))
            return GetKeywordOrIdentifier();
        else
            return Token.GetToken("null");
    }

    private Token GetSeparatorOrOperator() // Se devuelve un token que pudiera ser separador u operador
    {
        // Por razones de optimización de código se juntaron los métodos anteriormente separados GetSeparator y
        // GetOperator respectivamente
        // Ya que los separadores se analizan exactamente igual que los operadores de un solo caracter

        // Si el token de un caracter es un token válido (separador u operador de un caracter) 
        // y con el caracter siguiente deja de serlo, devolver ese token
        // Si no, si el token de dos caracteres es un token válido, devolverlo
        // De lo contrario, lanzar un error léxico y devolver un token nulo

        string token = line[i].ToString();

        if (Token.IsToken(token) && !Token.IsToken(token + Peek())){
            return Token.GetToken(token);
        }
        else{
            token += line[++i];

            if (Token.IsToken(token) && !Token.IsToken(token + Peek())){
                return Token.GetToken(token);
            }
            else{
                lex.Show($"\'{token}\' is not a valid token");
                return Token.GetToken("null");
            }
        }
    }

    private Token GetNumericLiteral() // Devuelve un token de tipo literal numérico
    {
        string token = line[i++].ToString();    // Si se encuentra un dígito, se busca si el token es un literal numérico
        bool point = false;

        for (; i < line.Length && i >= 0; i++){
            if (char.IsDigit(line[i])){
                token += line[i];
            }
            else if (line[i] == '.' && !point){ // Si se encuentra un punto, interpretarlo como un punto decimal y añadirlo
                token += line[i];               // Luego de él no debe existir ningún punto decimal o saltaría un error
                point = true;
            }
            else if (point && line[i] == '.'){ // En caso de existir otro punto o coma como decimal, lanzar un error léxico y devolver un token nulo
                lex.Show($"Cannot have more than one \'.\' on a number.");
                break;
            }
            else if (char.IsWhiteSpace(line[i]) || char.IsPunctuation(line[i]) || char.IsSymbol(line[i])){
                i--;
                return new Token(token, TokenType.NumericLiteral);
            }
            else{ // Si al tokenizar el número, se encuentra una letra o algún caracter extraño, lanzar un error léxico y devolver un token nulo
                lex.Show($"\'{token + line[i]}\' is not a valid token");
                break;
            }
        }
        return Token.GetToken("null");
    }

    private Token GetKeywordOrIdentifier() // Devuelve un token de tipo keyword o literal booleano o identificador
    {
        // Si se encuentra una letra, se busca si el token resultante es una palabra reservada o un identificador
        // Un identificador puede contener números pero no puede comenzar por un número porque se confunde con tokenizar 
        // un literal pero no puede comenzar por un número porque se confunde con tokenizar un literal numérico

        // Si al terminarse el token, este pertenece a los tokens predefinidos, entonces es una palabra reservada y se guarda como tal
        // De lo contrario, se guarda como identificador

        string token = line[i++].ToString();

        while (i < line.Length){
            if (char.IsLetterOrDigit(line[i])){
                token += line[i++];
            }
            else if (char.IsWhiteSpace(line[i]) || char.IsPunctuation(line[i]) || char.IsSymbol(line[i])){
                i--;
                return Token.IsToken(token) ? Token.GetToken(token) : new Token(token, TokenType.Identifier);
            }
        }

        lex.Show($"\'{token}\' is not a valid token");      // De no ser válido el token, se lanza un error léxico y se devuelve un token nulo
        return Token.GetToken("null");
    }

    private Token GetStringExpression()
    {
        // Se va guardando toda la expressión en el string hasta encontrar las próximas comillas dobles

        i++;
        string str = "";

        while (i < line.Length && line[i] != '\"'){
            str += line[i++];
        }

        if (line[i - 1] != '\"'){
            lex.Show("Missing closing '\"' in string expression.");
            return Token.GetToken("null");
        }
        
        return new Token(str, TokenType.StringLiteral);
    }
}