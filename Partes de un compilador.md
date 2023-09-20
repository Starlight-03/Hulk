# Partes de un compilador

## Front-end o Analizador

Se habla de «analizador» ya que se encarga de realizar el análisis del código fuente a compilar, lo valida e interactúa con el usuario. Además, suele ser independiente de la plataforma en la que se trabaja.

## Back-end o Generador

Es la parte del compilador que, a partir de los resultados de análisis, se encarga de generar el código para la máquina según la plataforma específica.
Fases de análisis de un compilador

## Análisis léxico

Reconocimiento de los elementos del lenguaje, agrupados en componentes llamados «tokens».

## Análisis sintáctico

Los componentes léxicos ahora se agrupan jerárquicamente en frases gramaticales para analizar su estructura según reglas y patrones específicos.

## Análisis semántico

Se utiliza la estructura jerárquica determinada por la fase de análisis sintáctico para identificar eventuales errores  semánticos.

## Generación del código intermedio

Después de las fases de análisis, se genera una representación de código intermedio para una máquina abstracta.

## Optimización del código

Se mejora la representación de código intermedio para obtener un código más rápido de ejecutar.