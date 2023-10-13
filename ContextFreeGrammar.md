# Context Free Grammar

   Expr -> Print
         | Func
         | LetIn
         | IfElse
         | Val

   Val -> Ident
        | NumExpr
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

   Bool -> Compar
         | !Bool
         | Bool OpBool
         | litBool

   LitBool -> true
            | false

   OpBool -> & Bool
           | | Bool

   Compar -> NumExpr OpComp

   OpComp -> < NumExpr
           | <= NumExpr
           | > NumExpr
           | >= NumExpr
           | == NumExpr
           | != NumExpr

## Str

   Str -> StrExpr Conc

   Conc -> @ Val
         | e

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

   LetIn -> let Var in (Expr)

   Var -> Ident = Val Var1

   Var1 -> , Var
         | e

## IfElse

   IfElse -> if (Bool) Expr else Expr
