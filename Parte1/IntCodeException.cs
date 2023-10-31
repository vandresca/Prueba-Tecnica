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
   
        public IntCodeException(string message)
            : base(message) { }
    }
}
