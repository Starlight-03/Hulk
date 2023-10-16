// public abstract class Expression
// {
//     public abstract int Evaluate(Context context);
//     public override string ToString()
//     {
//         return base.ToString();
//     }
// }

// public abstract class BinaryExpression : Expression
// {
//     protected Expression Left;
//     protected Expression Right;
// }

// public abstract class UnaryExpression : Expression
// {
//     protected Expression Exp;
// }

// public class Sum : BinaryExpression
// {
//     public Sum(Expression left, Expression right)
//     {
//         Left = left;
//         Right = right;
//     }

//     public override int Evaluate(Context context)
//     {
//         return Left.Evaluate(context) + Right.Evaluate(context);
//     }
// }

// public class Substract : BinaryExpression
// {
//     public Substract(Expression left, Expression right)
//     {
//         Left = left;
//         Right = right;
//     }

//     public override int Evaluate(Context context)
//     {
//         return Left.Evaluate(context) - Right.Evaluate(context);
//     }
// }

// public class Mult : BinaryExpression
// {
//     public Mult(Expression left, Expression right)
//     {
//         Left = left;
//         Right = right;
//     }

//     public override int Evaluate(Context context)
//     {
//         return Left.Evaluate(context) * Right.Evaluate(context);
//     }
// }

// public class Div : BinaryExpression
// {
//     public Div(Expression left, Expression right)
//     {
//         Left = left;
//         Right = right;
//     }

//     public override int Evaluate(Context context)
//     {
//         return Left.Evaluate(context) / Right.Evaluate(context);
//     }
// }

// public class Increment : UnaryExpression
// {
//     public Increment(Expression exp)
//     {
//         Exp = exp;
//     }

//     public override int Evaluate(Context context)
//     {
//         return Exp.Evaluate(context) + 1;
//     }
// }

// public class Number : Expression
// {
//     private int Value;
//     public Number(int value)
//     {
//         Value = value;
//     }
//     public override int Evaluate(Context context)
//     {
//         return Value;
//     }

//     public override string ToString()
//     {
//         return this.Value.ToString();
//     }
// }

// public class Variable : Expression
// {
//     private string Name;
//     public Variable(string name)
//     {
//         Name = name;
//     }
//     public override int Evaluate(Context context)
//     {
//         return context.Get(Name);
//     }
// }

// public class Assign : Expression
// {
//     private string Name;
//     private Expression Value;

//     public Assign(string name, Expression value)
//     {
//         Name = name;
//         Value = value;
//     }
//     public override int Evaluate(Context context)
//     {
//         throw new NotImplementedException();
//     }
// }

// public class If_Else : Expression
// {
//     // Una expresión if tiene tres elementos: condición, parte del cuerpo true y parte del cuerpo false
//     private Expression Condition;
//     private Expression Positive;
//     private Expression Negative;
//     public If_Else(Expression condition, Expression positive, Expression negative)
//     {
//         Condition = condition;
//         Positive = positive;
//         Negative = negative;
//     }

//     public override int Evaluate(Context context)
//     {
//         int c = Condition.Evaluate(context);

//         if (c == 1)
//             return Positive.Evaluate(context);
//         else
//             return Negative.Evaluate(context);
//     }
// }