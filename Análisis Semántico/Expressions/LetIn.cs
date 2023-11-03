namespace HULK;

public class LetIn : Expression
{
    private readonly Dictionary<string, Expression> Variables;
    
    private readonly Expression Body;

    public LetIn(Dictionary<string, Expression> variables, Expression body)
    {
        Variables = new Dictionary<string, Expression>();
        foreach (string variable in variables.Keys)
            Variables[variable] = variables[variable];

        Body = body;
    }

    public override bool Validate(Context context)
    {
        Context innerContext = new Context(context);

        foreach (string variable in Variables.Keys){
            if (!Variables[variable].Validate(context))
                return false;
            innerContext.Define(variable);
            innerContext.SetValue(variable, Variables[variable]);
        }

        if (!Body.Validate(innerContext)){
            return false;
        }
        
        Type = Body.Type;
        return true;
    }

    public override string Evaluate(Context context)
    {
        Context innerContext = new Context(context);

        foreach (string variable in Variables.Keys){
            innerContext.Define(variable);
            innerContext.SetValue(variable, Variables[variable]);
        }

        return Body.Evaluate(innerContext);
    }
}