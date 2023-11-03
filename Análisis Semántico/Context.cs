namespace HULK;

public class Context
{
    private Context parent;

    private Dictionary<string, string> variables;

    private Dictionary<string, Type> variableTypes;

    private Dictionary<string, Dictionary<string[], (Expression, Context)>> functions;

    public Context(Context parent)
    {
        this.parent = parent;
        variables = new Dictionary<string, string>();
        variableTypes = new Dictionary<string, Type>();
        functions = new Dictionary<string, Dictionary<string[], (Expression, Context)>>();
    }

    public bool IsDefined(string variable)
    {
        if (variables.ContainsKey(variable)){
            return true;
        }
        if (parent != null){
            return parent.IsDefined(variable);
        }
        return false;
    }
    
    public bool IsDefined(string function, int args)
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
    
    public void Define(string variable)
    {
        variables.Add(variable, "");
        variableTypes.Add(variable, Type.NotSet);
    }

    public void Define(string function, string[] args, Expression body, Context innerContext)
    {
        if (!IsDefined(function, args.Length)){
            if (!functions.ContainsKey(function)){
                functions[function] = new Dictionary<string[], (Expression, Context)>();
            }
            functions[function].Add(args, (body, innerContext));
        }
    }

    public void Undefine(string function, string[] args)
    {
        if (!functions[function].Remove(args)){
            if (parent != null){
                parent.Undefine(function, args);
            }
        }
    }

    public void SetValue(string variable, Expression expression)
    {
        if (IsDefined(variable)){
            if (variables.ContainsKey(variable)){
                variables[variable] = expression.Evaluate(this);
                SetType(variable);
            }
            else if (parent != null){
                parent.SetValue(variable, expression);
            }
        }
    }

    public bool SetType(string variable, Type type = Type.NotSet)
    {
        if (IsDefined(variable)){
            if (variables.ContainsKey(variable)){
                if (type != Type.NotSet){
                    if (variableTypes[variable] == Type.NotSet && variableTypes[variable] != type){
                        variableTypes[variable] = type;
                        return true;
                    }
                    else if (variableTypes[variable] == type)
                        return true;
                    else
                        return false;
                }
                if (variableTypes[variable] != Type.NotSet)
                    return false;
                if (variables[variable] != ""){
                    if (double.TryParse(variables[variable], out double n))
                        variableTypes[variable] = Type.Number;
                    else if (bool.TryParse(variables[variable], out bool b))
                        variableTypes[variable] = Type.Boolean;
                    else
                        variableTypes[variable] = Type.String;
                    return true;
                }
            }
            else if (parent != null){
                parent.SetType(variable);
            }
        }
        return false;
    }

    public string GetValue(string variable)
    {
        if (variables.ContainsKey(variable)){
            if (variables[variable] != ""){
                return variables[variable];
            }
        }
        if (parent != null){
            return parent.GetValue(variable);
        }
        return "";
    }

    public Type GetType(string variable)
    {
        if (variableTypes.ContainsKey(variable)){
            return variableTypes[variable];
        }
        if (parent != null){
            return parent.GetType(variable);
        }
        return Type.NotSet;
    }

    public Dictionary<string[], (Expression, Context)> GetFunction(string function)
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