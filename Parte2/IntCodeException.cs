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

        public IntCodeException(string message)
            : base(message) { }
    }
}
