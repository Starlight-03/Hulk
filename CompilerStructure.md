# Estructura general de un compilador

           +-------+                           +-----------+
COOL  ==>  | LEXER |                           | GENERADOR |  ==>  MIPS
           +-------+       +-----------+       +-----------+
               ||      =>  | SEMANTICO |  =>        ||
           +--------+      +-----------+      +--------------+
           | PARSER |                         | OPTIMIZACION |
           +--------+                         +--------------+

## Cinco fases fundamentales

### Análisis  Lexicográﬁco (lexer)

Donde se realiza la conversión de una secuencia de caracteres a una secuencia de tokens.

En esta fase se detectan los errores relacionados con la escritura incorrecta de símbolos, por ejemplo, un número decimal con más dígitos de los permitidos, una cadena de texto que no termina en ", un identiﬁcador que usa un caracter inválido (e.j. $).

### Análisis  Sintáctico (parser)
  
Donde se determina la estructura sintáctica del programa a partir de los tokens y se obtiene una estructura intermedia.
  
En esta fase se detectan los errores relacionados con la sintaxis, por ejemplo, un paréntesis no balanceado, una función que no tiene cuerpo, un if-else con la parte else vacía.

### Análisis  Semántico

Donde se veriﬁcan las condiciones semánticas del programa y se valida el uso correcto de todos los símbolos deﬁnidos.

En esta fase se determinan los errores relacionados con los símbolos y tipos, por ejemplo, variables no declaradas o usadas antes de su declaración o funciones invocadas con un número o un tipo incorrecto de los argumentos.

### Optimización

Donde se eliminan o simpliﬁcan secciones del programa en función de la arquitectura de máquina hacian donde se vaya a compilar.

### Generación

Donde se convierte ﬁnalmente la estructura semántica y optimizada hacia el lenguaje objetivo en la arquitectura donde será ejecutado.
