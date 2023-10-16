public class AST
{
    
}

public class Context
{
    private Context parent;

    private HashSet<Context> children;

    private HashSet<string> variables;

    private Dictionary<string, string[]> functions;

    public Context(Context parent)
    {
        this.parent = parent;
        this.children = new HashSet<Context>();
        this.variables = new HashSet<string>();
        this.functions = new Dictionary<string, string[]>();
    }

    public bool IsDefined(string variable)
    {
        return this.variables.Contains(variable) 
            || this.parent != null && this.parent.IsDefined(variable);
    }
    
    public bool IsDefined(string function, int args)
    {
        return this.functions.ContainsKey(function) && this.functions[function].Length == args
            || this.parent != null && this.parent.IsDefined(function, args);
    }
    
    public bool Define(string variable)
    {
        return this.variables.Add(variable);
    }
    
    public bool Define(string function, string[] args)
    {
        // if (!this.functions.ContainsKey(function))
        //     return this.functions.TryAdd(function, args);
        // else if (this.functions[function].Length != args.Length)
        //     return this.functions.TryAdd(function, args);
        // else
        //     return false;

        if (this.functions.ContainsKey(function) 
            && this.functions[function].Length == args.Length)
                return false;

        this.functions[function] = args;
        return true;
    }

    public Context CreateChildContext()
    {
        return new Context(this);
    }
}

public abstract class Node
{
    public abstract bool Validate(Context context);
}

public class CompilerProgram : Node
{
    public List<Statement> Statements;

    public override bool Validate(Context context)
    {
        foreach (var statement in Statements)
            if (!statement.Validate(context))
                return false;

        return true;
    }
}

public abstract class Statement : Node
{

}

public class VarDecl : Statement
{
    public string Identifier;
    
    public Expression Expr;

    public override bool Validate(Context context)
    {
        if (!Expr.Validate(context))
            return false;
        
        if (!context.Define(Identifier))
            return false;
        
        return true;
    }
}

public class FuncDef : Statement
{
    public string Identifier;

    public List<string> Args;

    public Expression Body;

    public override bool Validate(Context context)
    {
        Context innerContext = context.CreateChildContext();

        foreach (var arg in this.Args)
            innerContext.Define(arg);

        return Body.Validate(innerContext) && context.Define(Identifier, Args.ToArray());
    }
}

public class Print : Statement
{
    public Expression Expr;

    public override bool Validate(Context context)
    {
        return Expr.Validate(context);
    }
}

public abstract class Expression : Node
{
    
}

public abstract class BynaryExpression : Expression
{
    public Operator Op;

    public Expression Left;
    
    public Expression Right;

    public override bool Validate(Context context)
    {
        return Left.Validate(context) && Right.Validate(context);
    }
}

public enum Operator
{
    Add,
    Sub,
    Mult,
    Div,
    Mod
}


public abstract class AtomicExpression : Expression
{
    
}

public class FuncCall : AtomicExpression
{
    public string Identifier;

    public List<Expression> Args;

    public override bool Validate(Context context)
    {
        foreach (var expr in Args)
            if (!expr.Validate(context))
                return false;
                
        return context.IsDefined(Identifier, Args.Count);
    }
}

public class Variable : AtomicExpression
{
    public string Identifier;

    public override bool Validate(Context context)
    {
        return context.IsDefined(Identifier);
    }
}

public class Number : AtomicExpression
{
    public string Value;

    public override bool Validate(Context context)
    {
        return true;
    }
}