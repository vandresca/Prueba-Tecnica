using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parte3
{
   
    internal class IntCodeException : Exception
    {
        public static String ERROR_OVERFLOW_SUSTANTIVE_VERB = @"El sustantivo y/o verbo exceden de la longitud de la cadena de valores";
        public static String ERROR_OPERANDS_NOT_FOUND = @"No se ha podido hallar el sustantivo y el verbo para el resultado proporcionado";
        public static String ERROR_PATH = @"Debes introducir la ruta del fichero intCode";
        public static String ERROR_EMPTY_DATA = @"Debes introducir datos para el cálculo. Tienes 2 opciones:

               Añadir el sustantivo y el verbo y dejar el resultado en blanco
               Añadir el resultado y dejar el sustantivo y el verbo en blanco";
        public static String ERROR_WRONG_INSERT_DATA_ONE = @"Existe un problema para realizar el cálculo. O bien deja en blanco el sustantivo, o bien añade un valor númerico al verbo y deja el blanco el resultado";
        public static String ERROR_WRONG_INSERT_DATA_TWO = @"Existe un problema para realizar el cálculo. O bien deja en blanco el verbo, o bien añade un valor númerico al sustantivo y deja el blanco el resultado";
        public static String ERROR_NOT_VERB = @"Debes introducir el verbo";
        public static String ERROR_NOT_SUSTANTIVE = @"Debes introducir el sustantivo";
        public static String ERROR_ALL_DATA = @"Los campos sustantivo, verbo y resultado no pueden estar completos a la vez";


        public IntCodeException(string message)
            : base(message) { }
    }
}
