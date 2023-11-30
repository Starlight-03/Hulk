namespace HULK;

public class Context // En el contexto nos ayuda a guardar información de variables y funciones en un cierto ámbito
{
    // Cada contexto es un ámbito donde se guardan los tipos y valores de las variables 
    // y los argumentos, cuerpos y contextos internos de las funciones

    // Cada contexto debe tener un contexto padre que guarda la información que puede "heredar" el contexto interno
    // Solo el contexto global no posee padre, entonces declaramos a su padre un contexto vacío

    private readonly Context? parent;

    private readonly Dictionary<string, (string, Type)> variables;

    private readonly Dictionary<string, Dictionary<string[], (Expression, Context)>> functions;

    public Context(Context? parent = null)
    {
        this.parent = (parent != null) ? parent : null;
        variables = new Dictionary<string, (string, Type)>();
        functions = new Dictionary<string, Dictionary<string[], (Expression, Context)>>();
    }

    public bool IsDefined(string variable) // Verifica si una variable pertenece a un contexto o a su padre según su identificador
                                           // Devuelve true si la variable buscada pertenece al contexto o a su padre
                                           // De lo contrario devuelve false
    {
        if (variables.ContainsKey(variable)){
            return true;
        }
        if (parent != null){
            return parent.IsDefined(variable);
        }
        return false;
    }
    
    public bool IsDefined(string function, int args) // Verifica si una función pertenece a un contexto o a su padre según su identificador y su cantidad de elementos
                                                     // Devuelve true si la función buscada pertenece al contexto o a su padre y posee la misma cantidad de argumentos
                                                     // De lo contrario devuelve false
    {
        if (functions.ContainsKey(function)){
            foreach (string[] arguments in functions[function].Keys){
                if (arguments.Length == args){
                    return true;
                }
            }
        }
        if (parent != null){
            return parent.IsDefined(function, args);
        }
        return false;
    }
    
    public void Define(string variable, string value = "", Type type = Type.NotSet) // Define una nueva variable dados su identificador 
                                                                                    // y un valor y tipo dados (ya iniciallizados por defecto)
                                                                                    // (En caso de que no exista antes)
    {
        variables.Add(variable, (value, type));
    }

    public void Define(string function, string[] args, Expression body, Context innerContext) // Define una nueva función en caso de que no exista antes
                                                                                              // dados un identificador, la cantidad de argumentos, 
                                                                                              // un cuerpo y un contexto interno
    {
        if (!IsDefined(function, args.Length)){
            if (!functions.ContainsKey(function)){  // Si la función es nueva, se inicializa antes de asignarle los valores
                functions[function] = new Dictionary<string[], (Expression, Context)>();
            }
            functions[function].Add(args, (body, innerContext));
        }
    }

    public void Define(string identifier, PredeterminedFunction function, Dictionary<string, Type> args) // Define una función predeterminada dado su tipo de función predeterminada,
                                                                                                         // su tipo devuelto y una lista de pares de identificador de argumentos y sus tipos
    {        
        Context innerContext = new Context(this);
        string[] arguments = new string[args.Count];
        int count = 0;
        foreach (string arg in args.Keys){
            innerContext.Define(arg, "", args[arg]);
            arguments[count++] = arg;
        }
        Define(identifier, arguments, function, innerContext);
    }

    public void Undefine(string function, string[] args) // Indefine una función si ya está definida
    {
        if (!functions[function].Remove(args)){
            if (parent != null){
                parent.Undefine(function, args);
            }
        }
    }

    public void SetValue(string variable, Expression expression) // Define un valor en una variable definida dado el identificador de la variable 
                                                                 // y su expresión de valor
                                                                 // También define el tipo de la variable, si no estaba definido antes, basado en su valor
    {
        if (IsDefined(variable)){
            if (variables.ContainsKey(variable)){
                if (variables[variable] == ("", Type.NotSet)){
                    string value = expression.Evaluate(this);
                    variables[variable] = (value, GetType(value));
                }
            }
            else if (parent != null){
                parent.SetValue(variable, expression);
            }
        }
    }

    public void SetValue(string variable, string value) // Define un valor en una variable definida dado el identificador de la variable 
                                                        // y un valor dado (si su valor es del mismo tipo que el tipo definido)
    {
        if (IsDefined(variable)){
            Type type = variables[variable].Item2;
            if (type != Type.NotSet && type == GetType(value))
                variables[variable] = (value, type);
        }
    }


    public static Type GType(string value) // Devuelve el tipo de valor que define el valor dado
    {
        if (value != ""){
            if (double.TryParse(value, out double n))
                return Type.Number;
            else if (bool.TryParse(value, out bool b))
                return Type.Boolean;
            else
                return Type.String;
        }
        return Type.NotSet;
    }

    public void SetType(string variable, Type type) // Define un tipo en una variable definida dado el identificador de la variable 
                                                    // y un tipo dado (si no se ha definido un tipo anteriormente)
    {
        if (IsDefined(variable)){
            if (variables.ContainsKey(variable)){
                (string value, Type varType) = variables[variable];
                if (varType == Type.NotSet){
                    variables[variable] = (value, type);
                }
            }
            else if (parent != null){
                parent.SetType(variable, type);
            }
        }
    }

    public string GetValue(string variable) // Devuelve el valor de la variable que se esté buscando dado su identificador 
                                            // (si está definida y tiene valor definido)
    {
        if (variables.ContainsKey(variable)){
            string value = variables[variable].Item1;
            if (value != "")
                return value;
        }
        if (parent != null){
            return parent.GetValue(variable);
        }
        return "";
    }

    public Type GetType(string variable) // Devuelve el tipo de la variable que se esté buscando dado su identificador
                                         // (si está definida y tiene un tipo definido)
    {
        if (variables.ContainsKey(variable)){
            Type type = variables[variable].Item2;
            if (type != Type.NotSet)
                return type;
        }
        if (parent != null){
            return parent.GetType(variable);
        }
        return Type.NotSet;
    }

    public Dictionary<string[], (Expression, Context)>? GetFunction(string function) // Devuelve la función buscada dado su identificador
                                                                                     // (si está definida la función)
    {
        if (functions.ContainsKey(function)){
            return functions[function];
        }
        if (parent != null){
            return parent.GetFunction(function);
        }
        return null;
    }
}