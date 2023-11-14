namespace HULK;

public class Operator // Herramienta para las expresiones binarias
{
    // * Clase hecha para implementar el método ToString para facilitar el trabajo de 
    // lanzar una excepción en la expresión binaria dada

    // Todos los operadores son definidos por un enumerador, y tienen su propio valor en string
    
    public Op Op { get; private set; }

    public Operator(Op op)
    {
        Op = op;
    }

    public override string ToString()
    {
        switch (Op){
            case Op.Sum: return "+";
            case Op.Sub: return "-";
            case Op.Mul: return "*";
            case Op.Div: return "/";
            case Op.Mod: return "%";
            case Op.Pow: return "^";
            case Op.And: return "&";
            case Op.Or: return "|";
            case Op.Minor: return "<";
            case Op.MinorEqual: return "<=";
            case Op.Major: return ">";
            case Op.MajorEqual: return ">=";
            case Op.Equals: return "==";
            case Op.NotEqual: return "!=";
            case Op.Concat: return "@";
            default: return "";
        }
    }
}

public enum Op
{
    Sum,
    Sub,
    Mul,
    Div,
    Mod,
    Pow,
    And,
    Or,
    Minor,
    MinorEqual,
    Major,
    MajorEqual,
    Equals,
    NotEqual,
    Concat
}