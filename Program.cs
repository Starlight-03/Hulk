namespace HULK;

internal class Program
{
    private static void Main(string[] args) // Aquí se inicializan el intérprete y todas las variables que va a utilizar
    {
        Context globalContext = InitializeGlobalContext();

        while (true){
            Console.Write("> ");                    // Al inicar cada instrucción, denotamos una pequeña línea de código 
                                                    // donde dejamos que el usuario introduzca su línea de código
            var line = Console.ReadLine();          // Luego tomamos la línea de código que nos introduce el usuario
            if (line == ""){                        // Si la línea está vacía, interpretamos que el usuario desea cerrar el programa
                break;
            }
            var lexer = new Lexer();                // Luego iniciamos el proceso de parseo, empezando por tokenizar la línea de código
            var tokens = lexer.Tokenize(line);      // (convertimos la línea de código en un conjunto de tokens que son más fáciles de leer próximamente)
            if (tokens == null){                    // Si la lista dada es nula, significa que ocurrió un error léxico
                continue;                           // Entonces volvemos a empezar la iteración desde el principio
            }
            var parser = new Parser();              // Si no hubo errores léxicos, empezamos a parsear
            var expression = parser.Parse(tokens);  // (leemos la lista de tokens devuelta por el lexer y evaluamos la sintaxis de la expressión)
            if (expression == null){                // Si la expressión devuelta es nula, significa que ocurrió un error sintáctico
                continue;                           // Entonces volvemos a empezar la iteración desde el principio
            }
            if (!expression.Validate(globalContext)){// Si no ocurrieron errores sintácticos, entonces comenzamos a evaluar la semántica
                continue;                           // Si la semántica no es correcta, entonces ocurrió un error semántico, y volvemos a empezar
            }
            var value = expression.Evaluate(globalContext);// Si no han ocurrido errores durante toda la ejecución, entonces estamos listos para evaluar la expressión semantica
            if (value != ""){                       // Si tiene un valor para devolver, lo presentamos en pantalla; sino se continúa con la siguiente iteración
                Console.WriteLine(value);
            }
        }
    }

    private static Context InitializeGlobalContext()
    {
        // Para poder evaluar las variables y funciones es necesario guardarlos todos en un contexto, ya que las variables pueden cambiar su valor, etc.
        // Para ello creamos un contexto global que nos servirá para todo el flujo de nuestro programa, en el cual guardaremos las funciones y 
        // variables predeterminadas, y el que será el padre de todos los contextos internos de cada expressión que se utilice.

        // Provisionalmente solo hemos implementado las variables numéricas PI y E, y las funciones (también numéricas) seno, coseno, 
        // logaritmo usual (con base y argumento como parámetros) y logaritmo natural
        
        Context context = new Context(null);

        context.Define("PI", Math.PI.ToString(), Type.Number);
        context.Define("E", Math.E.ToString(), Type.Number);

        context.Define(PredFunc.sin, Type.Number, new Dictionary<string, Type>() {{"x", Type.Number}});
        context.Define(PredFunc.cos, Type.Number, new Dictionary<string, Type>() {{"x", Type.Number}});
        context.Define(PredFunc.log, Type.Number, new Dictionary<string, Type>() {{"a", Type.Number}, {"b", Type.Number}});
        context.Define(PredFunc.ln, Type.Number, new Dictionary<string, Type>() {{"x", Type.Number}});

        return context;
    }
}