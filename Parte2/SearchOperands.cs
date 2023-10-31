using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Parte3;

namespace intCodeProgram
{
    public class SearchOperands
    {
        //Tope de sustantivo y verbo a buscar
        private const int MAX_SUSTANTIVE_VALUE = 999;
        private const int MAX_VERB_VALUE = 999;

        //Método que busca el sustantivo y el verbo para un resultado de forma sincrona
        public String[] search(String[] contentFile, String result)
        {
            String[] intCode = contentFile;
            ProcessIntCode processIntCode = new ProcessIntCode();;
            
            //Probamos con todos los valores del sustantivo desde el 0 hasta maxSustantive
            //y de verb desde 0 hasta maxVerb
            int intCodeLength = contentFile.Length;
            
            int maxSustantive = getMaxSustantive(intCodeLength);
            int maxVerb = getMaxVerb(intCodeLength);
            for (int sustantive = 0; sustantive < maxSustantive; sustantive++)
            {
                for (int verb = 0; verb < maxVerb; verb++)
                {
                    contentFile[1] = sustantive.ToString();
                    contentFile[2] = verb.ToString();

                    //Lanzamos el método que procesa los operandos
                    String[] response = processIntCode.run(contentFile);

                    if (response[0].Equals(result))
                    {
                        return new string[] { sustantive.ToString(), verb.ToString() };
                    }

                }
            }
            //Si existe algun problema lanzamos un error
            throw new IntCodeException(IntCodeException.ERROR_OPERANDS_NOT_FOUND);
        }

        

        //Comprueba que la longitud de la cadena de valores esta por debajo del valor maximo del sustantivo.
        //Si es asi devuelve la longitud de la cadena de valores, en caso contrario devuelve el valor de la constante
        private int getMaxSustantive(int intCodeLength)
        {
            if ((intCodeLength - 1)<MAX_SUSTANTIVE_VALUE) return intCodeLength - 1;
            return MAX_SUSTANTIVE_VALUE;
        }

        //Comprueba que la longitud de la cadena de valores esta por debajo del valor maximo del verbo.
        //Si es asi devuelve la longitud de la cadena de valores, en caso contrario devuelve el valor de la constante
        private int getMaxVerb(int intCodeLength)
        {
            if ((intCodeLength - 1)<MAX_VERB_VALUE) return intCodeLength - 1;
            return MAX_VERB_VALUE;
        }
    }
}
