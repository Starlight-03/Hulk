public class AbstractSintaxisTree
{
    public SemanticError Semantic { get; private set; }

    public string Value { get; private set; }

    public bool Valid { get; private set; }

    private readonly Expression expr;

    private readonly Context context;

    public AbstractSintaxisTree(Expression expr, Context context)
    {
        Semantic = new SemanticError();
        Value = "";
        Valid = false;
        this.expr = expr;
        this.context = context;
        Evaluate();
    }

    private void Evaluate()
    {
        if (expr.Validate(context)){
            Valid = true;
            expr.Evaluate(context);
            Value = expr.Value;
        }
    }
}

public class Context
{
    private Context parent;

    private Dictionary<string, Expression> variables;

    private Dictionary<string, Dictionary<string[], Expression>> functions;

    public Context(Context parent)
    {
        this.parent = parent;
        variables = new Dictionary<string, Expression>();
        functions = new Dictionary<string, Dictionary<string[], Expression>>();
    }
    
    public bool Define(string variable)
    {
        if (!variables.ContainsKey(variable))
        {
            variables.Add(variable, null);
            return true;
        }
        
        return false;
    }

    public bool Define(string function, string[] args, Expression body)
    {
        if (!IsDefined(function)){
            functions[function] = new Dictionary<string[], Expression>();
        }
        if (functions.ContainsKey(function) && !functions[function].ContainsKey(args)){
            functions[function].Add(args, body);
            return true;
        }
        else if (parent != null){
            return parent.Define(function, args, body);
        }
        else
            return false;
    }

    public void Undefine(string function, string[] args)
    {
        if (functions.ContainsKey(function) && functions[function].ContainsKey(args)){
            functions[function].Remove(args);
        }
        else if (parent != null){
            parent.Undefine(function, args);
        }
    }

    public bool IsDefined(string variable)
    {
        return variables.ContainsKey(variable) 
            || parent != null && parent.IsDefined(variable);
    }
    
    public bool IsDefined(string function, Expression[] args)
    {
        if (functions.ContainsKey(function)){
            foreach (var ar in functions[function].Keys){
                if (ar.Length == args.Length)
                    return true;
            }
        }
        
        return parent != null && parent.IsDefined(function, args);
    }

    public void SetValue(string variable, Expression expression)
    {
        if (IsDefined(variable)){
            if (variables.ContainsKey(variable))
                variables[variable] = expression;
            else if (parent != null)
                parent.SetValue(variable, expression);
        }
    }

    public Expression GetValue(string variable)
    {
        if (variables.ContainsKey(variable))
            return variables[variable];
        else if (parent != null){
            Expression val = parent.GetValue(variable);
            if (val != null)
                return val;
        }
        
        return null;
    }

    public Dictionary<string[], Expression> GetFunction(string function)
    {
        if (functions.ContainsKey(function))
            return functions[function];
        else if (parent != null){
            Dictionary<string[], Expression> func = parent.GetFunction(function);
            if (func != null)
                return func;
        }
        return null;
    }

    public Expression GetBody(string function, string[] args)
    {
        if (functions.ContainsKey(function) && functions[function].ContainsKey(args))
            return functions[function][args];
        else if (parent != null){
            Expression body = parent.GetBody(function, args);
            if (body != null)
                return body;
        }
        return null;
    }
}