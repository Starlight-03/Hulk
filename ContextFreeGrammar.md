# Context Free Grammar

    E = T
      | T + E
    
    T = int * T
      | int
      | (E)

Básicamente, lo que hemos hecho ha sido probar todas las posibles derivaciones extrema izquierda, de forma recursiva, podando inteligentemente cada vez que era evidente que habíamos producido una derivación incorrecta. Tratemos de formalizar este proceso

## Parsing  Recursivo Descendente

De  forma  general,  tenemos  tres  tipos  de  operaciones  o  situaciones  que  analizar:

    - La  expansión de un no-terminal  en la forma  oracional  actual
    - La prueba recursiva de cada una de las producciones de este no-terminal
    - La comprobación de que un terminal generado coincide con el terminal esperado en la cadena