public class AST
{
    
}

public abstract class Node
{

}

public class Program
{
    public List<Statement> Statements;
}

public abstract class Statement
{

}

public class VarDecl : Statement
{
    public string Identifier;
    
    public Expression Expr;
}

public class FuncDef : Statement
{
    public string Identifier;

    public List<string> Args;

    public Expression Expr;
}

public class Print : Statement
{
    public Expression Expr;
}

public abstract class Expression : Node
{
    
}

public class BynaryExpression : Expression
{
    public Operator Op;

    public Expression Left;
    
    public Expression Right;
}

public enum Operator
{
    Add,
    Sub,
    Mult,
    Div,
    Mod
}

public class FuncCall : Expression
{
    public string Identifier;

    public List<Expression> Args;
}

public class Variable : Expression
{
    public string Identifier;
}

public class Number : Expression
{
    public string Value;
}