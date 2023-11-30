namespace HULK;

public abstract class LiteralExpression : Expression // Las expresiones literales son el final de nuestro árbol de sintáxis abstracta
{
    // Las expresiones literales poseen un valor literal del tipo que fuera dicha expresión
    // Son válidas estas expresiones de por sí, así que sólo hay que devolver true al validarlas
    // Al evaluarlas sólo hace falta devolver el valor

    // Las expresiones literales pueden ser: de String, numéricas, booleanas o, funciones predeterminadas (estas últimas son especiales)
    private readonly string value;

    protected LiteralExpression(string value) { this.value = value; }

    public override bool Validate(Context context) => true;

    public override string Evaluate(Context context) => value;
}

public class StringLiteral : LiteralExpression
{
    public StringLiteral(string value) : base(value) { Type = Type.String; }
}

public class NumericLiteral : LiteralExpression
{
    public NumericLiteral(string value) : base(value) { Type = Type.Number; }
}

public class BooleanLiteral : LiteralExpression
{
    public BooleanLiteral(string value) : base(value) { Type = Type.Boolean; }
}