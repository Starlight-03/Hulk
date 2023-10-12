# Context Free Grammar

    Expr -> E
          | F
          | I

    E -> T X Y

    X -> + E
       | - E
       | e
    
    T -> int Y
       | var Y
       | (E) Y

    Y -> * T
       | / T
       | e

    F -> print(Expr)
       | function I => Expr
       | let var = val in Expr
       | if (B) Expr else Expr

    I -> id P X Y

    P -> (J)
       | e

    J -> I K
       | int K
       | e

    K -> , J
       | e

    var -> id

    val -> E
         | B
         | S

    B -> (B)
       | C
       | !B
       | B & B
       | B | B
       | true
       | false

    C -> E < E
       | E <= E
       | E > E
       | E >= E
       | E == E
       | E != E
