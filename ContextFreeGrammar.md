# Context Free Grammar

   Expr -> Print
         | Func
         | LetIn
         | IfElse
         | Ident
         | Val

   Val -> NumExpr
        | Bool
        | Str

## NumExpr

   NumExpr -> Term (+|-) (*|/)

   (+|-) -> + NumExpr
          | - NumExpr
          | e

   Term -> int (*|/)
         | var (*|/)
         | (NumExpr) (*|/)

   (*|/) -> * Term
          | / Term
          | e

## Bool

   Bool -> (Bool)
         | Compar
         | !Bool
         | Bool & Bool
         | Bool | Bool
         | true
         | false

   Compar -> NumExpr < NumExpr
           | NumExpr <= NumExpr
           | NumExpr > NumExpr
           | NumExpr >= NumExpr
           | NumExpr == NumExpr
           | NumExpr != NumExpr

## Str

   S -> \" StrExpr \"

## Print

   Print -> print(Expr)

## Func

   Func -> function Ident => Expr

   Ident -> Id Param (+|-) (*|/)

   Param -> (Param1)
          | e

   Param1 -> Ident Param2
           | int Param2
           | e

   Param2 -> , Param1
           | e

## LetIn

   LetIn -> let Var in Expr

   Var -> Ident = Val Var1

   Var1 -> , Var
         | e

## IfElse

   IfElse -> if (Bool) Expr else Expr
