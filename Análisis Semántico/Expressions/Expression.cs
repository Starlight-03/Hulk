namespace HULK;

public abstract class Expression // En esta calse se define nuestro árbol de sintaxis abstracta
{
    // De esta clase se derivan todos los tipos de expresiones definidos en nuestro compilador
    // A partir de aquí de revisará la semántica de las expresiones y se evaluarán dichas expresiones en caso de que la semántica sea correcta
    // Si ocurre algún error semántico se lanza un SemanticError
    // Toda expresión posee un error semántico en caso de que la semántica no esté correcta al validar
    // También poseen un tipo que devuelve cada expresión

    // Los tipos que devuelven las expresiones son: String, Number, Bool o, en su defecto, NotSet

    public SemanticError Semantic { get; protected set; }

    public Type Type { get; protected set; }

    public Expression()
    {
        Semantic = new SemanticError();
        Type = Type.NotSet;
    }

    public virtual void SetType(Context context, Type type) { }

    public abstract bool Validate(Context context);

    public abstract string Evaluate(Context context);
}

public enum Type 
{
    NotSet,
    String,
    Number,
    Boolean
}